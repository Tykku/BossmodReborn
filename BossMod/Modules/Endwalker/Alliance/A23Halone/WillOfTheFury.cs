namespace BossMod.Endwalker.Alliance.A23Halone;

class WillOfTheFury(BossModule module) : Components.GenericAOEs(module)
{
    private AOEInstance? _aoe;
    private const float _impactRadiusIncrement = 6;
    public bool Active => _aoe != null;
    private static readonly HashSet<AID> castEnd = [AID.WillOfTheFuryAOE1, AID.WillOfTheFuryAOE2, AID.WillOfTheFuryAOE3, AID.WillOfTheFuryAOE4, AID.WillOfTheFuryAOE5];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.WillOfTheFuryAOE1)
        {
            UpdateAOE(Module.CastFinishAt(spell));
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (castEnd.Contains((AID)spell.Action.ID))
        {
            ++NumCasts;
            UpdateAOE(WorldState.FutureTime(2));
        }
    }

    private void UpdateAOE(DateTime activation)
    {
        var outerRadius = (5 - NumCasts) * _impactRadiusIncrement;
        AOEShape? shape = NumCasts switch
        {
            < 4 => new AOEShapeDonut(outerRadius - _impactRadiusIncrement, outerRadius),
            4 => new AOEShapeCircle(outerRadius),
            _ => null
        };
        _aoe = shape != null ? new(shape, Module.Center, default, activation) : null;
    }
}
