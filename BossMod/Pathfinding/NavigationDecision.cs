﻿namespace BossMod.Pathfinding;

// utility for selecting player's navigation target
// there are several goals that navigation has to meet, in following rough priority
// 1. stay away from aoes; tricky thing is that sometimes it is ok to temporarily enter aoe, if we're sure we'll exit it in time
// 2. maintain uptime - this is represented by being in specified range of specified target, and not moving to interrupt casts unless needed
// 3. execute positionals - this is strictly less important than points above, we only do that if we can meet other conditions
// 4. be in range of healers - even less important, but still nice to do
public struct NavigationDecision
{
    // context that allows reusing large memory allocations
    public sealed class Context
    {
        public Map Map = new();
        public Map Map2 = new();
        public ThetaStar ThetaStar = new();
    }

    public enum Decision
    {
        None,
        ImminentToSafe,
        ImminentToClosest,
        UnsafeToPositional,
        UnsafeToUptime,
        UnsafeToSafe,
        SafeToUptime,
        SafeToCloser,
        SafeBlocked,
        UptimeToPositional,
        UptimeBlocked,
        BackIntoBounds,
        Casting,
        Waiting,
        Optimal
    }

    public WPos? Destination;
    public float NextTurn; // > 0 if we turn left after reaching first waypoint, < 0 if we turn right, 0 otherwise (no more waypoints)
    public float LeewaySeconds; // can be used for finishing casts / slidecasting etc.
    public float TimeToGoal;
    public Map? Map;
    public int MapGoal;
    public Decision DecisionType;
    private static readonly AI.AIConfig _config = Service.Config.Get<AI.AIConfig>();

    public const float DefaultForbiddenZoneCushion = 0.354f; // 0.25 * sqrt(2) = distance from center to a corner for the standard 0.5 map resolution

    public static NavigationDecision Build(Context ctx, WorldState ws, AIHints hints, Actor player, WPos? targetPos, float targetRadius, Angle targetRot, Positional positional, float playerSpeed = 6, float forbiddenZoneCushion = DefaultForbiddenZoneCushion)
    {
        if (targetRadius < 1)
            targetRadius = 1; // ensure targetRadius is at least 1 to prevent game from freezing

        // local copies of forbidden zones and goal zones to prevent race conditions due to async pathfinding
        (Func<WPos, float> shapeDistance, DateTime activation)[] localForbiddenZones = [.. hints.ForbiddenZones];
        Func<WPos, float>[] localGoalZones = [.. hints.GoalZones];

        var imminent = ImminentExplosionTime(ws.CurrentTime);
        var len = localForbiddenZones.Length;
        var numImminentZones = len;
        int left = 0, right = len - 1;
        while (left <= right)
        {
            var mid = (left + right) / 2;
            ref var lfz = ref localForbiddenZones[mid];
            if (lfz.activation <= imminent)
                left = mid + 1;
            else
            {
                numImminentZones = mid;
                right = mid - 1;
            }
        }

        hints.PathfindMapBounds.PathfindMap(ctx.Map, hints.PathfindMapCenter);

        if (!_config.AllowAIToBeOutsideBounds && IsOutsideBounds(player.Position, ctx))
        {
            for (var i = 0; i < len; ++i)
            {
                ref var zf = ref localForbiddenZones[i];
                AddBlockerZone(ctx.Map, imminent, zf.activation, zf.shapeDistance, forbiddenZoneCushion);
            }
            return FindPathFromOutsideBounds(ctx, player.Position, playerSpeed);
        }

        // Check whether player is inside each forbidden zone
        var inZone = new bool[len];
        var inImminentForbiddenZone = false;
        var isInsideAnyZone = false;

        for (var i = 0; i < len; ++i)
        {
            var inside = localForbiddenZones[i].shapeDistance(player.Position) <= forbiddenZoneCushion - 0.1f;
            inZone[i] = inside;
            if (inside)
            {
                isInsideAnyZone = true;
                if (i < numImminentZones)
                    inImminentForbiddenZone = true;
            }
        }

        if (isInsideAnyZone)
        {
            // we're in forbidden zone => find path to safety (and ideally to uptime zone)
            // if such a path can't be found (that's always the case if we're inside imminent forbidden zone, but can also happen in other cases), try instead to find a path to safety that doesn't enter any other zones that we're not inside
            // first build a map with zones that we're outside of as blockers
            for (var i = 0; i < len; ++i)
            {
                if (!inZone[i])
                {
                    ref var zf = ref localForbiddenZones[i];
                    AddBlockerZone(ctx.Map, imminent, zf.activation, zf.shapeDistance, forbiddenZoneCushion);
                }
            }

            if (!inImminentForbiddenZone)
            {
                ctx.Map2.Init(ctx.Map, ctx.Map.Center);
                for (var i = 0; i < len; ++i)
                {
                    if (inZone[i])
                    {
                        ref var zf = ref localForbiddenZones[i];
                        AddBlockerZone(ctx.Map2, imminent, zf.activation, zf.shapeDistance, forbiddenZoneCushion);
                    }
                }
                var maxGoal = targetPos != null ? AddTargetGoal(ctx.Map2, targetPos.Value, targetRadius, targetRot, positional, 0) : 0;
                var res = FindPathFromUnsafe(ctx.ThetaStar, ctx.Map2, player.Position, 0, maxGoal, targetPos, targetRot, positional, playerSpeed);
                if (res != null)
                    return res.Value;

                // pathfind to any spot outside aoes we're in that doesn't enter new aoes
                for (var i = 0; i < len; ++i)
                {
                    if (inZone[i])
                    {
                        ctx.Map.AddGoal(localForbiddenZones[i].shapeDistance, forbiddenZoneCushion, 0, -1);
                    }
                }
                return FindPathFromImminent(ctx.ThetaStar, ctx.Map, player.Position, playerSpeed);
            }
            else
            {
                // try to find a path out of imminent aoes that we're in, while remaining in non-imminent aoes that we're already in - it might be worth it...
                for (var i = 0; i < len; ++i)
                {
                    if (inZone[i])
                    {
                        ctx.Map.AddGoal(localForbiddenZones[i].shapeDistance, forbiddenZoneCushion, 0, -1);
                    }
                }
                return FindPathFromImminent(ctx.ThetaStar, ctx.Map, player.Position, playerSpeed);
            }
        }
        if (player.CastInfo != null)
        {
            return new() { Destination = null, LeewaySeconds = 0, TimeToGoal = 0, Map = ctx.Map, DecisionType = Decision.Casting };
        }
        // we're safe, see if we can improve our position
        if (targetPos != null)
        {
            if (!player.Position.InCircle(targetPos.Value, targetRadius))
            {
                // we're not in uptime zone, just run to it, avoiding any aoes
                for (var i = 0; i < len; ++i)
                {
                    ref var zf = ref localForbiddenZones[i];
                    AddBlockerZone(ctx.Map, imminent, zf.activation, zf.shapeDistance, forbiddenZoneCushion);
                }
                var maxGoal = AddTargetGoal(ctx.Map, targetPos.Value, targetRadius, targetRot, Positional.Any, 0);
                if (maxGoal != 0)
                {
                    // try to find a path to target
                    ctx.ThetaStar.Start(ctx.Map, maxGoal, player.Position, 1.0f / playerSpeed);
                    var res = ctx.ThetaStar.Execute();
                    if (res >= 0)
                        return new() { Destination = GetFirstWaypoint(ctx.ThetaStar, res), LeewaySeconds = float.MaxValue, TimeToGoal = ctx.ThetaStar.NodeByIndex(res).GScore, Map = ctx.Map, MapGoal = maxGoal, DecisionType = Decision.SafeToUptime };
                }

                // goal is not reachable, but we can try getting as close to the target as we can until first aoe
                var start = ctx.Map.ClampToGrid(ctx.Map.WorldToGrid(player.Position));
                var end = ctx.Map.ClampToGrid(ctx.Map.WorldToGrid(targetPos.Value));
                var best = start;
                foreach (var (x, y) in ctx.Map.EnumeratePixelsInLine(start.x, start.y, end.x, end.y))
                {
                    if (ctx.Map[x, y].MaxG != float.MaxValue)
                        break;
                    best = (x, y);
                }
                if (best != start)
                {
                    var dest = ctx.Map.GridToWorld(best.x, best.y, 0.5f, 0.5f);
                    return new() { Destination = dest, LeewaySeconds = float.MaxValue, TimeToGoal = (dest - player.Position).Length() / playerSpeed, Map = ctx.Map, MapGoal = maxGoal, DecisionType = Decision.SafeToCloser };
                }

                return new() { Destination = null, LeewaySeconds = float.MaxValue, TimeToGoal = 0, Map = ctx.Map, MapGoal = maxGoal, DecisionType = Decision.SafeBlocked };
            }

            var playerOrientationToTarget = (player.Position - targetPos.Value).Normalized().Dot(targetRot.ToDirection());

            var inPositional = positional switch
            {
                Positional.Rear => playerOrientationToTarget < -0.7071068f,
                Positional.Flank => MathF.Abs(playerOrientationToTarget) < 0.7071068f,
                Positional.Front => playerOrientationToTarget > 0.999f, // ~2.5 degrees - assuming max position error of 0.1, this requires us to stay at least at R=~2.25
                _ => true
            };
            if (!inPositional)
            {
                // we're in uptime zone, but not in correct quadrant - move there, avoiding all aoes and staying within uptime zone
                ctx.Map.BlockPixelsInside(ShapeDistance.InvertedCircle(targetPos.Value, targetRadius), 0, 0);
                for (var i = 0; i < len; ++i)
                {
                    ref var zf = ref localForbiddenZones[i];
                    AddBlockerZone(ctx.Map, imminent, zf.activation, zf.shapeDistance, forbiddenZoneCushion);
                }
                var maxGoal = AddPositionalGoal(ctx.Map, targetPos.Value, targetRadius, targetRot, positional, 0);
                if (maxGoal > 0)
                {
                    // try to find a path to quadrant
                    ctx.ThetaStar.Start(ctx.Map, maxGoal, player.Position, 1.0f / playerSpeed);
                    var res = ctx.ThetaStar.Execute();
                    if (res >= 0)
                    {
                        var dest = IncreaseDestinationPrecision(GetFirstWaypoint(ctx.ThetaStar, res), targetPos, targetRot, positional);
                        return new() { Destination = dest, LeewaySeconds = float.MaxValue, TimeToGoal = ctx.ThetaStar.NodeByIndex(res).GScore, Map = ctx.Map, MapGoal = maxGoal, DecisionType = Decision.UptimeToPositional };
                    }
                }

                // fail
                return new() { Destination = null, LeewaySeconds = float.MaxValue, TimeToGoal = 0, Map = ctx.Map, MapGoal = maxGoal, DecisionType = Decision.UptimeBlocked };
            }
        }

        return new() { Destination = null, LeewaySeconds = float.MaxValue, TimeToGoal = 0, DecisionType = Decision.Optimal };
    }

    public static DateTime ImminentExplosionTime(DateTime currentTime) => currentTime.AddSeconds(1);

    public static void AddBlockerZone(Map map, DateTime imminent, DateTime activation, Func<WPos, float> shape, float threshold) => map.BlockPixelsInside(shape, MathF.Max(0, (float)(activation - imminent).TotalSeconds), threshold);

    public static int AddTargetGoal(Map map, WPos targetPos, float targetRadius, Angle targetRot, Positional positional, int minPriority)
    {
        var adjPrio = map.AddGoal(ShapeDistance.Circle(targetPos, targetRadius), 0, minPriority, 1);
        if (adjPrio == minPriority)
            return minPriority;
        return AddPositionalGoal(map, targetPos, targetRadius, targetRot, positional, minPriority + 1);
    }

    public static Func<WPos, float> ShapeDistanceFlank(WPos targetPos, Angle targetRot)
    {
        var n1 = (targetRot + 45.Degrees()).ToDirection();
        var n2 = n1.OrthoL();
        return p =>
        {
            var off = p - targetPos;
            var d1 = n1.Dot(off);
            var d2 = n2.Dot(off);
            var dr = Math.Max(d1, d2);
            var dl = Math.Max(-d1, -d2);
            return Math.Min(dr, dl);
        };
    }

    public static Func<WPos, float> ShapeDistanceRear(WPos targetPos, Angle targetRot)
    {
        var n1 = (targetRot - 45.Degrees()).ToDirection();
        var n2 = n1.OrthoL();
        return p =>
        {
            var off = p - targetPos;
            var d1 = n1.Dot(off);
            var d2 = n2.Dot(off);
            return Math.Max(d1, d2);
        };
    }

    public static int AddPositionalGoal(Map map, WPos targetPos, float targetRadius, Angle targetRot, Positional positional, int minPriority)
    {
        var adjPrio = minPriority;
        switch (positional)
        {
            case Positional.Flank:
                adjPrio = map.AddGoal(ShapeDistanceFlank(targetPos, targetRot), 0, minPriority, 1);
                break;
            case Positional.Rear:
                adjPrio = map.AddGoal(ShapeDistanceRear(targetPos, targetRot), 0, minPriority, 1);
                break;
            case Positional.Front:
                Func<int, int, bool> suitable = (x, y) =>
                {
                    if (!map.InBounds(x, y))
                        return false;
                    var pixel = map[x, y];
                    return pixel.Priority >= minPriority && pixel.MaxG == float.MaxValue;
                };

                var dir = targetRot.ToDirection();
                var maxRange = map.WorldToGrid(targetPos + dir * targetRadius);
                if (suitable(maxRange.x, maxRange.y))
                {
                    adjPrio = map.AddGoal(maxRange.x, maxRange.y, 1);
                }
                else if (targetRadius > 3)
                {
                    var minRange = map.WorldToGrid(targetPos + dir * 3);
                    if (minRange != maxRange)
                    {
                        foreach (var p in map.EnumeratePixelsInLine(maxRange.x, maxRange.y, minRange.x, minRange.y))
                        {
                            if (suitable(p.x, p.y))
                            {
                                adjPrio = map.AddGoal(p.x, p.y, 1);
                                break;
                            }
                        }
                    }
                }
                break;
        }
        return adjPrio;
    }

    public static NavigationDecision FindPathFromOutsideBounds(Context ctx, WPos startPos, float speed = 6)
    {
        WPos? closest = null;
        var closestDistance = float.MaxValue;
        foreach (var p in ctx.Map.EnumeratePixels())
        {
            if (ctx.Map[p.x, p.y].MaxG > 0) // assume any pixel not marked as blocked is better than being outside of bounds
            {
                var distance = (p.center - startPos).LengthSq();
                if (distance < closestDistance)
                {
                    closest = p.center;
                    closestDistance = distance;
                }
            }
        }
        return new() { Destination = closest, LeewaySeconds = 0, TimeToGoal = MathF.Sqrt(closestDistance) / speed, Map = ctx.Map, DecisionType = Decision.BackIntoBounds };
    }

    public static NavigationDecision? FindPathFromUnsafe(ThetaStar pathfind, Map map, WPos startPos, int safeGoal, int maxGoal, WPos? targetPos, Angle targetRot, Positional positional, float speed = 6)
    {
        if (maxGoal - safeGoal == 2)
        {
            // try finding path to flanking position
            pathfind.Start(map, maxGoal, startPos, 1.0f / speed);
            var res = pathfind.Execute();
            if (res >= 0)
            {
                var dest = IncreaseDestinationPrecision(GetFirstWaypoint(pathfind, res), targetPos, targetRot, positional);
                return new() { Destination = dest, LeewaySeconds = pathfind.NodeByIndex(res).PathLeeway, TimeToGoal = pathfind.NodeByIndex(res).GScore, Map = map, MapGoal = maxGoal, DecisionType = Decision.UnsafeToPositional };
            }
            --maxGoal;
        }

        if (maxGoal - safeGoal == 1)
        {
            // try finding path to uptime position
            pathfind.Start(map, maxGoal, startPos, 1.0f / speed);
            var res = pathfind.Execute();
            if (res >= 0)
                return new() { Destination = GetFirstWaypoint(pathfind, res), LeewaySeconds = pathfind.NodeByIndex(res).PathLeeway, TimeToGoal = pathfind.NodeByIndex(res).GScore, Map = map, MapGoal = maxGoal, DecisionType = Decision.UnsafeToUptime };
            --maxGoal;
        }

        if (maxGoal - safeGoal == 0)
        {
            // try finding path to any safe spot
            pathfind.Start(map, maxGoal, startPos, 1.0f / speed);
            var res = pathfind.Execute();
            if (res >= 0)
                return new() { Destination = GetFirstWaypoint(pathfind, res), LeewaySeconds = pathfind.NodeByIndex(res).PathLeeway, TimeToGoal = pathfind.NodeByIndex(res).GScore, Map = map, MapGoal = maxGoal, DecisionType = Decision.UnsafeToSafe };
        }

        return null;
    }

    public static NavigationDecision FindPathFromImminent(ThetaStar pathfind, Map map, WPos startPos, float speed = 6)
    {
        // Attempt to find a path to any safe goal
        pathfind.Start(map, 0, startPos, 1.0f / speed);
        var res = pathfind.Execute();
        if (res >= 0)
        {
            return new()
            {
                Destination = GetFirstWaypoint(pathfind, res),
                LeewaySeconds = 0,
                TimeToGoal = pathfind.NodeByIndex(res).GScore,
                Map = map,
                MapGoal = 0,
                DecisionType = Decision.ImminentToSafe
            };
        }

        // If no path was found, attempt to find the closest reachable less dangerous pixel, pretend runspeed is over 100x higher than normal
        pathfind.Start(map, 0, startPos, 0.001f);
        var res2 = pathfind.Execute();
        if (res2 >= 0)
        {
            return new()
            {
                Destination = GetFirstWaypoint(pathfind, res2),
                LeewaySeconds = 0,
                TimeToGoal = pathfind.NodeByIndex(res2).GScore,
                Map = map,
                MapGoal = 0,
                DecisionType = Decision.ImminentToClosest
            };
        }

        // No reachable safe pixel found; we just eat the AOE instead of risking to run into an even more dangerous pixel (eg holes in the arena)
        return new()
        {
            Destination = null,
            LeewaySeconds = 0,
            TimeToGoal = 0,
            Map = map,
            DecisionType = Decision.None
        };
    }

    public static WPos? GetFirstWaypoint(ThetaStar pf, int cell)
    {
        do
        {
            ref var node = ref pf.NodeByIndex(cell);
            var parent = pf.CellIndex(node.ParentX, node.ParentY);
            if (pf.NodeByIndex(parent).GScore == 0)
                return pf.CellCenter(cell);
            cell = parent;
        }
        while (true);
    }

    public static WPos? IncreaseDestinationPrecision(WPos? dest, WPos? targetPos, Angle targetRot, Positional positional)
    {
        if (dest == null || targetPos == null || positional != Positional.Front)
            return dest;
        var dir = targetRot.ToDirection();
        var adjDest = targetPos.Value + dir * dir.Dot(dest.Value - targetPos.Value);
        return (dest.Value - adjDest).LengthSq() < 1 ? adjDest : dest;
    }

    public static bool IsOutsideBounds(WPos position, Context ctx)
    {
        var map = ctx.Map;
        var (x, y) = map.WorldToGrid(position);
        if (x < 0 || x >= map.Width || y < 0 || y >= map.Height)
            return true; // outside current pathfinding map
        return map.Pixels[y * map.Width + x].MaxG == -1; // inside pathfinding map, but outside actual walkable bounds
    }
}
