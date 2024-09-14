namespace BossMod.Endwalker.VariantCriterion.C02AMR.C021Shishio;

class EyeThunderVortex(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];

    private static readonly AOEShapeCircle _shapeCircle = new(15);
    private static readonly AOEShapeDonut _shapeDonut = new(8, 30);
    private static readonly HashSet<AID> castEnd = [AID.NEyeOfTheThunderVortexFirst, AID.NEyeOfTheThunderVortexSecond, AID.NVortexOfTheThunderEyeFirst,
    AID.NVortexOfTheThunderEyeSecond, AID.SEyeOfTheThunderVortexFirst, AID.SEyeOfTheThunderVortexSecond, AID.SVortexOfTheThunderEyeFirst, AID.SVortexOfTheThunderEyeSecond];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes.Take(1);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.NEyeOfTheThunderVortexFirst:
            case AID.SEyeOfTheThunderVortexFirst:
                _aoes.Add(new(_shapeCircle, caster.Position, default, Module.CastFinishAt(spell)));
                _aoes.Add(new(_shapeDonut, caster.Position, default, Module.CastFinishAt(spell, 4)));
                break;
            case AID.NVortexOfTheThunderEyeFirst:
            case AID.SVortexOfTheThunderEyeFirst:
                _aoes.Add(new(_shapeDonut, caster.Position, default, Module.CastFinishAt(spell)));
                _aoes.Add(new(_shapeCircle, caster.Position, default, Module.CastFinishAt(spell, 4)));
                break;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (castEnd.Contains((AID)spell.Action.ID))
        {
            if (_aoes.Count > 0)
                _aoes.RemoveAt(0);
            ++NumCasts;
        }
    }
}
