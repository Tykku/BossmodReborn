namespace BossMod.Shadowbringers.TreasureHunt.ShiftingOubliettesOfLyheGhiah.SecretSerpent;

public enum OID : uint
{
    Boss = 0x3025, //R=5.29
    SerpentHatchling = 0x3026, //R=3.45
    WaterVoidzone = 0x1EA7D5,
    SecretQueen = 0x3021, // R0.84, icon 5, needs to be killed in order from 1 to 5 for maximum rewards
    SecretGarlic = 0x301F, // R0.84, icon 3, needs to be killed in order from 1 to 5 for maximum rewards
    SecretTomato = 0x3020, // R0.84, icon 4, needs to be killed in order from 1 to 5 for maximum rewards
    SecretOnion = 0x301D, // R0.84, icon 1, needs to be killed in order from 1 to 5 for maximum rewards
    SecretEgg = 0x301E, // R0.84, icon 2, needs to be killed in order from 1 to 5 for maximum rewards
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack1 = 870, // Boss/SerpentHatchling->player, no cast, single-target
    AutoAttack2 = 872, // Mandragoras->player, no cast, single-target

    Douse = 21701, // Boss->location, 3.0s cast, range 8 circle
    Drench1 = 21700, // Boss->self, 3.0s cast, range 10+R 90-degree cone
    Drench2 = 22771, // SerpentHatchling->self, 3.0s cast, range 10+R 90-degree cone
    FangsEnd = 21699, // Boss->player, 4.0s cast, single-target
    ScaleRipple = 21702, // Boss->self, 2.5s cast, range 8 circle

    Pollen = 6452, // SecretQueen->self, 3.5s cast, range 6+R circle
    TearyTwirl = 6448, // SecretOnion->self, 3.5s cast, range 6+R circle
    HeirloomScream = 6451, // SecretTomato->self, 3.5s cast, range 6+R circle
    PluckAndPrune = 6449, // SecretEgg->self, 3.5s cast, range 6+R circle
    PungentPirouette = 6450, // SecretGarlic->self, 3.5s cast, range 6+R circle
    Telega = 9630 // Mandragoras->self, no cast, single-target, bonus adds disappear
}

class Douse(BossModule module) : Components.PersistentVoidzoneAtCastTarget(module, 8, ActionID.MakeSpell(AID.Douse), m => m.Enemies(OID.WaterVoidzone).Where(z => z.EventState != 7), 0);
class FangsEnd(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.FangsEnd));
class Drench1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Drench1), new AOEShapeCone(15.29f, 45.Degrees()));
class Drench2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Drench2), new AOEShapeCone(13.45f, 45.Degrees()));
class ScaleRipple(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.ScaleRipple), new AOEShapeCircle(8));

class Mandragoras(BossModule module, AID aid) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(aid), new AOEShapeCircle(6.84f));
class PluckAndPrune(BossModule module) : Mandragoras(module, AID.PluckAndPrune);
class TearyTwirl(BossModule module) : Mandragoras(module, AID.TearyTwirl);
class HeirloomScream(BossModule module) : Mandragoras(module, AID.HeirloomScream);
class PungentPirouette(BossModule module) : Mandragoras(module, AID.PungentPirouette);
class Pollen(BossModule module) : Mandragoras(module, AID.Pollen);

class SecretSerpentStates : StateMachineBuilder
{
    public SecretSerpentStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<Douse>()
            .ActivateOnEnter<FangsEnd>()
            .ActivateOnEnter<Drench1>()
            .ActivateOnEnter<Drench2>()
            .ActivateOnEnter<ScaleRipple>()
            .ActivateOnEnter<PluckAndPrune>()
            .ActivateOnEnter<TearyTwirl>()
            .ActivateOnEnter<HeirloomScream>()
            .ActivateOnEnter<PungentPirouette>()
            .ActivateOnEnter<Pollen>()
            .Raw.Update = () => module.Enemies(OID.SerpentHatchling).Concat([module.PrimaryActor]).Concat(module.Enemies(OID.SecretEgg)).Concat(module.Enemies(OID.SecretQueen))
            .Concat(module.Enemies(OID.SecretOnion)).Concat(module.Enemies(OID.SecretGarlic)).Concat(module.Enemies(OID.SecretTomato)).All(e => e.IsDeadOrDestroyed);
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 745, NameID = 9776)]
public class SecretSerpent(WorldState ws, Actor primary) : BossModule(ws, primary, new(100, 100), new ArenaBoundsCircle(19))
{
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.SerpentHatchling));
        Arena.Actors(Enemies(OID.SecretEgg).Concat(Enemies(OID.SecretTomato)).Concat(Enemies(OID.SecretQueen)).Concat(Enemies(OID.SecretGarlic)).Concat(Enemies(OID.SecretOnion)), Colors.Vulnerable);
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        foreach (var e in hints.PotentialTargets)
        {
            e.Priority = (OID)e.Actor.OID switch
            {
                OID.SecretOnion => 7,
                OID.SecretEgg => 6,
                OID.SecretGarlic => 5,
                OID.SecretTomato => 4,
                OID.SecretQueen => 3,
                OID.SerpentHatchling => 2,
                OID.Boss => 1,
                _ => 0
            };
        }
    }
}
