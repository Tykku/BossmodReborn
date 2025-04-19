namespace BossMod.Shadowbringers.Foray.Duel.Duel4Dabog;

abstract class RightArmBlaster(BossModule module, uint aid) : Components.SimpleAOEs(module, aid, new AOEShapeRect(100, 3));
class RightArmBlasterFragment(BossModule module) : RightArmBlaster(module, (uint)AID.RightArmBlasterFragment);
class RightArmBlasterBoss(BossModule module) : RightArmBlaster(module, (uint)AID.RightArmBlasterBoss);

class LeftArmSlash(BossModule module) : Components.SimpleAOEs(module, (uint)AID.LeftArmSlash, new AOEShapeCone(10, 90.Degrees())); // TODO: verify angle
class LeftArmWave(BossModule module) : Components.SimpleAOEs(module, (uint)AID.LeftArmWaveAOE, 24);

[ModuleInfo(BossModuleInfo.Maturity.Verified, GroupType = BossModuleInfo.GroupType.BozjaDuel, GroupID = 778, NameID = 19)] // bnpcname=9958
public class Duel4Dabog(WorldState ws, Actor primary) : BossModule(ws, primary, new(250, 710), new ArenaBoundsCircle(20));
