namespace BossMod.Dawntrail.Extreme.Ex1Valigarmanda;

class HailOfFeathers(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];

    private static readonly AOEShapeCircle _shape = new(20); // TODO: verify falloff
    private static readonly HashSet<AID> casts = [AID.HailOfFeathersAOE1, AID.HailOfFeathersAOE2, AID.HailOfFeathersAOE3, AID.HailOfFeathersAOE4, AID.HailOfFeathersAOE5, AID.HailOfFeathersAOE6];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = _aoes.Count;
        if (count == 0)
            return [];
        var max = count > 2 ? 2 : count;
        List<AOEInstance> aoes = new(max);
        for (var i = 0; i < max; ++i)
            aoes.Add(_aoes[i]);
        return aoes;
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (casts.Contains((AID)spell.Action.ID))
        {
            _aoes.Add(new(_shape, caster.Position, spell.Rotation, Module.CastFinishAt(spell)));
            _aoes.SortBy(x => x.Activation);
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (_aoes.Count != 0 && casts.Contains((AID)spell.Action.ID))
        {
            _aoes.RemoveAt(0);
            ++NumCasts;
        }
    }
}

class FeatherOfRuin(BossModule module) : Components.Adds(module, (uint)OID.FeatherOfRuin);

class BlightedBolt : Components.GenericAOEs
{
    private readonly List<Actor> _targets = [];
    private DateTime _activation;

    private static readonly AOEShapeCircle _shape = new(8);

    public BlightedBolt(BossModule module) : base(module)
    {
        var platform = module.FindComponent<ThunderPlatform>();
        if (platform != null)
        {
            foreach (var (i, _) in module.Raid.WithSlot(true, true, true))
            {
                platform.RequireHint[i] = true;
                platform.RequireLevitating[i] = false;
            }
        }
    }

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_targets.Count < 6 || _targets.Any(t => t.IsDead))
            foreach (var t in _targets.Where(t => !t.IsDead))
                yield return new(_shape, t.Position, default, _activation);
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        base.AddHints(slot, actor, hints);
        if (_targets.Count == 6 && !_targets.Any(t => t.IsDead))
            hints.Add("Kill feather!");
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.BlightedBoltAOE)
        {
            _targets.AddIfNonNull(WorldState.Actors.Find(spell.TargetID));
            _activation = Module.CastFinishAt(spell);
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.BlightedBoltAOE)
        {
            ++NumCasts;
            _targets.RemoveAll(t => t.InstanceID == spell.TargetID);
        }
    }
}
