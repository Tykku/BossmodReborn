namespace BossMod.Shadowbringers.Foray.DelubrumReginae.DRS8Queen;

class NorthswainsGlow(BossModule module) : Components.SimpleAOEs(module, (uint)AID.NorthswainsGlowAOE, 20);
class CleansingSlashSecond(BossModule module) : Components.CastCounter(module, (uint)AID.CleansingSlashSecond);
class GodsSaveTheQueen(BossModule module) : Components.CastCounter(module, (uint)AID.GodsSaveTheQueenAOE);

// note: apparently there is no 'front unseen' status
class QueensShot(BossModule module) : Components.CastWeakpoint(module, (uint)AID.QueensShot, new AOEShapeCircle(60), 0, (uint)SID.BackUnseen, (uint)SID.LeftUnseen, (uint)SID.RightUnseen);
class TurretsTourUnseen(BossModule module) : Components.CastWeakpoint(module, (uint)AID.TurretsTourUnseen, new AOEShapeRect(50, 2.5f), 0, (uint)SID.BackUnseen, (uint)SID.LeftUnseen, (uint)SID.RightUnseen);

class OptimalOffensive(BossModule module) : Components.ChargeAOEs(module, (uint)AID.OptimalOffensive, 2.5f);

// note: there are two casters (as usual in bozja content for raidwides)
// TODO: not sure whether it ignores immunes, I assume so...
class OptimalOffensiveKnockback(BossModule module) : Components.SimpleKnockbacks(module, (uint)AID.OptimalOffensiveKnockback, 10, true, 1);

class OptimalPlaySword(BossModule module) : Components.SimpleAOEs(module, (uint)AID.OptimalPlaySword, 10);
class OptimalPlayShield(BossModule module) : Components.SimpleAOEs(module, (uint)AID.OptimalPlayShield, new AOEShapeDonut(5, 60));
class OptimalPlayCone(BossModule module) : Components.SimpleAOEs(module, (uint)AID.OptimalPlayCone, new AOEShapeCone(60, 135.Degrees()));
class PawnOff(BossModule module) : Components.SimpleAOEs(module, (uint)AID.PawnOffReal, 20);

[ModuleInfo(BossModuleInfo.Maturity.Verified, GroupType = BossModuleInfo.GroupType.CFC, GroupID = 761, NameID = 9863, PlanLevel = 80)]
public class DRS8(WorldState ws, Actor primary) : BossModule(ws, primary, new(-272, -415), new ArenaBoundsCircle(25)); // note: initially arena is square, but it quickly changes to circle
