namespace BossMod.Dawntrail.Raid.M02NHoneyBLovely;

class Sweethearts(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCircle circle = new(1);
    private readonly List<Actor> _hearts = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _hearts.Select(a => new AOEInstance(circle, a.Position));

    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if ((OID)actor.OID == OID.Sweetheart && id == 0x11D3)
            _hearts.Add(actor);
    }

    public override void OnActorDestroyed(Actor actor)
    {
        if ((OID)actor.OID == OID.Sweetheart)
            _hearts.Remove(actor);
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.SweetheartTouch)
            _hearts.Remove(caster);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        base.AddAIHints(slot, actor, assignment, hints);
        foreach (var w in _hearts)
            hints.AddForbiddenZone(new AOEShapeCircle(1), w.Position + w.Rotation.ToDirection());
    }
}
