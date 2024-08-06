namespace BossMod.Dawntrail.Raid.M03NBruteBomber;

class BrutalImpact(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.BrutalImpactFirst));
class KnuckleSandwich(BossModule module) : Components.CastSharedTankbuster(module, ActionID.MakeSpell(AID.KnuckleSandwich), 6);
class BrutalLariat1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BrutalLariat1), new AOEShapeRect(20, 30, 5, -90.Degrees()));
class BrutalLariat2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BrutalLariat2), new AOEShapeRect(20, 30, 5, 90.Degrees()));
class MurderousMist(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.MurderousMist), new AOEShapeCone(40, 135.Degrees()));
class BrutalBurn(BossModule module) : Components.StackWithCastTargets(module, ActionID.MakeSpell(AID.BrutalBurn), 6, 8);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 989, NameID = 13356)]
public class M03NBruteBomber(WorldState ws, Actor primary) : BossModule(ws, primary, new(100, 100), new ArenaBoundsSquare(15));
