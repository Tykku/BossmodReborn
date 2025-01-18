namespace BossMod.Dawntrail.Alliance.A14ShadowLord;

class DarkNebula(BossModule module) : Components.Knockback(module)
{
    private const int Length = 4;
    private const float HalfWidth = 1.75f;

    public readonly List<Actor> Casters = new(4);

    private static readonly Angle a90 = 90.Degrees();
    private static readonly List<(Predicate<WPos> Matcher, int[] CircleIndices, WDir Directions)> PositionMatchers =
        [
        (pos => pos == new WPos(142, 792), [3, 1], 45.Degrees().ToDirection()),  // 135°
        (pos => pos == new WPos(158, 792), [0, 3], -135.Degrees().ToDirection()),  // 45°
        (pos => pos == new WPos(158, 808), [2, 0], -45.Degrees().ToDirection()),  // -45°
        (pos => pos.AlmostEqual(new WPos(142, 808), 1), [1, 2], 135.Degrees().ToDirection())  // -135°
    ];

    public override IEnumerable<Source> Sources(int slot, Actor actor)
    {
        var count = Casters.Count;
        if (count == 0)
            return [];
        var max = count > 2 ? 2 : count;
        List<Source> sources = new(max);
        for (var i = 0; i < max; ++i)
        {
            var caster = Casters[i];
            var dir = caster.CastInfo?.Rotation ?? caster.Rotation;
            var kind = dir.ToDirection().OrthoL().Dot(actor.Position - caster.Position) > 0 ? Kind.DirLeft : Kind.DirRight;
            sources.Add(new(caster.Position, 20, Module.CastFinishAt(caster.CastInfo), null, dir, kind));
        }
        return sources;
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.DarkNebulaShort or AID.DarkNebulaLong)
            Casters.Add(caster);
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.DarkNebulaShort or AID.DarkNebulaLong)
        {
            ++NumCasts;
            Casters.Remove(caster);
        }
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (Casters.Count == 0)
            return;

        var forbidden = new List<Func<WPos, float>>(2);
        var caster0 = Casters[0];
        static Func<WPos, float> CreateForbiddenZone(int circleIndex, WDir dir)
         => ShapeDistance.InvertedRect(A14ShadowLord.Circles[circleIndex].Center, dir, Length, 0, HalfWidth);

        var mapping = PositionMatchers.FirstOrDefault(m => m.Matcher(caster0.Position));

        if (Casters.Count == 1)
        {
            for (var i = 0; i < 2; ++i)
                forbidden.Add(CreateForbiddenZone(mapping.CircleIndices[i], mapping.Directions));
        }
        else
        {
            var caster1 = Casters[1];
            var rotationMatch = caster0.Rotation.AlmostEqual(caster1.Rotation + a90, Angle.DegToRad);
            var circleIndex = rotationMatch ? mapping.CircleIndices[0] : mapping.CircleIndices[1];
            forbidden.Add(CreateForbiddenZone(circleIndex, mapping.Directions));
        }
        float maxDistanceFunc(WPos pos)
        {
            var minDistance = float.MinValue;
            for (var i = 0; i < forbidden.Count; ++i)
            {
                var distance = forbidden[i](pos);
                if (distance > minDistance)
                    minDistance = distance;
            }
            return minDistance;
        }

        hints.AddForbiddenZone(maxDistanceFunc, Sources(slot, actor).FirstOrDefault().Activation);
    }
}
