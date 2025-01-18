namespace BossMod.Endwalker.Alliance.A24Menphina;

class MidnightFrostWaxingClaw(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];

    private static readonly AOEShapeCone _shape = new(60, 90.Degrees());
    private static readonly HashSet<AID> casts = [AID.MidnightFrostShortNormalFrontAOE, AID.MidnightFrostShortNormalBackAOE,
            AID.MidnightFrostShortMountedFrontAOE, AID.MidnightFrostShortMountedBackAOE,
            AID.MidnightFrostLongMountedFrontAOE, AID.MidnightFrostLongMountedBackAOE,
            AID.MidnightFrostLongDismountedFrontAOE, AID.MidnightFrostLongDismountedBackAOE,
            AID.WaxingClawRight, AID.WaxingClawLeft];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (casts.Contains((AID)spell.Action.ID))
        {
            _aoes.Add(new(_shape, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell)));
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (casts.Contains((AID)spell.Action.ID))
        {
            ++NumCasts;
            _aoes.Clear();
        }
    }
}
