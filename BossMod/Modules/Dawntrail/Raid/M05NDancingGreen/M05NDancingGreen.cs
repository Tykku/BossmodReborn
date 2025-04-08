namespace BossMod.Dawntrail.Raid.M05NDancingGreen;

abstract class DoTheHustle(BossModule module, AID aid) : Components.SimpleAOEs(module, ActionID.MakeSpell(aid), new AOEShapeCone(50f, 90f.Degrees()));
class DoTheHustle1(BossModule module) : DoTheHustle(module, AID.DoTheHustle1);
class DoTheHustle2(BossModule module) : DoTheHustle(module, AID.DoTheHustle2);

class DeepCut(BossModule module) : Components.BaitAwayIcon(module, new AOEShapeCone(60, 22.5f.Degrees()), (uint)IconID.DeepCut, ActionID.MakeSpell(AID.DeepCut), 5f, tankbuster: true);
class FullBeat(BossModule module) : Components.StackWithCastTargets(module, ActionID.MakeSpell(AID.FullBeat), 6f, 8, 8);
class CelebrateGoodTimes(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.CelebrateGoodTimes));
class DiscoInfernal(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.DiscoInfernal));
class LetsPose1(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.LetsPose1));
class LetsPose2(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.LetsPose2));
class EighthBeats(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.EighthBeats), 5f);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1019, NameID = 13778)]
public class M05NDancingGreen(WorldState ws, Actor primary) : BossModule(ws, primary, new(100f, 100f), new ArenaBoundsSquare(20f));
