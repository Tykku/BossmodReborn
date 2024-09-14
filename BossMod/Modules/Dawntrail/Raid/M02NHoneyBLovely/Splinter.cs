namespace BossMod.Dawntrail.Raid.M02NHoneyBLovely;

class Splinter(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCircle circle = new(8);
    private readonly List<AOEInstance> _aoes = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.Splinter)
            _aoes.Clear();
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.SplinterVisual1)
            _aoes.Add(new(circle, spell.TargetXZ, default, WorldState.FutureTime(5)));
    }
}
