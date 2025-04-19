namespace BossMod.Dawntrail.Raid.M03NBruteBomber;

class BrutalImpact(BossModule module) : Components.RaidwideCast(module, (uint)AID.BrutalImpactFirst);
class KnuckleSandwich(BossModule module) : Components.CastSharedTankbuster(module, (uint)AID.KnuckleSandwich, 6);

abstract class BrutalLariat(BossModule module, uint aid) : Components.SimpleAOEs(module, aid, new AOEShapeRect(50, 17));
class BrutalLariat1(BossModule module) : BrutalLariat(module, (uint)AID.BrutalLariat1);
class BrutalLariat2(BossModule module) : BrutalLariat(module, (uint)AID.BrutalLariat2);

class MurderousMist(BossModule module) : Components.SimpleAOEs(module, (uint)AID.MurderousMist, new AOEShapeCone(40, 135.Degrees()));
class BrutalBurn(BossModule module) : Components.StackWithCastTargets(module, (uint)AID.BrutalBurn, 6, 8, 8);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 989, NameID = 13356)]
public class M03NBruteBomber(WorldState ws, Actor primary) : BossModule(ws, primary, new(100, 100), new ArenaBoundsSquare(15));
