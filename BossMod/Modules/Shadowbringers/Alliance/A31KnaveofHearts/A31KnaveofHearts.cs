namespace BossMod.Shadowbringers.Alliance.A31KnaveofHearts;

class Roar(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.Roar));

class ColossalImpact6(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.ColossalImpact6), new AOEShapeRect(61, 10));
class ColossalImpact7(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.ColossalImpact7), new AOEShapeRect(61, 10));
class ColossalImpact8(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.ColossalImpact8), new AOEShapeRect(61, 10));
class ColossalImpactLeft(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.ColossalImpactLeft), new AOEShapeRect(90, 61, -10, DirectionOffset: -90.Degrees()));
class ColossalImpactRight(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.ColossalImpactRight), new AOEShapeRect(90, 61, -10, DirectionOffset: 90.Degrees()));
class ColossalImpactMiddle(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.ColossalImpactMiddle), new AOEShapeRect(61, 10));

class MagicArtilleryBeta2(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.MagicArtilleryBeta2), 3);
class MagicArtilleryAlpha2(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.MagicArtilleryAlpha2), 5);
class LightLeap2(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.LightLeap2), 25);
class BoxSpawn(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BoxSpawn), new AOEShapeRect(8, 4));
class MagicBarrage(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.MagicBarrage), new AOEShapeRect(61, 2.5f));
class Lunge(BossModule module) : Components.KnockbackFromCastTarget(module, ActionID.MakeSpell(AID.Lunge), 60, stopAtWall: true, kind: Kind.DirForward);
class Energy(BossModule module) : Components.PersistentVoidzone(module, 1, m => m.Enemies(OID.Energy).Where(z => z.EventState != 7));

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 779, NameID = 9955)]
public class A31KnaveofHearts(WorldState ws, Actor primary) : BossModule(ws, primary, new(-800, -724.4f), new ArenaBoundsSquare(29.5f));
