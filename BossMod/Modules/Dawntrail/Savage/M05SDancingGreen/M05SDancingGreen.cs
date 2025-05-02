namespace BossMod.Dawntrail.Savage.M05SDancingGreen;

class EighthBeats(BossModule module) : Components.SpreadFromCastTargets(module, (uint)AID.EighthBeats, 5f);
class QuarterBeats(BossModule module) : Components.StackWithCastTargets(module, (uint)AID.QuarterBeats, 4f, 2, 2);
class DoTheHustle(BossModule module) : Components.SimpleAOEGroupsByTimewindow(module, [(uint)AID.DoTheHustle1, (uint)AID.DoTheHustle2,
(uint)AID.DoTheHustle3, (uint)AID.DoTheHustle4], new AOEShapeCone(50f, 90f.Degrees()));

class DeepCut(BossModule module) : Components.BaitAwayIcon(module, new AOEShapeCone(60f, 22.5f.Degrees()), (uint)IconID.DeepCut, (uint)AID.DeepCut, 5.7f, tankbuster: true);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1020, NameID = 13778, PlanLevel = 100)]
public class M05SDancingGreen(WorldState ws, Actor primary) : BossModule(ws, primary, new(100f, 100f), new ArenaBoundsSquare(20f));
