namespace BossMod.Dawntrail.FATE.Ttokrrone;

class Sandspheres(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCircle circleSmall = new(6);
    private static readonly AOEShapeCircle circleBig = new(12);
    private readonly List<AOEInstance> _aoesSmall = [];
    private readonly List<AOEInstance> _aoesBig = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var sandspheres = Module.Enemies(OID.SandSphere).Count(x => !x.IsDead);
        return sandspheres == 7 ? _aoesSmall.Take(7).Concat(_aoesBig.Take(7)) : _aoesSmall.Take(8).Concat(_aoesBig.Take(8));
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.SummoningSands:
                _aoesSmall.Add(new(circleSmall, caster.Position, default, Module.CastFinishAt(spell)));
                break;
            case AID.Sandburst1:
            case AID.Sandburst2:
                _aoesBig.Add(new(circleBig, caster.Position, default, Module.CastFinishAt(spell)));
                break;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (_aoesSmall.Count != 0 && (AID)spell.Action.ID == AID.SummoningSands)
            _aoesSmall.RemoveAt(0);
        else if (_aoesBig.Count == 0)
            return;
        switch ((AID)spell.Action.ID)
        {
            case AID.Sandburst1:
            case AID.Sandburst2:
                _aoesBig.RemoveAt(0);
                break;
        }
    }
}
