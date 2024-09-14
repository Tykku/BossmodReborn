namespace BossMod.Dawntrail.Raid.M01NBlackCat;

class PredaceousPounce(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];
    private bool sorted;
    private static readonly AOEShapeCircle circle = new(11);
    private static readonly HashSet<AID> chargeTelegraphs = [AID.PredaceousPounceTelegraphCharge1, AID.PredaceousPounceTelegraphCharge2,
            AID.PredaceousPounceTelegraphCharge3, AID.PredaceousPounceTelegraphCharge4, AID.PredaceousPounceTelegraphCharge5,
            AID.PredaceousPounceTelegraphCharge6];
    private static readonly HashSet<AID> circleTelegraphs = [AID.PredaceousPounceTelegraphCircle1, AID.PredaceousPounceTelegraphCircle2,
            AID.PredaceousPounceTelegraphCircle3, AID.PredaceousPounceTelegraphCircle4, AID.PredaceousPounceTelegraphCircle5,
            AID.PredaceousPounceTelegraphCircle6];
    private static readonly HashSet<AID> castEnd = [AID.PredaceousPounceCharge1, AID.PredaceousPounceCharge2, AID.PredaceousPounceCircle1, AID.PredaceousPounceCircle2];

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
        if (chargeTelegraphs.Contains((AID)spell.Action.ID))
        {
            var dir = spell.LocXZ - caster.Position;
            _aoes.Add(new(new AOEShapeRect(dir.Length(), 3), caster.Position, Angle.FromDirection(dir), Module.CastFinishAt(spell)));
        }
        else if (circleTelegraphs.Contains((AID)spell.Action.ID))
            _aoes.Add(new(circle, caster.Position, default, Module.CastFinishAt(spell)));
        if (_aoes.Count == 12 && !sorted)
        {
            _aoes.SortBy(x => x.Activation);
            for (var i = 0; i < _aoes.Count; i++)
                _aoes[i] = new(_aoes[i].Shape, _aoes[i].Origin, _aoes[i].Rotation, WorldState.FutureTime(13.5f + i * 0.5f));
            sorted = true;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (castEnd.Contains((AID)spell.Action.ID))
        {
            ++NumCasts;
            if (_aoes.Count > 0)
                _aoes.RemoveAt(0);
        }
    }
}
