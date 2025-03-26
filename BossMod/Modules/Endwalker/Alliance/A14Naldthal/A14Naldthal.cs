namespace BossMod.Endwalker.Alliance.A14Naldthal;

class GoldenTenet(BossModule module) : Components.CastSharedTankbuster(module, ActionID.MakeSpell(AID.GoldenTenetAOE), 6f);
class StygianTenet(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.StygianTenetAOE), 6f);

class HellOfFire(BossModule module, AID aid) : Components.SimpleAOEs(module, ActionID.MakeSpell(aid), new AOEShapeCone(60f, 90f.Degrees()));
class HellOfFireFront(BossModule module) : HellOfFire(module, AID.HellOfFireFrontAOE);
class HellOfFireBack(BossModule module) : HellOfFire(module, AID.HellOfFireBackAOE);

class WaywardSoul(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.WaywardSoulAOE), 18f, 3);
class SoulVessel(BossModule module) : Components.Adds(module, (uint)OID.SoulVesselReal);
class Twingaze(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Twingaze), new AOEShapeCone(60f, 15f.Degrees()));
class MagmaticSpell(BossModule module) : Components.StackWithCastTargets(module, ActionID.MakeSpell(AID.MagmaticSpellAOE), 6f, 8);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 866, NameID = 11286, SortOrder = 6, PlanLevel = 90)]
public class A14Naldthal(WorldState ws, Actor primary) : BossModule(ws, primary, arena.Center, arena)
{
    private static readonly ArenaBoundsComplex arena = new([new Polygon(new(750f, -750f), 29f, 180)]);
}
