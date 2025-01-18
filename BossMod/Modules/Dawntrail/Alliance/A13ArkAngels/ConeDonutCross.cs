namespace BossMod.Dawntrail.Alliance.A13ArkAngels;

class ConcertedDissolution(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.ConcertedDissolution), new AOEShapeCone(40, 20.Degrees()))
{
    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = Casters.Count;
        if (count == 0)
            return [];
        var chain = Module.FindComponent<LightsChain>();
        var check = chain != null && chain.Casters.Count != 0;

        List<AOEInstance> result = new(count);
        for (var i = 0; i < count; ++i)
        {
            result.Add(Casters[i] with { Color = check ? Colors.Danger : Colors.AOE });
        }
        return result;
    }
}

class LightsChain(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.LightsChain), new AOEShapeDonut(4, 40))
{
    private readonly ConcertedDissolution? _aoe = module.FindComponent<ConcertedDissolution>();

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (Casters.Count == 0)
            return [];

        var reaver = Module.FindComponent<CrossReaver>();
        var check = _aoe != null && _aoe.Casters.Count != 0;
        var check2 = reaver != null && reaver.Casters.Count != 0;

        return [Casters[0] with { Color = check2 ? Colors.Danger : Colors.AOE, Risky = !check }];
    }
}

class CrossReaver(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.CrossReaverAOE), new AOEShapeCross(50, 6))
{
    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (Casters.Count == 0)
            return [];
        var chain = Module.FindComponent<LightsChain>();
        var check = chain != null && chain.Casters.Count != 0;
        return [Casters[0] with { Risky = !check }];
    }
}
