namespace BossMod.Endwalker.Trial.T08Asura;

class SixBladedKhadga(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = new(6);
    private static readonly Angle a180 = 180.Degrees();
    private static readonly AOEShapeCone cone = new(20, 90.Degrees());
    private static readonly HashSet<AID> castEnd = [AID.Khadga1, AID.Khadga2, AID.Khadga3, AID.Khadga4, AID.Khadga5, AID.Khadga6];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = _aoes.Count;
        if (count == 0)
            return [];
        var max = count > 2 ? 2 : count;
        var aoes = new AOEInstance[max];
        for (var i = 0; i < max; ++i)
        {
            var aoe = _aoes[i];
            if (i == 0)
                aoes[i] = count > 1 ? aoe with { Color = Colors.Danger } : aoe;
            else if (i == 1)
                aoes[i] = _aoes[0].Rotation.AlmostEqual(_aoes[1].Rotation + a180, Angle.DegToRad) ? aoe with { Risky = false } : aoe;
        }
        return aoes;
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.KhadgaTelegraph1 or AID.KhadgaTelegraph2 or AID.KhadgaTelegraph3)
            _aoes.Add(new(cone, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell, 11.9f)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (_aoes.Count != 0 && castEnd.Contains((AID)spell.Action.ID))
            _aoes.RemoveAt(0);
    }
}
