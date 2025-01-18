namespace BossMod.Dawntrail.Raid.M04NWickedThunder;

class SidewiseSpark(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCone cone = new(60, 90.Degrees());
    private static readonly AOEShapeRect rect = new(40, 8);
    private readonly List<AOEInstance> _aoes = [];
    private static readonly HashSet<AID> castEnd = [AID.SidewiseSpark1, AID.SidewiseSpark2, AID.SidewiseSpark3, AID.SidewiseSpark4, AID.SidewiseSpark5, AID.SidewiseSpark6, AID.Burst];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = _aoes.Count;
        if (count == 0)
            return [];
        var max = count > 2 ? 2 : count;
        List<AOEInstance> aoes = new(max);
        for (var i = 0; i < max; ++i)
        {
            var aoe = _aoes[i];
            if (i == 0)
                aoes.Add(aoe with { Color = count == 1 || count > 1 && _aoes[0].Activation == _aoes[1].Activation ? 0 : Colors.Danger });
            else if (i == 1)
                aoes.Add(aoe with { Risky = _aoes[0].Activation == _aoes[1].Activation });
        }
        return aoes;
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.SidewiseSpark1 or AID.SidewiseSpark2 or AID.SidewiseSpark3 or AID.SidewiseSpark4)
            _aoes.Add(new(cone, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell)));
        else if ((AID)spell.Action.ID == AID.Burst)
            _aoes.Add(new(rect, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell)));
    }

    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if ((OID)actor.OID == OID.WickedReplica)
        {
            var activation = WorldState.CurrentTime.AddSeconds(8);
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
        if (_aoes.Count != 0 && castEnd.Contains((AID)spell.Action.ID))
            _aoes.RemoveAt(0);
    }
}
