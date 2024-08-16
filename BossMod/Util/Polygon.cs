﻿using Clipper2Lib;
using EarcutNet;

// currently we use Clipper2 library (based on Vatti algorithm) for boolean operations and Earcut.net library (earcutting) for triangulating
// note: the major user of these primitives is bounds clipper; since they operate in 'local' coordinates, we use WDir everywhere (offsets from center) and call that 'relative polygons' - i'm not quite happy with that, it's not very intuitive
namespace BossMod;

// a triangle; as basic as it gets
public readonly record struct RelTriangle(WDir A, WDir B, WDir C);

// a complex polygon that is a single simple-polygon exterior minus 0 or more simple-polygon holes; all edges are assumed to be non intersecting
// hole-starts list contains starting index of each hole
public record class RelPolygonWithHoles(List<WDir> Vertices, List<int> HoleStarts)
{
    // constructor for simple polygon
    public RelPolygonWithHoles(List<WDir> simpleVertices) : this(simpleVertices, []) { }

    public ReadOnlySpan<WDir> AllVertices => Vertices.AsSpan();
    public ReadOnlySpan<WDir> Exterior => AllVertices[..ExteriorEnd];
    public ReadOnlySpan<WDir> Interior(int index) => AllVertices[HoleStarts[index]..HoleEnd(index)];
    public IEnumerable<int> Holes => Enumerable.Range(0, HoleStarts.Count);
    public IEnumerable<(WDir, WDir)> ExteriorEdges => PolygonUtil.EnumerateEdges(Vertices.Take(ExteriorEnd));
    public IEnumerable<(WDir, WDir)> InteriorEdges(int index) => PolygonUtil.EnumerateEdges(Vertices.Skip(HoleStarts[index]).Take(HoleEnd(index) - HoleStarts[index]));

    public bool IsSimple => HoleStarts.Count == 0;
    public bool IsConvex => IsSimple && PolygonUtil.IsConvex(Exterior);

    private int ExteriorEnd => HoleStarts.Count > 0 ? HoleStarts[0] : Vertices.Count;
    private int HoleEnd(int index) => index + 1 < HoleStarts.Count ? HoleStarts[index + 1] : Vertices.Count;

    // add new hole; input is assumed to be a simple polygon
    public void AddHole(ReadOnlySpan<WDir> simpleHole)
    {
        HoleStarts.Add(Vertices.Count);
        Vertices.AddRange(simpleHole);
    }
    public void AddHole(IEnumerable<WDir> simpleHole)
    {
        HoleStarts.Add(Vertices.Count);
        Vertices.AddRange(simpleHole);
    }

    // build a triangulation of the polygon
    public bool Triangulate(List<RelTriangle> result)
    {
        var pts = new List<double>(Vertices.Count * 2);
        foreach (var p in Vertices)
        {
            pts.Add(p.X);
            pts.Add(p.Z);
        }

        var tess = Earcut.Tessellate(pts, HoleStarts);
        for (var i = 0; i < tess.Count; i += 3)
            result.Add(new(Vertices[tess[i]], Vertices[tess[i + 1]], Vertices[tess[i + 2]]));
        return tess.Count > 0;
    }

    public List<RelTriangle> Triangulate()
    {
        List<RelTriangle> result = [];
        Triangulate(result);
        return result;
    }

    // point-in-polygon test; point is defined as offset from shape center
    public bool Contains(WDir p)
    {
        if (!InSimplePolygon(p, ExteriorEdges))
            return false;
        foreach (var h in Holes)
            if (InSimplePolygon(p, InteriorEdges(h)))
                return false;
        return true;
    }

    private static bool InSimplePolygon(WDir p, IEnumerable<(WDir, WDir)> edges)
    {
        // for simple polygons, it doesn't matter which rule (even-odd, non-zero, etc) we use
        // so let's just use non-zero rule and calculate winding order
        // we need to select arbitrary direction to count winding intersections - let's select unit X
        var winding = 0;
        const float epsilon = 1e-6f;

        foreach (var (a, b) in edges)
        {
            // see whether edge ab intersects our test ray - it has to intersect the infinite line on the correct side
            var pa = a - p;
            var pb = b - p;

            if (PointOnLineSegment(p, a, b, epsilon))
                return true;

            // if pa.Z and pb.Z have the same signs, the edge is fully above or below the test ray
            if (pa.Z <= 0)
            {
                if (pb.Z > 0 && pa.Cross(pb) > 0)
                    ++winding;
            }
            else
            {
                if (pb.Z <= 0 && pa.Cross(pb) < 0)
                    --winding;
            }
        }
        return winding != 0;
    }

    private static bool PointOnLineSegment(WDir point, WDir a, WDir b, float epsilon)
    {
        var crossProduct = (point.Z - a.Z) * (b.X - a.X) - (point.X - a.X) * (b.Z - a.Z);
        if (MathF.Abs(crossProduct) > epsilon)
            return false;

        var dotProduct = (point.X - a.X) * (b.X - a.X) + (point.Z - a.Z) * (b.Z - a.Z);
        if (dotProduct < 0)
            return false;

        var squaredLengthBA = (b.X - a.X) * (b.X - a.X) + (b.Z - a.Z) * (b.Z - a.Z);
        return dotProduct <= squaredLengthBA;
    }

    public static Func<WPos, float> PolygonWithHoles(WPos origin, RelSimplifiedComplexPolygon polygon)
    {
        float distanceFunc(WPos p)
        {
            var localPoint = new WDir(p.X - origin.X, p.Z - origin.Z);
            var isInside = polygon.Contains(localPoint);
            var minDistance = polygon.Parts.SelectMany(part => part.ExteriorEdges)
                .Min(edge => PolygonUtil.DistanceToEdge(p, PolygonUtil.ConvertToWPos(origin, edge)));

            Parallel.ForEach(polygon.Parts, part =>
            {
                Parallel.ForEach(part.Holes, holeIndex =>
                {
                    var holeMinDistance = part.InteriorEdges(holeIndex)
                        .Min(edge => PolygonUtil.DistanceToEdge(p, PolygonUtil.ConvertToWPos(origin, edge)));
                    lock (polygon)
                        minDistance = Math.Min(minDistance, holeMinDistance);
                });
            });
            return isInside ? -minDistance : minDistance;
        }
        return ShapeDistance.CacheFunction(distanceFunc);
    }

    public static Func<WPos, float> InvertedPolygonWithHoles(WPos origin, RelSimplifiedComplexPolygon polygon)
    {
        var polygonWithHoles = PolygonWithHoles(origin, polygon);
        return p => -polygonWithHoles(p);
    }
}

// generic 'simplified' complex polygon that consists of 0 or more non-intersecting polygons with holes (note however that some polygons could be fully inside other polygon's hole)
public record class RelSimplifiedComplexPolygon(List<RelPolygonWithHoles> Parts)
{
    public bool IsSimple => Parts.Count == 1 && Parts[0].IsSimple;
    public bool IsConvex => Parts.Count == 1 && Parts[0].IsConvex;

    public RelSimplifiedComplexPolygon() : this(new List<RelPolygonWithHoles>()) { }

    // constructors for simple polygon
    public RelSimplifiedComplexPolygon(List<WDir> simpleVertices) : this([new RelPolygonWithHoles(simpleVertices)]) { }
    public RelSimplifiedComplexPolygon(IEnumerable<WDir> simpleVertices) : this([new RelPolygonWithHoles([.. simpleVertices])]) { }

    // build a triangulation of the polygon
    public List<RelTriangle> Triangulate()
    {
        List<RelTriangle> result = [];
        foreach (var p in Parts)
            p.Triangulate(result);
        return result;
    }

    // point-in-polygon test; point is defined as offset from shape center
    public bool Contains(WDir p) => Parts.Any(part => part.Contains(p));

    // positive offsets inflate, negative shrink polygon
    public RelSimplifiedComplexPolygon Offset(float Offset)
    {
        var offset = new ClipperOffset();
        var exteriorPaths = new List<Path64>();
        var holePaths = new List<Path64>();

        foreach (var part in Parts)
        {
            var exteriorPath = new Path64(part.Exterior.Length);
            foreach (var vertex in part.Exterior)
                exteriorPath.Add(new Point64(vertex.X * PolygonClipper.Scale, vertex.Z * PolygonClipper.Scale));
            exteriorPaths.Add(exteriorPath);

            foreach (var holeIndex in part.Holes)
            {
                var holePath = new Path64(part.Interior(holeIndex).Length);
                foreach (var vertex in part.Interior(holeIndex))
                    holePath.Add(new Point64(vertex.X * PolygonClipper.Scale, vertex.Z * PolygonClipper.Scale));
                holePaths.Add(holePath);
            }
        }

        foreach (var path in exteriorPaths)
            offset.AddPath(path, JoinType.Miter, EndType.Polygon);

        var expandedHoles = new List<Path64>();
        foreach (var path in holePaths)
        {
            var holeOffset = new ClipperOffset();
            holeOffset.AddPath(path, JoinType.Miter, EndType.Polygon);
            var expandedHole = new Paths64();
            holeOffset.Execute(-Offset * PolygonClipper.Scale, expandedHole);
            expandedHoles.AddRange(expandedHole);
        }

        var solution = new Paths64();
        offset.Execute(Offset * PolygonClipper.Scale, solution);

        var result = new RelSimplifiedComplexPolygon();
        foreach (var path in solution)
        {
            var vertices = path.Select(pt => new WDir(pt.X * PolygonClipper.InvScale, pt.Y * PolygonClipper.InvScale)).ToList();
            result.Parts.Add(new RelPolygonWithHoles(vertices));
        }

        foreach (var path in expandedHoles)
        {
            var vertices = path.Select(pt => new WDir(pt.X * PolygonClipper.InvScale, pt.Y * PolygonClipper.InvScale)).ToList();
            result.Parts.Last().AddHole(vertices);
        }
        return result;
    }
}

// utility for simplifying and performing boolean operations on complex polygons
public class PolygonClipper
{
    public const float Scale = 1024 * 1024; // note: we need at least 10 bits for integer part (-1024 to 1024 range); using 11 bits leaves 20 bits for fractional part; power-of-two scale should reduce rounding issues
    public const float InvScale = 1 / Scale;

    // reusable representation of the complex polygon ready for boolean operations
    public record class Operand
    {
        public Operand() { }
        public Operand(ReadOnlySpan<WDir> contour, bool isOpen = false) => AddContour(contour, isOpen);
        public Operand(IEnumerable<WDir> contour, bool isOpen = false) => AddContour(contour, isOpen);
        public Operand(RelPolygonWithHoles polygon) => AddPolygon(polygon);
        public Operand(RelSimplifiedComplexPolygon polygon) => AddPolygon(polygon);

        private readonly ReuseableDataContainer64 _data = new();

        public void Clear() => _data.Clear();

        public void AddContour(ReadOnlySpan<WDir> contour, bool isOpen = false)
        {
            Path64 path = new(contour.Length);
            foreach (var p in contour)
                path.Add(ConvertPoint(p));
            AddContour(path, isOpen);
        }

        public void AddContour(IEnumerable<WDir> contour, bool isOpen = false) => AddContour([.. contour.Select(ConvertPoint)], isOpen);

        public void AddPolygon(RelPolygonWithHoles polygon)
        {
            AddContour(polygon.Exterior);
            foreach (var i in polygon.Holes)
                AddContour(polygon.Interior(i));
        }

        public void AddPolygon(RelSimplifiedComplexPolygon polygon) => polygon.Parts.ForEach(AddPolygon);

        public void Assign(Clipper64 clipper, PathType role) => clipper.AddReuseableData(_data, role);

        private void AddContour(Path64 contour, bool isOpen) => _data.AddPaths([contour], PathType.Subject, isOpen);
    }

    private readonly Clipper64 _clipper = new() { PreserveCollinear = false };

    public RelSimplifiedComplexPolygon Simplify(Operand poly, FillRule fillRule = FillRule.NonZero)
    {
        poly.Assign(_clipper, PathType.Subject);
        return Execute(ClipType.Union, fillRule);
    }

    public RelSimplifiedComplexPolygon Intersect(Operand p1, Operand p2, FillRule fillRule = FillRule.NonZero) => Execute(ClipType.Intersection, fillRule, p1, p2);
    public RelSimplifiedComplexPolygon Union(Operand p1, Operand p2, FillRule fillRule = FillRule.NonZero) => Execute(ClipType.Union, fillRule, p1, p2);
    public RelSimplifiedComplexPolygon Difference(Operand starting, Operand remove, FillRule fillRule = FillRule.NonZero) => Execute(ClipType.Difference, fillRule, starting, remove);
    public RelSimplifiedComplexPolygon Xor(Operand p1, Operand p2, FillRule fillRule = FillRule.NonZero) => Execute(ClipType.Xor, fillRule, p1, p2);

    private RelSimplifiedComplexPolygon Execute(ClipType operation, FillRule fillRule, Operand subject, Operand clip)
    {
        subject.Assign(_clipper, PathType.Subject);
        clip.Assign(_clipper, PathType.Clip);
        return Execute(operation, fillRule);
    }

    private RelSimplifiedComplexPolygon Execute(ClipType operation, FillRule fillRule)
    {
        var solution = new PolyTree64();
        _clipper.Execute(operation, fillRule, solution);
        _clipper.Clear();

        var result = new RelSimplifiedComplexPolygon();
        BuildResult(result, solution);
        return result;
    }

    private static void BuildResult(RelSimplifiedComplexPolygon result, PolyPath64 parent)
    {
        for (var i = 0; i < parent.Count; ++i)
        {
            var exterior = parent[i];
            RelPolygonWithHoles poly = new([.. exterior.Polygon?.Select(ConvertPoint) ?? throw new InvalidOperationException("Unexpected null polygon list")]);
            result.Parts.Add(poly);
            for (var j = 0; j < exterior.Count; ++j)
            {
                var interior = exterior[j];
                poly.AddHole(interior.Polygon?.Select(ConvertPoint) ?? throw new InvalidOperationException("Unexpected null hole list"));

                // add nested polygons
                BuildResult(result, interior);
            }
        }
    }

    private static Point64 ConvertPoint(WDir pt) => new(pt.X * Scale, pt.Z * Scale);
    private static WDir ConvertPoint(Point64 pt) => new(pt.X * InvScale, pt.Y * InvScale);
}

public static class PolygonUtil
{
    public static IEnumerable<(T, T)> EnumerateEdges<T>(IEnumerable<T> contour) where T : struct, IEquatable<T>
    {
        using var e = contour.GetEnumerator();
        if (!e.MoveNext())
            yield break;

        var prev = e.Current;
        var first = prev;
        while (e.MoveNext())
        {
            var curr = e.Current;
            yield return (prev, curr);
            prev = curr;
        }
        if (!first.Equals(prev))
            yield return (prev, first);
    }

    public static bool IsConvex(ReadOnlySpan<WDir> contour)
    {
        // polygon is convex if cross-product of all successive edges has same sign
        if (contour.Length < 3)
            return false;

        var prevEdge = contour[0] - contour[^1];
        var cross = (contour[^1] - contour[^2]).Cross(prevEdge);
        if (contour.Length > 3)
        {
            for (var i = 1; i < contour.Length; ++i)
            {
                var currEdge = contour[i] - contour[i - 1];
                var curCross = prevEdge.Cross(currEdge);
                prevEdge = currEdge;
                if (curCross == 0)
                    continue;
                else if (cross == 0)
                    cross = curCross;
                else if ((cross < 0) != (curCross < 0))
                    return false;
            }
        }
        return cross != 0;
    }

    public static bool IsPointInsideConcavePolygon(WPos point, IEnumerable<WPos> vertices)
    {
        var intersections = 0;
        var verticesList = vertices.ToList();
        for (var i = 0; i < verticesList.Count; i++)
        {
            var a = verticesList[i];
            var b = verticesList[(i + 1) % verticesList.Count];
            if (RayIntersectsEdge(point, a, b))
                intersections++;
        }
        return intersections % 2 != 0;
    }

    public static bool RayIntersectsEdge(WPos point, WPos a, WPos b)
    {
        if (a.Z > b.Z)
            (b, a) = (a, b);
        if (point.Z == a.Z || point.Z == b.Z)
            point = new WPos(point.X, point.Z + 0.0001f);
        if (point.Z > b.Z || point.Z < a.Z || point.X >= Math.Max(a.X, b.X))
            return false;
        if (point.X < Math.Min(a.X, b.X))
            return true;
        var red = (point.Z - a.Z) / (b.Z - a.Z);
        var blue = (b.X - a.X) * red + a.X;
        return point.X < blue;
    }

    public static float DistanceToEdge(WPos p, (WPos p1, WPos p2) edge)
    {
        var (p1, p2) = edge;
        var edgeDir = p2 - p1;
        var len = edgeDir.Length();
        if (len == 0)
            return (p - p1).Length();

        var proj = (p - p1).Dot(edgeDir) / len;
        if (proj < 0)
            return (p - p1).Length();
        if (proj > len)
            return (p - p2).Length();

        var closestPoint = p1 + edgeDir * (proj / len);
        return (p - closestPoint).Length();
    }

    public static (WPos p1, WPos p2) ConvertToWPos(WPos origin, (WDir p1, WDir p2) edge)
    {
        return (new WPos(origin.X + edge.p1.X, origin.Z + edge.p1.Z), new WPos(origin.X + edge.p2.X, origin.Z + edge.p2.Z));
    }
}
