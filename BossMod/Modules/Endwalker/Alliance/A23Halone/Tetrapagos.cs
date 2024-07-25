namespace BossMod.Endwalker.Alliance.A23Halone;

class Tetrapagos(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];
    private static readonly AOEShapeCircle circle = new(10);
    private static readonly AOEShapeDonut donut = new(10, 30);
    private static readonly AOEShapeCone cone = new(30, 90.Degrees());

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_aoes.Count > 0)
            yield return _aoes[0] with { Color = Colors.Danger };
        if (_aoes.Count > 1)
            yield return _aoes[1] with { Risky = false };
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        AOEShape? shape = (AID)spell.Action.ID switch
        {
            AID.TetrapagosHailstormPrepare => circle,
            AID.TetrapagosSwirlPrepare => donut,
            AID.TetrapagosRightrimePrepare or AID.TetrapagosLeftrimePrepare => cone,
            _ => null
        };
        if (shape != null)
            _aoes.Add(new(shape, caster.Position, caster.Rotation, Module.CastFinishAt(spell, 7.3f)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.TetrapagosHailstormAOE or AID.TetrapagosSwirlAOE or AID.TetrapagosRightrimeAOE or AID.TetrapagosLeftrimeAOE)
        {
            ++NumCasts;
            if (_aoes.Count > 0)
                _aoes.RemoveAt(0);
        }
    }
}
