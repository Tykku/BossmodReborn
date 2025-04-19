namespace BossMod.Dawntrail.Extreme.Ex4Zelenia;

class PowerBreak(BossModule module) : Components.GenericAOEs(module)
{
    private AOEInstance? _aoe;
    private static readonly AOEShapeRect rect = new(24f, 32f);

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(ref _aoe);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.PowerBreak1 or (uint)AID.PowerBreak2)
            _aoe = new(rect, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.PowerBreak1 or (uint)AID.PowerBreak2)
        {
            ++NumCasts;
            _aoe = null;
        }
    }
}
