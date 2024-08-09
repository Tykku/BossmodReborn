namespace BossMod.Dawntrail.Raid.M04NWickedThunder;

class SidewiseSpark(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCone cone = new(60, 90.Degrees());
    private static readonly AOEShapeRect rect = new(40, 8);
    private readonly List<AOEInstance> _aoes = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_aoes.Count > 0)
            yield return _aoes[0] with { Color = _aoes.Count == 1 || _aoes.Count > 1 && _aoes[0].Activation == _aoes[1].Activation ? Colors.AOE : Colors.Danger };
        if (_aoes.Count > 1)
            yield return _aoes[1] with { Risky = _aoes[0].Activation == _aoes[1].Activation };
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.SidewiseSpark1 or AID.SidewiseSpark2 or AID.SidewiseSpark3 or AID.SidewiseSpark4)
            _aoes.Add(new(cone, caster.Position, spell.Rotation, Module.CastFinishAt(spell)));
        else if ((AID)spell.Action.ID == AID.Burst)
            _aoes.Add(new(rect, caster.Position, spell.Rotation, Module.CastFinishAt(spell)));
    }

    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if ((OID)actor.OID == OID.WickedReplica)
        {
            var activation = Module.WorldState.CurrentTime.AddSeconds(8);
            switch (id)
            {
                case 0x11D8:
                    _aoes.Add(new(cone, actor.Position, actor.Rotation + 90.Degrees(), activation));
                    break;
                case 0x11D6:
                    _aoes.Add(new(cone, actor.Position, actor.Rotation - 90.Degrees(), activation));
                    break;
            }
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (_aoes.Count > 0 && (AID)spell.Action.ID is AID.SidewiseSpark1 or AID.SidewiseSpark2 or AID.SidewiseSpark3 or AID.SidewiseSpark4 or AID.SidewiseSpark5 or AID.SidewiseSpark6 or AID.Burst)
            _aoes.RemoveAt(0);
    }
}
