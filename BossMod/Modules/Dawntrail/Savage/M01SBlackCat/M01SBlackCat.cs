namespace BossMod.Dawntrail.Savage.M01SBlackCat;

class BiscuitMaker(BossModule module) : Components.TankSwap(module, (uint)AID.BiscuitMaker, (uint)AID.BiscuitMaker, (uint)AID.BiscuitMakerSecond, 2, null, true);
class QuadrupleSwipeBoss(BossModule module) : Components.StackWithCastTargets(module, (uint)AID.QuadrupleSwipeBossAOE, 4, 2, 2);
class DoubleSwipeBoss(BossModule module) : Components.StackWithCastTargets(module, (uint)AID.DoubleSwipeBossAOE, 5, 4, 4);
class QuadrupleSwipeShade(BossModule module) : Components.StackWithCastTargets(module, (uint)AID.QuadrupleSwipeShadeAOE, 4, 2, 2);
class DoubleSwipeShade(BossModule module) : Components.StackWithCastTargets(module, (uint)AID.DoubleSwipeShadeAOE, 5, 4, 4);
class Nailchipper(BossModule module) : Components.SpreadFromCastTargets(module, (uint)AID.NailchipperAOE, 5);
class TempestuousTear(BossModule module) : Components.LineStack(module, (uint)AID.TempestuousTearTargetSelect, (uint)AID.TempestuousTearAOE, 5, 100, 3, 1, 4);
class Overshadow(BossModule module) : Components.LineStack(module, (uint)AID.OvershadowTargetSelect, (uint)AID.OvershadowAOE, 5.1f, 100, 2.5f, 7, 8);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 986, NameID = 12686, PlanLevel = 100)]
public class M01SBlackCat(WorldState ws, Actor primary) : Raid.M01NBlackCat.M01NBlackCat(ws, primary);
