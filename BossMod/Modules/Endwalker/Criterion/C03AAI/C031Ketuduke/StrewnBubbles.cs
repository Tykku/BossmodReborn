namespace BossMod.Endwalker.VariantCriterion.C03AAI.C031Ketuduke;

class StrewnBubbles(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];

    private static readonly AOEShapeRect _shape = new(20, 5);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes.Take(4);

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.BubbleStrewer)
        {
            _aoes.Add(new(_shape, actor.Position, actor.Rotation, WorldState.FutureTime(10.7f)));
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.NSphereShatter or AID.SSphereShatter)
        {
            var count = _aoes.RemoveAll(aoe => aoe.Origin.AlmostEqual(caster.Position, 1));
            if (count != 1)
                ReportError($"{spell.Action} removed {count} aoes");
            ++NumCasts;
        }
    }
}

class RecedingEncroachingTwintides(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];
    private static readonly HashSet<AID> castEnd = [AID.NRecedingTwintides, AID.NEncroachingTwintides, AID.NFarTide, AID.NNearTide,
    AID.SRecedingTwintides, AID.SEncroachingTwintides, AID.SFarTide, AID.SNearTide];
    private static readonly AOEShapeCircle _shapeOut = new(14);
    private static readonly AOEShapeDonut _shapeIn = new(8, 60);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes.Skip(NumCasts).Take(1);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.NRecedingTwintides:
            case AID.SRecedingTwintides:
                _aoes.Add(new(_shapeOut, spell.LocXZ, default, Module.CastFinishAt(spell)));
                _aoes.Add(new(_shapeIn, spell.LocXZ, default, Module.CastFinishAt(spell, 3.1f)));
                break;
            case AID.NEncroachingTwintides:
            case AID.SEncroachingTwintides:
                _aoes.Add(new(_shapeIn, spell.LocXZ, default, Module.CastFinishAt(spell)));
                _aoes.Add(new(_shapeOut, spell.LocXZ, default, Module.CastFinishAt(spell, 3.1f)));
                break;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (castEnd.Contains((AID)spell.Action.ID))
            ++NumCasts;
    }
}
