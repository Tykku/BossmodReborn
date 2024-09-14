namespace BossMod.Endwalker.VariantCriterion.V02MR.V023Gorai;

class FlameAndSulphur(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];

    private static readonly AOEShapeRect _shapeFlameExpand = new(46, 5);
    private static readonly AOEShapeRect _shapeFlameSplit = new(46, 2.5f);
    private static readonly AOEShapeCircle _shapeRockExpand = new(11);
    private static readonly AOEShapeDonut _shapeRockSplit = new(5, 16);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        var activation = Module.CastFinishAt(spell, 3.1f);
        switch ((AID)spell.Action.ID)
        {
            case AID.BrazenBalladSplitting:
                foreach (var a in Module.Enemies(OID.FlameAndSulphurFlame))
                    _aoes.Add(new(_shapeFlameExpand, a.Position, a.Rotation, activation));
                foreach (var a in Module.Enemies(OID.FlameAndSulphurRock))
                    _aoes.Add(new(_shapeRockExpand, a.Position, a.Rotation, activation));
                break;
            case AID.BrazenBalladExpanding:
                foreach (var a in Module.Enemies(OID.FlameAndSulphurFlame))
                {
                    var offset = a.Rotation.ToDirection().OrthoL() * 7.5f;
                    _aoes.Add(new(_shapeFlameSplit, a.Position + offset, a.Rotation, activation));
                    _aoes.Add(new(_shapeFlameSplit, a.Position - offset, a.Rotation, activation));
                }
                foreach (var a in Module.Enemies(OID.FlameAndSulphurRock))
                    _aoes.Add(new(_shapeRockSplit, a.Position, a.Rotation, activation));
                break;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.FireSpreadExpand or AID.FireSpreadSplit or AID.FallingRockExpand or AID.FallingRockSplit)
            _aoes.Clear();
    }
}
