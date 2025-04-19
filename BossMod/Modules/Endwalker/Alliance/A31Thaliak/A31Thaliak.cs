namespace BossMod.Endwalker.Alliance.A31Thaliak;

class Katarraktes(BossModule module) : Components.CastCounter(module, (uint)AID.KatarraktesAOE);
class Thlipsis(BossModule module) : Components.StackWithCastTargets(module, (uint)AID.ThlipsisAOE, 6f, 8);
class Hydroptosis(BossModule module) : Components.SpreadFromCastTargets(module, (uint)AID.HydroptosisAOE, 6f);
class Rhyton(BossModule module) : Components.BaitAwayIcon(module, new AOEShapeRect(70, 3), (uint)IconID.Rhyton, (uint)AID.RhytonAOE, 6f);

class Bank(BossModule module, uint aid) : Components.SimpleAOEs(module, aid, new AOEShapeCone(60, 90.Degrees()));
class LeftBank(BossModule module) : Bank(module, (uint)AID.LeftBank);
class RightBank(BossModule module) : Bank(module, (uint)AID.RightBank);
class HieroglyphikaLeftBank(BossModule module) : Bank(module, (uint)AID.HieroglyphikaLeftBank);
class HieroglyphikaRightBank(BossModule module) : Bank(module, (uint)AID.HieroglyphikaRightBank);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus, LTS", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 962, NameID = 11298, SortOrder = 2, PlanLevel = 90)]
public class A31Thaliak(WorldState ws, Actor primary) : BossModule(ws, primary, TetraktysBorder.NormalCenter, TetraktysBorder.NormalBounds);
