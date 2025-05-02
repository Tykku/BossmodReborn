namespace BossMod.Dawntrail.Raid.M03NBruteBomber;

class BrutalImpact(BossModule module) : Components.RaidwideCast(module, (uint)AID.BrutalImpactFirst);
class KnuckleSandwich(BossModule module) : Components.CastSharedTankbuster(module, (uint)AID.KnuckleSandwich, 6f);
class BrutalLariat(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.BrutalLariat1, (uint)AID.BrutalLariat2], new AOEShapeRect(50f, 17f));

class MurderousMist(BossModule module) : Components.SimpleAOEs(module, (uint)AID.MurderousMist, new AOEShapeCone(40f, 135f.Degrees()));
class BrutalBurn(BossModule module) : Components.StackWithCastTargets(module, (uint)AID.BrutalBurn, 6f, 8, 8);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 989, NameID = 13356)]
public class M03NBruteBomber(WorldState ws, Actor primary) : BossModule(ws, primary, new(100f, 100f), new ArenaBoundsSquare(15f));
