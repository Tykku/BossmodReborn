namespace BossMod.Stormblood.TreasureHunt.HiddenCalansOfUznair.Airavata;

public enum OID : uint
{
    Boss = 0x1FCC, // R4.75
    CanalAnila = 0x1F10, // R1.92
    CanalAnala = 0x1F0C, // R1.6
    CanalLightningHomunculus = 0x1F13, // R1.6
    CanalWindHomunculus = 0x1F11, // R1.6
    CanalFireHomunculus = 0x1F0D, // R1.6
    CanalIceHomunculus = 0x1F0F, // R1.6
    GoldenApa = 0x1FEC, // R3.12
    GoldenDhara = 0x1FEB, // R1.95

    CanalQueen = 0x1FD1, // R0.840, x0 (spawn during fight)
    CanalEgg = 0x1FCE, // R0.840, x0 (spawn during fight)
    CanalTomato = 0x1FD0, // R0.840, x0 (spawn during fight)
    CanalGarlic = 0x1FCF, // R0.840, x0 (spawn during fight)
    CanalOnion = 0x1FCD, // R0.840, x0 (spawn during fight)

    NamazuStickywhisker = 0x2063, // R0.54
    Abharamu = 0x2064, // R3.42
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack1 = 870, // CanalAnala->player, no cast, single-target
    AutoAttack2 = 872, // Boss/Abharamu/GoldenDhara->player, no cast, single-target

    SpinBoss = 9892, // Boss->self, 3.5s cast, range 20 120-degree cone
    HurlBoss = 9891, // Boss->location, 3.0s cast, range 6 circle
    Buffet = 9893, // Boss->player, 3.0s cast, single-target, tankbuster
    BarbarousScream = 9894, // Boss->self, 3.0s cast, range 14 circle

    Blizzard = 967, // CanalIceHomunculus->player, 1.0s cast, single-target
    Fire = 966, // CanalFireHomunculus->player, 1.0s cast, single-target
    Aero = 969, // CanalWindHomunculus/CanalAnila->player, 1.0s cast, single-target
    Thunder = 968, // CanalLightningHomunculus->player, 1.0s cast, single-target
    Water = 971, // GoldenApa->player, 1.0s cast, single-target
    RingOfFire = 8629, // CanalAnala->location, 3.0s cast, range 5 circle
    DoubleSmash = 618, // GoldenDhara->self, 3.0s cast, range 6+R 120-degree cone
    Abacinate = 8628, // CanalAnala->players, no cast, single-target
    AncientAero = 8727, // CanalAnila->self, 3.0s cast, range 12+R width 4 rect
    StraightPunch = 8659, // GoldenDhara->player, no cast, single-target
    StoneII = 8660, // GoldenDhara->player, 4.0s cast, single-target

    AbharamuActivate = 9636, // Abharamu->self, no cast, single-target
    Spin = 8599, // Abharamu->self, no cast, range 6+R 120-degree cone
    RaucousScritch = 8598, // Abharamu->self, 2.5s cast, range 5+R 120-degree cone
    Hurl = 5352, // Abharamu->location, 3.0s cast, range 6 circle

    PungentPirouette = 6450, // 1FCF->self, 3.5s cast, range 6+R circle
    PluckAndPrune = 6449, // 1FCE->self, 3.5s cast, range 6+R circle
    HeirloomScream = 6451, // 1FD0->self, 3.5s cast, range 6+R circle
    Pollen = 6452, // 1FD1->self, 3.5s cast, range 6+R circle
    TearyTwirl = 6448, // 1FCD->self, 3.5s cast, range 6+R circle

    Telega = 9630 // Mandragoras/Abharamu/NamazuStickywhisker->self, no cast, single-target, bonus adds disappear
}

abstract class Hurl(BossModule module, AID aid) : Components.SimpleAOEs(module, ActionID.MakeSpell(aid), 6);
class HurlBoss(BossModule module) : Hurl(module, AID.HurlBoss);
class HurlBonusAdd(BossModule module) : Hurl(module, AID.Hurl);

class SpinBoss(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.SpinBoss), new AOEShapeCone(20, 60.Degrees()));
class BarbarousScream(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BarbarousScream), new AOEShapeCircle(14));
class Buffet(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.Buffet));

class DoubleSmash(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.DoubleSmash), new AOEShapeCone(7.95f, 60.Degrees()));
class AncientAero(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.AncientAero), new AOEShapeRect(13.6f, 2));
class RingOfFire(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.RingOfFire), 5);
class StoneII(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.StoneII));

class RaucousScritch(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.RaucousScritch), new AOEShapeCone(8.42f, 30.Degrees()));
class Spin(BossModule module) : Components.Cleave(module, ActionID.MakeSpell(AID.Spin), new AOEShapeCone(9.42f, 60.Degrees()), (uint)OID.Abharamu);

abstract class Mandragoras(BossModule module, AID aid) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(aid), new AOEShapeCircle(6.84f));
class PluckAndPrune(BossModule module) : Mandragoras(module, AID.PluckAndPrune);
class TearyTwirl(BossModule module) : Mandragoras(module, AID.TearyTwirl);
class HeirloomScream(BossModule module) : Mandragoras(module, AID.HeirloomScream);
class PungentPirouette(BossModule module) : Mandragoras(module, AID.PungentPirouette);
class Pollen(BossModule module) : Mandragoras(module, AID.Pollen);

class AiravataStates : StateMachineBuilder
{
    public AiravataStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<HurlBoss>()
            .ActivateOnEnter<SpinBoss>()
            .ActivateOnEnter<BarbarousScream>()
            .ActivateOnEnter<Buffet>()
            .ActivateOnEnter<HurlBonusAdd>()
            .ActivateOnEnter<RaucousScritch>()
            .ActivateOnEnter<Spin>()
            .ActivateOnEnter<PluckAndPrune>()
            .ActivateOnEnter<TearyTwirl>()
            .ActivateOnEnter<HeirloomScream>()
            .ActivateOnEnter<PungentPirouette>()
            .ActivateOnEnter<Pollen>()
            .Raw.Update = () => module.Enemies(Airavata.All).All(x => x.IsDeadOrDestroyed);
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 276, NameID = 6853)]
public class Airavata(WorldState ws, Actor primary) : FinalRoomArena(ws, primary)
{
    private static readonly uint[] bonusAdds = [(uint)OID.CanalEgg, (uint)OID.CanalGarlic, (uint)OID.CanalOnion, (uint)OID.CanalTomato,
    (uint)OID.CanalQueen, (uint)OID.Abharamu, (uint)OID.NamazuStickywhisker];
    private static readonly uint[] rest = [(uint)OID.Boss, (uint)OID.CanalLightningHomunculus, (uint)OID.CanalIceHomunculus, (uint)OID.CanalFireHomunculus, (uint)OID.CanalWindHomunculus,
    (uint)OID.CanalAnala, (uint)OID.CanalAnila, (uint)OID.GoldenApa, (uint)OID.GoldenDhara];
    public static readonly uint[] All = [.. rest, .. bonusAdds];

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actors(Enemies(rest));
        Arena.Actors(Enemies(bonusAdds), Colors.Vulnerable);
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        for (var i = 0; i < hints.PotentialTargets.Count; ++i)
        {
            var e = hints.PotentialTargets[i];
            e.Priority = (OID)e.Actor.OID switch
            {
                OID.CanalOnion => 7,
                OID.CanalEgg => 6,
                OID.CanalGarlic => 5,
                OID.CanalTomato => 4,
                OID.CanalQueen or OID.NamazuStickywhisker => 3,
                OID.Abharamu => 2,
                OID.CanalFireHomunculus or OID.CanalIceHomunculus or OID.CanalWindHomunculus or OID.CanalLightningHomunculus or OID.CanalAnala or OID.CanalAnila or
                OID.GoldenApa or OID.GoldenDhara => 2,
                _ => 0
            };
        }
    }
}
