namespace BossMod.Endwalker.VariantCriterion.V02MR.V023Gorai;

class Thundercall(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];
    private int counter;
    private static readonly AOEShapeCircle circleSmall = new(8);
    private static readonly AOEShapeCircle circleBig = new(18);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;

    public override void OnActorCreated(Actor actor)
    {
        if (counter < 2 && (OID)actor.OID == OID.BallOfLevin)
        {
            _aoes.Add(new(circleBig, actor.Position, default, WorldState.FutureTime(10.8f)));
            ++counter;
        }
    }

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if ((OID)actor.OID == OID.BallOfLevin && (SID)status.ID == SID.SmallOrb)
        {
            var activation = WorldState.FutureTime(10);
            _aoes.Add(new(circleSmall, actor.Position, default, activation));
            foreach (var a in Module.Enemies(OID.BallOfLevin).Except([actor]))
                _aoes.Add(new(circleBig, a.Position, default, activation));
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.ShockSmall or AID.ShockLarge)
            _aoes.Clear();
    }
}
