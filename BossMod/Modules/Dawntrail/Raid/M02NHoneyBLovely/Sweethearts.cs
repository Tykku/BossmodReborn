namespace BossMod.Dawntrail.Raid.M02NHoneyBLovely;

class Sweethearts(BossModule module) : Components.GenericAOEs(module)
{
    private readonly IReadOnlyList<Actor> _hearts = module.Enemies(OID.Sweetheart);
    private IEnumerable<Actor> Hearts => _hearts;
    private static readonly AOEShapeCircle circle = new(1);
    private readonly List<Actor> _aoes = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        foreach (var o in _aoes)
            yield return new(circle, o.Position, o.Rotation);
    }

    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if (id == 0x11D3 && Hearts.Contains(actor))
            _aoes.Add(actor);
    }

    public override void OnActorDestroyed(Actor actor)
    {
        if ((OID)actor.OID == OID.Sweetheart)
            _aoes.Remove(actor);
    }

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if ((SID)status.ID == SID.VulnerabilityUp) // hearts do not seem to get destroyed if they hit a player, so we use this fallback
            _aoes.Remove(Hearts.FirstOrDefault(x => x.InstanceID == status.SourceID)!);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        base.AddAIHints(slot, actor, assignment, hints);
        foreach (var w in ActiveAOEs(slot, actor))
            hints.AddForbiddenZone(new AOEShapeCircle(1), w.Origin + w.Rotation.ToDirection());
    }
}
