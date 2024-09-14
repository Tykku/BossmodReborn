namespace BossMod.Endwalker.TreasureHunt.ShiftingGymnasionAgonon.GymnasiouLeon;

public enum OID : uint
{
    Boss = 0x3D27, //R=5.95
    GymnasiouLeonMikros = 0x3D28, //R=3.5
    GymnasiouLyssa = 0x3D4E, //R=3.75
    GymnasticGarlic = 0x3D51, // R0.84, icon 3, needs to be killed in order from 1 to 5 for maximum rewards
    GymnasticQueen = 0x3D53, // R0.84, icon 5, needs to be killed in order from 1 to 5 for maximum rewards
    GymnasticEggplant = 0x3D50, // R0.84, icon 2, needs to be killed in order from 1 to 5 for maximum rewards
    GymnasticOnion = 0x3D4F, // R0.84, icon 1, needs to be killed in order from 1 to 5 for maximum rewards
    GymnasticTomato = 0x3D52, // R0.84, icon 4, needs to be killed in order from 1 to 5 for maximum rewards
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 870, // Boss/GymnasiouLyssa/GymnasiouLeonMikros->player, no cast, single-target

    InfernoBlast = 32204, // Boss->self, 3.5s cast, range 46 width 20 rect
    Roar = 32201, // Boss->self, 3.0s cast, range 12 circle
    Pounce = 32200, // Boss->player, 5.0s cast, single-target
    MagmaChamberVisual = 32202, // Boss->self, 3.0s cast, single-target
    MagmaChamber = 32203, // Helper->location, 3.0s cast, range 8 circle
    FlareStarVisual = 32815, // Boss->self, 3.0s cast, single-target
    FlareStar = 32816, // Helper->self, 7.0s cast, range 40 circle, AOE with dmg fall off, damage seems to stop falling after about range 10-12
    MarkOfTheBeast = 32205, // GymnasiouLeonMikros->self, 3.0s cast, range 8 120-degree cone

    PluckAndPrune = 32302, // GymnasticEggplant->self, 3.5s cast, range 7 circle
    Pollen = 32305, // GymnasticQueen->self, 3.5s cast, range 7 circle
    HeirloomScream = 32304, // GymnasticTomato->self, 3.5s cast, range 7 circle
    PungentPirouette = 32303, // GymnasticGarlic->self, 3.5s cast, range 7 circle
    TearyTwirl = 32301, // GymnasticOnion->self, 3.5s cast, range 7 circle
    HeavySmash = 32317, // GymnasiouLyssa->location, 3.0s cast, range 6 circle
    Telega = 9630 // Mandragoras/GymnasiouLyssa->self, no cast, single-target, bonus add disappear
}

class InfernoBlast(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.InfernoBlast), new AOEShapeRect(46, 20));
class Roar(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Roar), new AOEShapeCircle(12));
class FlareStar(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.FlareStar), new AOEShapeCircle(12));
class MarkOfTheBeast(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.MarkOfTheBeast), new AOEShapeCone(8, 60.Degrees()));
class Pounce(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.Pounce));
class MagmaChamber(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.MagmaChamber), 8);

class Mandragoras(BossModule module, AID aid) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(aid), new AOEShapeCircle(7));
class PluckAndPrune(BossModule module) : Mandragoras(module, AID.PluckAndPrune);
class TearyTwirl(BossModule module) : Mandragoras(module, AID.TearyTwirl);
class HeirloomScream(BossModule module) : Mandragoras(module, AID.HeirloomScream);
class PungentPirouette(BossModule module) : Mandragoras(module, AID.PungentPirouette);
class Pollen(BossModule module) : Mandragoras(module, AID.Pollen);

class HeavySmash(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.HeavySmash), 6);

class GymnasiouLeonStates : StateMachineBuilder
{
    public GymnasiouLeonStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<InfernoBlast>()
            .ActivateOnEnter<Roar>()
            .ActivateOnEnter<FlareStar>()
            .ActivateOnEnter<MarkOfTheBeast>()
            .ActivateOnEnter<Pounce>()
            .ActivateOnEnter<MagmaChamber>()
            .ActivateOnEnter<HeavySmash>()
            .ActivateOnEnter<PluckAndPrune>()
            .ActivateOnEnter<TearyTwirl>()
            .ActivateOnEnter<HeirloomScream>()
            .ActivateOnEnter<PungentPirouette>()
            .ActivateOnEnter<Pollen>()
            .Raw.Update = () => module.Enemies(OID.GymnasiouLeonMikros).Concat([module.PrimaryActor]).Concat(module.Enemies(OID.GymnasticEggplant))
            .Concat(module.Enemies(OID.GymnasticQueen)).Concat(module.Enemies(OID.GymnasticOnion)).Concat(module.Enemies(OID.GymnasticGarlic))
            .Concat(module.Enemies(OID.GymnasticTomato)).Concat(module.Enemies(OID.GymnasiouLyssa)).All(e => e.IsDeadOrDestroyed);
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 909, NameID = 11997)]
public class GymnasiouLeon(WorldState ws, Actor primary) : BossModule(ws, primary, new(100, 100), new ArenaBoundsCircle(19))
{
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.GymnasiouLeonMikros));
        Arena.Actors(Enemies(OID.GymnasticEggplant).Concat(Enemies(OID.GymnasticTomato)).Concat(Enemies(OID.GymnasticQueen)).Concat(Enemies(OID.GymnasticGarlic))
        .Concat(Enemies(OID.GymnasticOnion)).Concat(Enemies(OID.GymnasiouLyssa)), Colors.Vulnerable);
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        foreach (var e in hints.PotentialTargets)
        {
            e.Priority = (OID)e.Actor.OID switch
            {
                OID.GymnasticOnion => 7,
                OID.GymnasticEggplant => 6,
                OID.GymnasticGarlic => 5,
                OID.GymnasticTomato => 4,
                OID.GymnasticQueen or OID.GymnasiouLyssa => 3,
                OID.GymnasiouLeonMikros => 2,
                OID.Boss => 1,
                _ => 0
            };
        }
    }
}
