namespace BossMod.Dawntrail.Alliance.A14ShadowLord;

class GigaSlash3(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.GigaSlash3), new AOEShapeCone(60, 112.5f.Degrees()));
class GigaSlash4(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.GigaSlash4), new AOEShapeCone(60, 135.Degrees()));
class GigaSlash5(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.GigaSlash5), new AOEShapeCone(60, 112.5f.Degrees()));
class GigaSlash6(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.GigaSlash6), new AOEShapeCone(60, 135.Degrees()));
class UmbraSmash2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.UmbraSmash2), new AOEShapeRect(60, 5));
class UmbraSmash5(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.UmbraSmash5), new AOEShapeRect(60, 5));
class UmbraSmash6(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.UmbraSmash6), new AOEShapeRect(60, 5));
class UmbraSmash7(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.UmbraSmash7), new AOEShapeRect(60, 5));
class UmbraSmash8(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.UmbraSmash8), new AOEShapeRect(60, 5));
class UmbraWave(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.UmbraWave), new AOEShapeRect(5, 30));
class FlamesOfHatred(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.FlamesOfHatred));
class Implosion1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Implosion1), new AOEShapeCone(60, 90.Degrees()));
class Implosion2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Implosion2), new AOEShapeCone(12, 90.Degrees()));
class Implosion3(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Implosion3), new AOEShapeCone(60, 90.Degrees()));
class Implosion4(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Implosion4), new AOEShapeCone(12, 90.Degrees()));
class CthonicFury1(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.CthonicFury1));
class CthonicFury2(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.CthonicFury2));
class BurningCourt(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BurningCourt), new AOEShapeCircle(8));
class BurningKeep(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BurningKeep), new AOEShapeRect(23, 11.5f, 23));
class TeraSlash(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.TeraSlash));
class GigaSlashNightfall1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.GigaSlashNightfall1), new AOEShapeCone(60, 105.Degrees()));
class GigaSlashNightfall2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.GigaSlashNightfall2), new AOEShapeCone(60, 105.Degrees()));
class GigaSlashNightfall3(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.GigaSlashNightfall3), new AOEShapeCone(60, 112.5f.Degrees()));
class GigaSlashNightfall4(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.GigaSlashNightfall4), new AOEShapeCone(60, 135.Degrees()));
class GigaSlashNightfall5(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.GigaSlashNightfall5), new AOEShapeCone(60, 112.5f.Degrees()));
class GigaSlashNightfall6(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.GigaSlashNightfall6), new AOEShapeCone(60, 135.Degrees()));
class DarkNova(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.DarkNova), 6);
class SoulBinding(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.SoulBinding), new AOEShapeCircle(9));
class Impact1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Impact1), new AOEShapeCircle(3));
class Impact2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Impact2), new AOEShapeCircle(3));
class Impact3(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Impact3), new AOEShapeCircle(3));
class DoomArc(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.DoomArc));

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team (LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1015, NameID = 13653)]
public class A14ShadowLord(WorldState ws, Actor primary) : BossModule(ws, primary, new(150, 800), new ArenaBoundsCircle(30));
