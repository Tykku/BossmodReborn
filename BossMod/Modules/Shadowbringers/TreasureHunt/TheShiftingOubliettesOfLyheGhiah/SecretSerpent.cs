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
    KeeperOfKeys = 0x3034, // R3.23
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

    Mash = 21767, // KeeperOfKeys->self, 3.0s cast, range 13 width 4 rect
    Inhale = 21770, // KeeperOfKeys->self, no cast, range 20 120-degree cone, attract 25 between hitboxes, shortly before Spin
    Spin = 21769, // KeeperOfKeys->self, 4.0s cast, range 11 circle
    Scoop = 21768, // KeeperOfKeys->self, 4.0s cast, range 15 120-degree cone
    Pollen = 6452, // SecretQueen->self, 3.5s cast, range 6+R circle
    TearyTwirl = 6448, // SecretOnion->self, 3.5s cast, range 6+R circle
    HeirloomScream = 6451, // SecretTomato->self, 3.5s cast, range 6+R circle
    PluckAndPrune = 6449, // SecretEgg->self, 3.5s cast, range 6+R circle
    PungentPirouette = 6450, // SecretGarlic->self, 3.5s cast, range 6+R circle
    Telega = 9630 // Mandragoras->self, no cast, single-target, bonus adds disappear
}

class DouseVoidzone(BossModule module) : Components.PersistentVoidzone(module, 7.5f, m => m.Enemies(OID.WaterVoidzone).Where(z => z.EventState != 7), 0);
class Douse(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Douse), 8);
class FangsEnd(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.FangsEnd));
class Drench1(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Drench1), new AOEShapeCone(15.29f, 45.Degrees()));
class Drench2(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Drench2), new AOEShapeCone(13.45f, 45.Degrees()));
class ScaleRipple(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.ScaleRipple), 8);

abstract class Mandragoras(BossModule module, AID aid) : Components.SimpleAOEs(module, ActionID.MakeSpell(aid), 6.84f);
class PluckAndPrune(BossModule module) : Mandragoras(module, AID.PluckAndPrune);
class TearyTwirl(BossModule module) : Mandragoras(module, AID.TearyTwirl);
class HeirloomScream(BossModule module) : Mandragoras(module, AID.HeirloomScream);
class PungentPirouette(BossModule module) : Mandragoras(module, AID.PungentPirouette);
class Pollen(BossModule module) : Mandragoras(module, AID.Pollen);

class Spin(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Spin), 11);
class Mash(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Mash), new AOEShapeRect(13, 2));
class Scoop(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Scoop), new AOEShapeCone(15, 60.Degrees()));

class SecretSerpentStates : StateMachineBuilder
{
    public SecretSerpentStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<Douse>()
            .ActivateOnEnter<DouseVoidzone>()
            .ActivateOnEnter<FangsEnd>()
            .ActivateOnEnter<Drench1>()
            .ActivateOnEnter<Drench2>()
            .ActivateOnEnter<ScaleRipple>()
            .ActivateOnEnter<PluckAndPrune>()
            .ActivateOnEnter<TearyTwirl>()
            .ActivateOnEnter<HeirloomScream>()
            .ActivateOnEnter<PungentPirouette>()
            .ActivateOnEnter<Pollen>()
            .ActivateOnEnter<Spin>()
            .ActivateOnEnter<Mash>()
            .ActivateOnEnter<Scoop>()
            .Raw.Update = () => module.Enemies(SecretSerpent.All).All(x => x.IsDeadOrDestroyed);
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 745, NameID = 9776)]
public class SecretSerpent(WorldState ws, Actor primary) : THTemplate(ws, primary)
{
    private static readonly uint[] bonusAdds = [(uint)OID.SecretEgg, (uint)OID.SecretGarlic, (uint)OID.SecretOnion, (uint)OID.SecretTomato,
    (uint)OID.SecretQueen, (uint)OID.KeeperOfKeys];
    public static readonly uint[] All = [(uint)OID.Boss, (uint)OID.SerpentHatchling, .. bonusAdds];

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.SerpentHatchling));
        Arena.Actors(Enemies(bonusAdds), Colors.Vulnerable);
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        for (var i = 0; i < hints.PotentialTargets.Count; ++i)
        {
            var e = hints.PotentialTargets[i];
            e.Priority = (OID)e.Actor.OID switch
            {
                OID.SecretOnion => 6,
                OID.SecretEgg => 5,
                OID.SecretGarlic => 4,
                OID.SecretTomato => 3,
                OID.SecretQueen or OID.KeeperOfKeys => 2,
                OID.SerpentHatchling => 1,
                _ => 0
            };
        }
    }
}
