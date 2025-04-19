﻿using static BossMod.Components.GenericKnockback;

namespace BossMod.Components;

// generic knockback/attract component; it's a cast counter for convenience
public abstract class GenericKnockback(BossModule module, uint aid = default, bool ignoreImmunes = false, int maxCasts = int.MaxValue, bool stopAtWall = false, bool stopAfterWall = false, List<SafeWall>? safeWalls = null) : CastCounter(module, aid)
{
    public enum Kind
    {
        None,
        AwayFromOrigin, // standard knockback - specific distance along ray from origin to target
        TowardsOrigin, // standard pull - "knockback" to source -  specific distance along ray from origin to target + 180 degrees
        DirBackward, // standard pull - "knockback" to source - forward along source's direction + 180 degrees
        DirForward, // directional knockback - forward along source's direction
        DirLeft, // directional knockback - forward along source's direction + 90 degrees
        DirRight, // directional knockback - forward along source's direction - 90 degrees
    }

    public record struct Knockback(
        WPos Origin,
        float Distance,
        DateTime Activation = default,
        AOEShape? Shape = null, // if null, assume it is unavoidable raidwide knockback/attract
        Angle Direction = default, // irrelevant for non-directional knockback/attract
        Kind Kind = Kind.AwayFromOrigin,
        float MinDistance = 0, // irrelevant for knockbacks
        IReadOnlyList<SafeWall>? SafeWalls = null
    );

    public readonly record struct SafeWall(WPos Vertex1, WPos Vertex2);

    protected struct PlayerImmuneState
    {
        public DateTime RoleBuffExpire; // 0 if not active
        public DateTime JobBuffExpire; // 0 if not active
        public DateTime DutyBuffExpire; // 0 if not active

        public readonly bool ImmuneAt(DateTime time) => RoleBuffExpire > time || JobBuffExpire > time || DutyBuffExpire > time;
    }

    public readonly bool IgnoreImmunes = ignoreImmunes;
    public readonly bool StopAtWall = stopAtWall; // use if wall is solid rather than deadly
    public readonly bool StopAfterWall = stopAfterWall; // use if the wall is a polygon where you need to check for intersections
    public readonly int MaxCasts = maxCasts; // use to limit number of drawn knockbacks
    private const float approxHitBoxRadius = 0.499f; // calculated because due to floating point errors this does not result in 0.001
    private const float maxIntersectionError = 0.5f - approxHitBoxRadius; // calculated because due to floating point errors this does not result in 0.001
    public readonly List<SafeWall> SafeWalls = safeWalls ?? [];

    protected readonly PlayerImmuneState[] PlayerImmunes = new PlayerImmuneState[PartyState.MaxAllies];

    public bool IsImmune(int slot, DateTime time) => !IgnoreImmunes && PlayerImmunes[slot].ImmuneAt(time);

    public static WPos AwayFromSource(WPos pos, WPos origin, float distance) => pos != origin ? pos + distance * (pos - origin).Normalized() : pos;
    public static WPos AwayFromSource(WPos pos, Actor? source, float distance) => source != null ? AwayFromSource(pos, source.Position, distance) : pos;

    public static void DrawKnockback(WPos from, WPos to, Angle rot, MiniArena arena)
    {
        if (from != to)
        {
            arena.ActorProjected(from, to, rot, Colors.Danger);
            arena.AddLine(from, to);
        }
    }
    public static void DrawKnockback(Actor actor, WPos adjPos, MiniArena arena) => DrawKnockback(actor.Position, adjPos, actor.Rotation, arena);

    // note: if implementation returns multiple sources, it is assumed they are applied sequentially (so they should be pre-sorted in activation order)
    public abstract ReadOnlySpan<Knockback> ActiveKnockbacks(int slot, Actor actor);

    // called to determine whether we need to show hint
    public virtual bool DestinationUnsafe(int slot, Actor actor, WPos pos) => !StopAtWall && !Module.InBounds(pos);

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        var movements = CalculateMovements(slot, actor);
        var count = movements.Count;
        for (var i = 0; i < count; ++i)
        {
            var movement = movements[i];
            if (DestinationUnsafe(slot, actor, movement.to))
            {
                hints.Add("About to be knocked into danger!");
                break;
            }
        }
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        foreach (var e in CalculateMovements(pcSlot, pc))
            DrawKnockback(e.from, e.to, pc.Rotation, Arena);
    }

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        var slot = Raid.FindSlot(actor.InstanceID);
        if (slot >= 0)
            switch (status.ID)
            {
                case 3054: //Guard in PVP
                case (uint)WHM.SID.Surecast:
                case (uint)WAR.SID.ArmsLength:
                    PlayerImmunes[slot].RoleBuffExpire = status.ExpireAt;
                    break;
                case 1722: //Bluemage Diamondback
                case (uint)WAR.SID.InnerStrength:
                    PlayerImmunes[slot].JobBuffExpire = status.ExpireAt;
                    break;
                case 2345: //Lost Manawall in Bozja
                    PlayerImmunes[slot].DutyBuffExpire = status.ExpireAt;
                    break;
            }
    }

    public override void OnStatusLose(Actor actor, ActorStatus status)
    {
        var slot = Raid.FindSlot(actor.InstanceID);
        if (slot >= 0)
            switch (status.ID)
            {
                case 3054: //Guard in PVP
                case (uint)WHM.SID.Surecast:
                case (uint)WAR.SID.ArmsLength:
                    PlayerImmunes[slot].RoleBuffExpire = new();
                    break;
                case 1722: //Bluemage Diamondback
                case (uint)WAR.SID.InnerStrength:
                    PlayerImmunes[slot].JobBuffExpire = new();
                    break;
                case 2345: //Lost Manawall in Bozja
                    PlayerImmunes[slot].DutyBuffExpire = new();
                    break;
            }
    }

    public List<(WPos from, WPos to)> CalculateMovements(int slot, Actor actor)
    {
        if (MaxCasts <= 0)
            return [];
        var movements = new List<(WPos, WPos)>();
        var from = actor.Position;
        var count = 0;
        foreach (var s in ActiveKnockbacks(slot, actor))
        {
            if (IsImmune(slot, s.Activation))
                continue; // this source won't affect player due to immunity
            if (s.Shape != null && !s.Shape.Check(from, s.Origin, s.Direction))
                continue; // this source won't affect player due to being out of aoe

            var dir = s.Kind switch
            {
                Kind.AwayFromOrigin => from != s.Origin ? (from - s.Origin).Normalized() : default,
                Kind.TowardsOrigin => from != s.Origin ? (s.Origin - from).Normalized() : default,
                Kind.DirBackward => (s.Direction + 180f.Degrees()).ToDirection(),
                Kind.DirForward => s.Direction.ToDirection(),
                Kind.DirLeft => s.Direction.ToDirection().OrthoL(),
                Kind.DirRight => s.Direction.ToDirection().OrthoR(),
                _ => default
            };
            if (dir == default)
                continue; // couldn't determine direction for some reason

            var distance = s.Distance;
            if (s.Kind == Kind.TowardsOrigin)
                distance = Math.Min(s.Distance, (s.Origin - from).Length() - s.MinDistance);
            if (s.Kind == Kind.DirBackward)
            {
                var perpendicularDir = s.Direction.ToDirection().OrthoL();
                var perpendicularDistance = Math.Abs((from - s.Origin).Cross(perpendicularDir) / perpendicularDir.Length());
                distance = Math.Min(s.Distance, perpendicularDistance);
            }

            if (distance <= 0)
                continue; // this could happen if attract starts from < min distance

            if (StopAtWall)
                distance = Math.Min(distance, Arena.IntersectRayBounds(from, dir) - Math.Clamp(actor.HitboxRadius - approxHitBoxRadius, maxIntersectionError, actor.HitboxRadius - approxHitBoxRadius)); // hitbox radius can be != 0.5 if player is transformed/mounted, but normal arenas with walls should account for walkable arena in their shape already
            if (StopAfterWall)
                distance = Math.Min(distance, Arena.IntersectRayBounds(from, dir) + maxIntersectionError);

            var walls = s.SafeWalls ?? SafeWalls;
            var countW = walls.Count;
            if (countW != 0)
            {
                var distanceToWall = float.MaxValue;
                for (var i = 0; i < countW; ++i)
                {
                    var wall = walls[i];
                    var t = Intersect.RaySegment(from, dir, wall.Vertex1, wall.Vertex2);
                    if (t < distanceToWall && t <= s.Distance)
                        distanceToWall = t;
                }
                var hitboxradius = actor.HitboxRadius < approxHitBoxRadius ? 0.5f : actor.HitboxRadius; // some NPCs have less than 0.5 radius and cause error while clamping
                distance = distanceToWall < float.MaxValue
                    ? Math.Min(distance, distanceToWall - Math.Clamp(hitboxradius - approxHitBoxRadius, maxIntersectionError, hitboxradius - approxHitBoxRadius))
                    : Math.Min(distance, Arena.IntersectRayBounds(from, dir) + maxIntersectionError);
            }

            var to = from + distance * dir;
            movements.Add((from, to));
            from = to;

            if (++count == MaxCasts)
                break;
        }
        return movements;
    }
}

// generic 'knockback from/attract to cast target' component
// TODO: knockback is really applied when effectresult arrives rather than when actioneffect arrives, this is important for ai hints (they can reposition too early otherwise)
public class SimpleKnockbacks(BossModule module, uint aid, float distance, bool ignoreImmunes = false, int maxCasts = int.MaxValue, AOEShape? shape = null, Kind kind = Kind.AwayFromOrigin, float minDistance = 0, bool minDistanceBetweenHitboxes = false, bool stopAtWall = false, bool stopAfterWall = false, List<SafeWall>? safeWalls = null)
    : GenericKnockback(module, aid, ignoreImmunes, maxCasts, stopAtWall, stopAfterWall, safeWalls)
{
    public readonly float Distance = distance;
    public readonly AOEShape? Shape = shape;
    public readonly Kind KnockbackKind = kind;
    public readonly float MinDistance = minDistance;
    public readonly bool MinDistanceBetweenHitboxes = minDistanceBetweenHitboxes;
    public readonly List<Actor> Casters = [];

    public override ReadOnlySpan<Knockback> ActiveKnockbacks(int slot, Actor actor)
    {
        var count = Casters.Count;
        if (count == 0)
            return [];
        var knockbacks = new Knockback[count];
        for (var i = 0; i < count; ++i)
        {
            var c = Casters[i];
            // note that majority of knockback casts are self-targeted
            var minDist = MinDistance + (MinDistanceBetweenHitboxes ? actor.HitboxRadius + c.HitboxRadius : 0);
            if (c.CastInfo!.TargetID == c.InstanceID)
            {
                knockbacks[i] = new(c.CastInfo.LocXZ, Distance, Module.CastFinishAt(c.CastInfo), Shape, c.CastInfo.Rotation, KnockbackKind, minDist);
            }
            else
            {
                var origin = c.CastInfo.LocXZ;
                knockbacks[i] = new(origin, Distance, Module.CastFinishAt(c.CastInfo), Shape, Angle.FromDirection(origin - c.Position), KnockbackKind, minDist);
            }
        }
        return knockbacks;
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == WatchedAction)
            Casters.Add(caster);
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == WatchedAction)
            Casters.Remove(caster);
    }
}
