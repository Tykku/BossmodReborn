namespace BossMod.Dawntrail.Raid.M01NBlackCat;

class PredaceousPounce(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCircle circle = new(11);
    private bool sorted;
    private readonly List<AOEInstance> _aoes = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_aoes.Count > 0)
        {
            var aoeCount = Math.Clamp(_aoes.Count, 0, 2);
            for (var i = aoeCount; i < _aoes.Count; i++)
                yield return _aoes[i];
            for (var i = 0; i < aoeCount; i++)
                yield return _aoes[i] with { Color = Colors.Danger };
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.PredaceousPounceTelegraphCharge1:
            case AID.PredaceousPounceTelegraphCharge2:
            case AID.PredaceousPounceTelegraphCharge3:
            case AID.PredaceousPounceTelegraphCharge4:
            case AID.PredaceousPounceTelegraphCharge5:
            case AID.PredaceousPounceTelegraphCharge6:
                var dir = spell.LocXZ - caster.Position;
                _aoes.Add(new(new AOEShapeRect(dir.Length(), 3), caster.Position, Angle.FromDirection(dir), Module.CastFinishAt(spell)));
                break;
            case AID.PredaceousPounceTelegraphCircle1:
            case AID.PredaceousPounceTelegraphCircle2:
            case AID.PredaceousPounceTelegraphCircle3:
            case AID.PredaceousPounceTelegraphCircle4:
            case AID.PredaceousPounceTelegraphCircle5:
            case AID.PredaceousPounceTelegraphCircle6:
                _aoes.Add(new(circle, caster.Position, default, Module.CastFinishAt(spell)));
                break;
        }
        if (_aoes.Count == 12 && !sorted)
        {
            _aoes.SortBy(x => x.Activation);
            for (var i = 0; i < _aoes.Count; i++)
            {
                var aoe = _aoes[i];
                aoe.Activation = Module.WorldState.FutureTime(13.5f + i * 0.5f);
                _aoes[i] = aoe;
            }
            sorted = true;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (_aoes.Count > 0)
            switch ((AID)spell.Action.ID)
            {
                case AID.PredaceousPounceCharge1:
                case AID.PredaceousPounceCharge2:
                case AID.PredaceousPounceCircle1:
                case AID.PredaceousPounceCircle2:
                    _aoes.RemoveAt(0);
                    sorted = false;
                    break;
            }
    }
}
