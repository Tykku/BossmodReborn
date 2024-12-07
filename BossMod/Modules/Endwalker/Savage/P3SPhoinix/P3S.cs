namespace BossMod.Endwalker.Savage.P3SPhoinix;

class HeatOfCondemnation(BossModule module) : Components.TankbusterTether(module, ActionID.MakeSpell(AID.HeatOfCondemnationAOE), (uint)TetherID.HeatOfCondemnation, 6);
class DevouringBrand(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.DevouringBrandAOE), new AOEShapeCross(40, 5));
class SunBirdLarge(BossModule module) : Components.Adds(module, (uint)OID.SunbirdLarge)
{
    public int FinishedTethers;
    public override void Update()
    {
        var comp = Module.FindComponent<BirdTether>();
        if (comp != null)
            FinishedTethers = comp.NumFinishedChains;
    }
}

class SunBirdSmall(BossModule module) : Components.Adds(module, (uint)OID.SunbirdSmall);
class DarkenedFireAdd(BossModule module) : Components.Adds(module, (uint)OID.DarkenedFire);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 807, NameID = 10720, PlanLevel = 90)]
public class P3S(WorldState ws, Actor primary) : BossModule(ws, primary, new(100, 100), new ArenaBoundsCircle(20));
