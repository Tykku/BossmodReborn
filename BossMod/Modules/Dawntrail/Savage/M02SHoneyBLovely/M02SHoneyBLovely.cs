namespace BossMod.Dawntrail.Savage.M02SHoneyBLovely;

class StingingSlash(BossModule module) : Components.BaitAwayIcon(module, new AOEShapeCone(50f, 45f.Degrees()), (uint)IconID.StingingSlash, (uint)AID.StingingSlashAOE);
class KillerSting(BossModule module) : Components.IconSharedTankbuster(module, (uint)IconID.KillerSting, (uint)AID.KillerStingAOE, 6f);

class BlindingLoveBait : Components.SimpleAOEs
{
    public BlindingLoveBait(BossModule module) : base(module, (uint)AID.BlindingLoveBaitAOE, new AOEShapeRect(50f, 4f)) { MaxDangerColor = 2; }
}

class BlindingLoveCharge(BossModule module, uint aid) : Components.SimpleAOEs(module, aid, new AOEShapeRect(45f, 5f));
class BlindingLoveCharge1(BossModule module) : BlindingLoveCharge(module, (uint)AID.BlindingLoveCharge1AOE);
class BlindingLoveCharge2(BossModule module) : BlindingLoveCharge(module, (uint)AID.BlindingLoveCharge2AOE);

class PoisonStingBait(BossModule module) : Components.BaitAwayCast(module, (uint)AID.PoisonStingAOE, 6f);
class PoisonStingVoidzone(BossModule module) : Components.Voidzone(module, 6f, m => m.Enemies(OID.PoisonStingVoidzone).Where(z => z.EventState != 7));
class BeeSting(BossModule module) : Components.StackWithCastTargets(module, (uint)AID.BeeStingAOE, 6f, 4, 4);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 988, NameID = 12685, PlanLevel = 100)]
public class M02SHoneyBLovely(WorldState ws, Actor primary) : Raid.M02NHoneyBLovely.M02NHoneyBLovely(ws, primary);