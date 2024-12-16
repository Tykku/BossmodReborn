namespace BossMod.Dawntrail.Raid.M04NWickedThunder;

class WitchHunt(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];
    private static readonly AOEShapeCircle circle = new(6);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = _aoes.Count;
        if (count > 0)
            for (var i = 0; i < count / 2; ++i)
                yield return _aoes[i] with { Color = Colors.Danger };
        for (var i = count / 2; i < count; ++i)
            yield return _aoes[i] with { Risky = count < 12 };
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.WitchHuntTelegraph)
            _aoes.Add(new(circle, spell.LocXZ, default, Module.CastFinishAt(spell, 6.3f)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (_aoes.Count != 0 && (AID)spell.Action.ID == AID.WitchHunt)
            _aoes.RemoveAt(0);
    }
}
