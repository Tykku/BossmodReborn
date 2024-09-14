namespace BossMod.Endwalker.TreasureHunt.ShiftingGymnasionAgonon.GymnasiouTriton;

public enum OID : uint
{
    Boss = 0x3D30, //R=6.08
    GymnasiouEcheneis = 0x3D31, //R=2.2
    Bubble = 0x3D32, //R=1.0
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 872, // Boss/GymnasiouEcheneis->player, no cast, single-target

    PelagicCleaver = 32230, // Boss->self, 3.5s cast, range 40 60-degree cone
    AquaticLance = 32231, // Boss->self, 4.0s cast, range 13 circle
    FoulWaters = 32229, // Boss->location, 3.0s cast, range 3 circle, AOE + spawns bubble
    Riptide = 32233, // Bubble->self, 1.0s cast, range 5 circle, pulls into bubble, dist 30 between centers
    WateryGrave = 32234, // Bubble->self, no cast, range 2 circle, voidzone, imprisons player until status runs out
    NavalRam = 32232, // GymnasiouEcheneis->player, no cast, single-target
    ProtolithicPuncture = 32228 // Boss->player, 5.0s cast, single-target
}

class PelagicCleaver(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.PelagicCleaver), new AOEShapeCone(40, 30.Degrees()));
class FoulWaters(BossModule module) : Components.PersistentVoidzoneAtCastTarget(module, 5, ActionID.MakeSpell(AID.FoulWaters), m => m.Enemies(OID.Bubble), 0);
class AquaticLance(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.AquaticLance), new AOEShapeCircle(13));
class ProtolithicPuncture(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.ProtolithicPuncture));

class GymnasiouTritonStates : StateMachineBuilder
{
    public GymnasiouTritonStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<PelagicCleaver>()
            .ActivateOnEnter<FoulWaters>()
            .ActivateOnEnter<AquaticLance>()
            .ActivateOnEnter<ProtolithicPuncture>()
            .Raw.Update = () => module.Enemies(OID.Boss).Concat(module.Enemies(OID.GymnasiouEcheneis)).All(e => e.IsDeadOrDestroyed);
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 909, NameID = 12006)]
public class GymnasiouTriton(WorldState ws, Actor primary) : BossModule(ws, primary, new(100, 100), new ArenaBoundsCircle(19))
{
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.GymnasiouEcheneis));
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        foreach (var e in hints.PotentialTargets)
        {
            e.Priority = (OID)e.Actor.OID switch
            {
                OID.GymnasiouEcheneis => 2,
                OID.Boss => 1,
                _ => 0
            };
        }
    }
}
