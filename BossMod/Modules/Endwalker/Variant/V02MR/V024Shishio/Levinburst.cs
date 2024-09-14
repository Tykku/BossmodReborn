namespace BossMod.Endwalker.VariantCriterion.V02MR.V024Shishio;

class Levinburst(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect rect = new(5, 20, 5);
    private readonly List<AOEInstance> _aoes = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;
    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.Rairin)
            _aoes.Add(new(rect, actor.Position, actor.Rotation, WorldState.FutureTime(6.9f)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.Levinburst)
            _aoes.Clear();
    }
}
