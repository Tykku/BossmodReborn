namespace BossMod.Global.MaskedCarnivale;

public static class Layouts
{
    private static readonly Shape[] circle = [new Circle(new(100, 100), 24.5f)];

    private static readonly WPos[] wall1a = [new WPos(85, 95), new WPos(95, 95), new WPos(95, 94.5f), new WPos(85, 94.5f)];
    private static readonly WPos[] wall1b = [new WPos(95, 94.5f), new WPos(95, 89), new WPos(94.5f, 89), new WPos(94.5f, 94.5f)];
    private static readonly WPos[] wall2a = [new WPos(105, 95), new WPos(115, 95), new WPos(115, 94.5f), new WPos(105, 94.5f)];
    private static readonly WPos[] wall2b = [new WPos(105, 94.5f), new WPos(105, 89), new WPos(105.5f, 89), new WPos(105.5f, 94.5f)];
    private static readonly Shape[] walls = [new PolygonCustom(wall1a), new PolygonCustom(wall1b), new PolygonCustom(wall2a), new PolygonCustom(wall2b)];
    public static readonly ArenaBounds Layout2Corners = new ArenaBoundsComplex(circle, walls);

    private static readonly WPos[] square1 = [new(107, 110), new(110, 113), new(113, 110), new(110, 107)];
    private static readonly WPos[] square2 = [new(93, 110), new(90, 107), new(87, 110), new(90, 113)];
    private static readonly WPos[] square3 = [new(90, 93), new(93, 90), new(90, 87), new(87, 90)];
    private static readonly WPos[] square4 = [new(110, 93), new(113, 90), new(110, 87), new(107, 90)];
    private static readonly Shape[] squares = [new PolygonCustom(square1), new PolygonCustom(square2), new PolygonCustom(square3), new PolygonCustom(square4)];
    public static readonly ArenaBounds Layout4Quads = new ArenaBoundsComplex(circle, squares);

    private static readonly WPos[] squareBig = [new WPos(100, 107), new WPos(107, 100), new WPos(100, 93), new WPos(93, 100)];
    public static readonly ArenaBounds LayoutBigQuad = new ArenaBoundsComplex(circle, [new PolygonCustom(squareBig)]);
}
