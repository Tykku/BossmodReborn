namespace BossMod.Endwalker.Alliance.A32Llymlaen;

class ToTheLast(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect rect = new(80, 5);
    private readonly List<AOEInstance> _aoes = new(3);

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
                aoes.Add(count > 1 ? aoe with { Color = Colors.Danger } : aoe);
            else
                aoes.Add(aoe with { Risky = false });
        }
        return aoes;
    }
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.ToTheLastVisual)
            _aoes.Add(new(rect, caster.Position, spell.Rotation, Module.CastFinishAt(spell, 5 + 1.9f + _aoes.Count)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.ToTheLastAOE)
        {
            ++NumCasts;
            if (_aoes.Count != 0)
                _aoes.RemoveAt(0);
        }
    }
}
