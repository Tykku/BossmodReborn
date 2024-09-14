namespace BossMod.RealmReborn.Trial.T09WhorleaterH;

class SpinningDive(BossModule module) : Components.GenericAOEs(module) //TODO: Find out how to detect spinning dives earlier eg. the water column telegraph
{
    private AOEInstance? _aoe;
    public static readonly AOEShapeRect Rect = new(50.5f, 8);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.SpinningDiveHelper)
            _aoe = new(Rect, actor.Position, actor.Rotation, WorldState.FutureTime(0.6f));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.SpinningDiveSnapshot)
            _aoe = null;
    }
}

class SpinningDiveKB(BossModule module) : Components.Knockback(module, stopAtWall: true) //TODO: Find out how to detect spinning dives earlier eg. the water column telegraph
{
    private Source? _knockback;

    public override IEnumerable<Source> Sources(int slot, Actor actor) => Utils.ZeroOrOne(_knockback);

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.SpinningDiveHelper)
            _knockback = new(actor.Position, 10, WorldState.FutureTime(1.4f), SpinningDive.Rect, actor.Rotation);
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.SpinningDiveEffect)
            _knockback = null;
    }

    public override bool DestinationUnsafe(int slot, Actor actor, WPos pos) => (Module.FindComponent<Hydroshot>()?.ActiveAOEs(slot, actor).Any(z => z.Shape.Check(pos, z.Origin, z.Rotation)) ?? false) || (Module.FindComponent<Dreadstorm>()?.ActiveAOEs(slot, actor).Any(z => z.Shape.Check(pos, z.Origin, z.Rotation)) ?? false);
}
