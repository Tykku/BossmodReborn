namespace BossMod.Dawntrail.Raid.M05NDancingGreen;

class DoTheHustle(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.DoTheHustle1, (uint)AID.DoTheHustle2], new AOEShapeCone(50f, 90f.Degrees()));
class DeepCut(BossModule module) : Components.BaitAwayIcon(module, new AOEShapeCone(60, 22.5f.Degrees()), (uint)IconID.DeepCut, (uint)AID.DeepCut, 5f, tankbuster: true);
class FullBeat(BossModule module) : Components.StackWithCastTargets(module, (uint)AID.FullBeat, 6f, 8, 8);
class CelebrateGoodTimes(BossModule module) : Components.RaidwideCast(module, (uint)AID.CelebrateGoodTimes);
class DiscoInfernal(BossModule module) : Components.RaidwideCast(module, (uint)AID.DiscoInfernal);
class LetsPose1(BossModule module) : Components.RaidwideCast(module, (uint)AID.LetsPose1);
class LetsPose2(BossModule module) : Components.RaidwideCast(module, (uint)AID.LetsPose2);
class EighthBeats(BossModule module) : Components.SpreadFromCastTargets(module, (uint)AID.EighthBeats, 5f);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1019, NameID = 13778)]
public class M05NDancingGreen(WorldState ws, Actor primary) : BossModule(ws, primary, new(100f, 100f), new ArenaBoundsSquare(20f));
