namespace BossMod.Dawntrail.Alliance.A11Prishe;

class Banishga(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.Banishga));

// Knuckle Sandwich and Brittle Impact happen at the same time and need to be staggered
class KnuckleSandwichAOE1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.KnuckleSandwichAOE1), new AOEShapeCircle(9));
class KnuckleSandwichAOE2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.KnuckleSandwichAOE2), new AOEShapeCircle(18));
class KnuckleSandwichAOE3(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.KnuckleSandwichAOE3), new AOEShapeCircle(27));
class BrittleImpact1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BrittleImpact1), new AOEShapeDonut(9, 60));
class BrittleImpact2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BrittleImpact2), new AOEShapeDonut(18, 60));
class BrittleImpact3(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BrittleImpact3), new AOEShapeDonut(27, 60));

class NullifyingDropkick1(BossModule module) : Components.CastSharedTankbuster(module, ActionID.MakeSpell(AID.NullifyingDropkick1), 6);

class Holy2(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.Holy2), 6);

class BanishgaIV(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.BanishgaIV));
class Explosion(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Explosion), new AOEShapeCircle(8));

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team (LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1015, NameID = 13351)]
public class A11Prishe(WorldState ws, Actor primary) : BossModule(ws, primary, new(800, 400), new ArenaBoundsSquare(35));
