namespace BossMod.Shadowbringers.TreasureHunt.DungeonsOfLyheGhiah.Goliath;

public enum OID : uint
{
    Boss = 0x2BA5, //R=5.25
    GoliathsJavelin = 0x2BA6, //R=2.1
    DungeonQueen = 0x2A0A, // R0.84, icon 5, needs to be killed in order from 1 to 5 for maximum rewards
    DungeonGarlic = 0x2A08, // R0.84, icon 3, needs to be killed in order from 1 to 5 for maximum rewards
    DungeonTomato = 0x2A09, // R0.84, icon 4, needs to be killed in order from 1 to 5 for maximum rewards
    DungeonOnion = 0x2A06, // R0.84, icon 1, needs to be killed in order from 1 to 5 for maximum rewards
    DungeonEgg = 0x2A07, // R0.84, icon 2, needs to be killed in order from 1 to 5 for maximum rewards
    KeeperOfKeys = 0x2A05, // R3.23
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 872, // Boss/GoliathsJavelin/Mandragoras->player, no cast, single-target

    MechanicalBlow = 17873, // Boss->player, 5.0s cast, single-target
    Wellbore = 17874, // Boss->location, 7.0s cast, range 15 circle
    Fount = 17875, // Helper->location, 3.0s cast, range 4 circle
    Incinerate = 17876, // Boss->self, 5.0s cast, range 100 circle
    Accelerate = 17877, // Boss->players, 5.0s cast, range 6 circle
    Compress1 = 17879, // Boss->self, 2.5s cast, range 100 width 7 cross
    Compress2 = 17878, // GoliathsJavelin->self, 2.5s cast, range 100+R width 7 rect

    Pollen = 6452, // DungeonQueen->self, 3.5s cast, range 6+R circle
    TearyTwirl = 6448, // DungeonOnion->self, 3.5s cast, range 6+R circle
    HeirloomScream = 6451, // DungeonTomato->self, 3.5s cast, range 6+R circle
    PluckAndPrune = 6449, // DungeonEgg->self, 3.5s cast, range 6+R circle
    PungentPirouette = 6450, // DungeonGarlic->self, 3.5s cast, range 6+R circle
    Mash = 17852, // KeeperOfKeys->self, 2.5s cast, range 12+R width 4 rect
    Scoop = 17853, // KeeperOfKeys->self, 4.0s cast, range 15 120-degree cone
    Inhale = 17855, // KeeperOfKeys->self, no cast, range 20 120-degree cone, attract 25 between hitboxes, shortly before Spin
    Spin = 17854, // KeeperOfKeys->self, 2.5s cast, range 11 circle
    Telega = 9630 // BonusAdds->self, no cast, single-target, bonus adds disappear
}

class Wellbore(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Wellbore), new AOEShapeCircle(15));
class Compress1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Compress1), new AOEShapeCross(100, 3.5f));
class Compress2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Compress2), new AOEShapeRect(102.1f, 3.5f));
class Accelerate(BossModule module) : Components.StackWithCastTargets(module, ActionID.MakeSpell(AID.Accelerate), 6, 8, 8);
class Incinerate(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.Incinerate));
class Fount(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.Fount), 4);
class MechanicalBlow(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.MechanicalBlow));

class Spin(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Spin), new AOEShapeCircle(11));
class Mash(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Mash), new AOEShapeRect(15.23f, 2));
class Scoop(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Scoop), new AOEShapeCone(15, 60.Degrees()));

class Mandragoras(BossModule module, AID aid) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(aid), new AOEShapeCircle(6.84f));
class PluckAndPrune(BossModule module) : Mandragoras(module, AID.PluckAndPrune);
class TearyTwirl(BossModule module) : Mandragoras(module, AID.TearyTwirl);
class HeirloomScream(BossModule module) : Mandragoras(module, AID.HeirloomScream);
class PungentPirouette(BossModule module) : Mandragoras(module, AID.PungentPirouette);
class Pollen(BossModule module) : Mandragoras(module, AID.Pollen);

class GoliathStates : StateMachineBuilder
{
    public GoliathStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<Wellbore>()
            .ActivateOnEnter<Compress1>()
            .ActivateOnEnter<Compress2>()
            .ActivateOnEnter<Accelerate>()
            .ActivateOnEnter<Incinerate>()
            .ActivateOnEnter<MechanicalBlow>()
            .ActivateOnEnter<PluckAndPrune>()
            .ActivateOnEnter<TearyTwirl>()
            .ActivateOnEnter<HeirloomScream>()
            .ActivateOnEnter<PungentPirouette>()
            .ActivateOnEnter<Pollen>()
            .Raw.Update = () => Module.WorldState.Actors.Where(x => !x.IsAlly && x.IsTargetable).All(x => x.IsDeadOrDestroyed);
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 688, NameID = 8953)]
public class Goliath(WorldState ws, Actor primary) : BossModule(ws, primary, new(0, -390), new ArenaBoundsCircle(20))
{
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.GoliathsJavelin));
        Arena.Actors(Enemies(OID.DungeonEgg).Concat(Enemies(OID.DungeonTomato)).Concat(Enemies(OID.DungeonQueen).Concat(Enemies(OID.DungeonGarlic)).Concat(Enemies(OID.DungeonOnion))
        .Concat(Enemies(OID.KeeperOfKeys))), Colors.Vulnerable);
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        for (var i = 0; i < hints.PotentialTargets.Count; ++i)
        {
            var e = hints.PotentialTargets[i];
            e.Priority = (OID)e.Actor.OID switch
            {
                OID.DungeonOnion => 6,
                OID.DungeonEgg => 5,
                OID.DungeonGarlic => 4,
                OID.DungeonTomato => 3,
                OID.DungeonQueen or OID.KeeperOfKeys => 2,
                OID.GoliathsJavelin => 1,
                _ => 0
            };
        }
    }
}
