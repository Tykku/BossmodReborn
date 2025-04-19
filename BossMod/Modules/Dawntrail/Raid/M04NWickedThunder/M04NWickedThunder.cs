namespace BossMod.Dawntrail.Raid.M04NWickedThunder;

class WickedJolt(BossModule module) : Components.BaitAwayCast(module, (uint)AID.WickedJolt, new AOEShapeRect(60, 2.5f), endsOnCastEvent: true, tankbuster: true);

class WickedBolt(BossModule module) : Components.StackWithIcon(module, (uint)IconID.WickedBolt, (uint)AID.WickedBolt, 5, 5, 8, 8, 5);
class SoaringSoulpress(BossModule module) : Components.StackWithIcon(module, (uint)IconID.SoaringSoulpress, (uint)AID.SoaringSoulpress, 6, 5.4f, 8, 8);
class WrathOfZeus(BossModule module) : Components.RaidwideCast(module, (uint)AID.WrathOfZeus);
class BewitchingFlight(BossModule module) : Components.SimpleAOEs(module, (uint)AID.BewitchingFlight, new AOEShapeRect(40, 2.5f));
class Thunderslam(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Thunderslam, 5);
class Thunderstorm(BossModule module) : Components.SpreadFromCastTargets(module, (uint)AID.Thunderstorm, 6);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 991, NameID = 13057)]
public class M04NWickedThunder(WorldState ws, Actor primary) : BossModule(ws, primary, ArenaChanges.DefaultCenter, ArenaChanges.DefaultBounds);
