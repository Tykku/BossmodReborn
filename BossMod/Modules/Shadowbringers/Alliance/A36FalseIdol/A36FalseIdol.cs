namespace BossMod.Shadowbringers.Alliance.A36FalseIdol;

class MadeMagic1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.MadeMagic1), new AOEShapeRect(50, 15));
class MadeMagic2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.MadeMagic2), new AOEShapeRect(50, 15));
class ScreamingScore(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.ScreamingScore));
class ScatteredMagic(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.ScatteredMagic), 4);
class DarkerNote2(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.DarkerNote2), 6);

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 779, NameID = 9948)]
public class A36FalseIdol(WorldState ws, Actor primary) : BossModule(ws, primary, new(-700, -700), new ArenaBoundsSquare(25));
