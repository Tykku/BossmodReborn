namespace BossMod.Shadowbringers.TreasureHunt.ShiftingOubliettesOfLyheGhiah.SecretBasket;

public enum OID : uint
{
    Boss = 0x302D, //R=2.34
    SecretEchivore = 0x302E, //R=1.05
    SecretQueen = 0x3021, // R0.84, icon 5, needs to be killed in order from 1 to 5 for maximum rewards
    SecretGarlic = 0x301F, // R0.84, icon 3, needs to be killed in order from 1 to 5 for maximum rewards
    SecretTomato = 0x3020, // R0.84, icon 4, needs to be killed in order from 1 to 5 for maximum rewards
    SecretOnion = 0x301D, // R0.84, icon 1, needs to be killed in order from 1 to 5 for maximum rewards
    SecretEgg = 0x301E, // R0.84, icon 2, needs to be killed in order from 1 to 5 for maximum rewards
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 872, // Boss/SecretEchivore/Mandragoras->player, no cast, single-target

    HeavyStrikeVisual1 = 21698, // Boss->self, no cast, single-target
    HeavyStrikeVisual2 = 21723, // Boss->self, no cast, single-target
    HeavyStrike1 = 21724, // Helper->self, 4.0s cast, range 6+R 270-degree cone, donut segment
    HeavyStrike2 = 21725, // Helper->self, 4.0s cast, range 12+R 270-degree cone, donut segment
    HeavyStrike3 = 21726, // Helper->self, 4.9s cast, range 18+R 270-degree cone, donut segment

    PollenCorona = 21722, // Boss->self, 3.0s cast, range 8 circle
    StraightPunch = 21721, // Boss->player, 4.0s cast, single-target
    Leafcutter = 21732, // SecretEchivore->self, 3.0s cast, range 15 width 4 rect
    EarthCrusher = 21727, // Boss->self, 3.0s cast, single-target
    EarthCrusher2 = 21728, // Helper->self, 4.0s cast, range 10-20 donut
    SomersaultSlash = 21731, // SecretEchivore->player, no cast, single-target
    EarthquakeVisual = 21729, // Boss->self, 4.0s cast, single-target
    Earthquake = 21730, // Helper->self, no cast, range 20 circle

    Pollen = 6452, // SecretQueen->self, 3.5s cast, range 6+R circle
    TearyTwirl = 6448, // SecretOnion->self, 3.5s cast, range 6+R circle
    HeirloomScream = 6451, // SecretTomato->self, 3.5s cast, range 6+R circle
    PluckAndPrune = 6449, // SecretEgg->self, 3.5s cast, range 6+R circle
    PungentPirouette = 6450, // SecretGarlic->self, 3.5s cast, range 6+R circle
    Telega = 9630 // Mandragoras->self, no cast, single-target, bonus adds disappear
}

class Earthquake(BossModule module) : Components.RaidwideCastDelay(module, ActionID.MakeSpell(AID.EarthquakeVisual), ActionID.MakeSpell(AID.Earthquake), 1.2f);

class HeavyStrike1 : Components.SelfTargetedAOEs
{
    public HeavyStrike1(BossModule module) : base(module, ActionID.MakeSpell(AID.HeavyStrike1), new AOEShapeDonutSector(0.5f, 6.5f, 135.Degrees()))
    {
        Color = Colors.Danger;
    }
}

class HeavyStrike2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.HeavyStrike2), new AOEShapeDonutSector(6.5f, 12.5f, 135.Degrees()))
{
    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        base.OnCastFinished(caster, spell);
        Color = (AID)spell.Action.ID == AID.HeavyStrike1 ? Colors.Danger : Colors.AOE;
    }
}

class HeavyStrike3(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.HeavyStrike3), new AOEShapeDonutSector(12.5f, 18.5f, 135.Degrees()))
{
    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        base.OnCastFinished(caster, spell);
        Color = (AID)spell.Action.ID == AID.HeavyStrike2 ? Colors.Danger : Colors.AOE;
    }
}

class PollenCorona(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.PollenCorona), new AOEShapeCircle(8));
class StraightPunch(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.StraightPunch));
class Leafcutter(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Leafcutter), new AOEShapeRect(15, 2));
class EarthCrusher(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.EarthCrusher2), new AOEShapeDonut(10, 20));

class Mandragoras(BossModule module, AID aid) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(aid), new AOEShapeCircle(6.84f));
class PluckAndPrune(BossModule module) : Mandragoras(module, AID.PluckAndPrune);
class TearyTwirl(BossModule module) : Mandragoras(module, AID.TearyTwirl);
class HeirloomScream(BossModule module) : Mandragoras(module, AID.HeirloomScream);
class PungentPirouette(BossModule module) : Mandragoras(module, AID.PungentPirouette);
class Pollen(BossModule module) : Mandragoras(module, AID.Pollen);

class SecretBasketStates : StateMachineBuilder
{
    public SecretBasketStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<Earthquake>()
            .ActivateOnEnter<HeavyStrike1>()
            .ActivateOnEnter<HeavyStrike2>()
            .ActivateOnEnter<HeavyStrike3>()
            .ActivateOnEnter<PollenCorona>()
            .ActivateOnEnter<StraightPunch>()
            .ActivateOnEnter<Leafcutter>()
            .ActivateOnEnter<EarthCrusher>()
            .ActivateOnEnter<PluckAndPrune>()
            .ActivateOnEnter<TearyTwirl>()
            .ActivateOnEnter<HeirloomScream>()
            .ActivateOnEnter<PungentPirouette>()
            .ActivateOnEnter<Pollen>()
            .Raw.Update = () => module.Enemies(OID.SecretEchivore).Concat([module.PrimaryActor]).Concat(module.Enemies(OID.SecretEgg)).Concat(module.Enemies(OID.SecretQueen))
            .Concat(module.Enemies(OID.SecretOnion)).Concat(module.Enemies(OID.SecretGarlic)).Concat(module.Enemies(OID.SecretTomato)).All(e => e.IsDeadOrDestroyed);
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 745, NameID = 9784)]
public class SecretBasket(WorldState ws, Actor primary) : BossModule(ws, primary, new(100, 100), new ArenaBoundsCircle(19))
{
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.SecretEchivore));
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
                OID.SecretEchivore => 2,
                OID.Boss => 1,
                _ => 0
            };
        }
    }
}
