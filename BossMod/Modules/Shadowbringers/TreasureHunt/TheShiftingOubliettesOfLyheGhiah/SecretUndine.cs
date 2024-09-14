namespace BossMod.Shadowbringers.TreasureHunt.ShiftingOubliettesOfLyheGhiah.SecretUndine;

public enum OID : uint
{
    Boss = 0x3011, //R=3.6
    AqueousAether = 0x3013, //R=1.12
    Bubble = 0x3012, //R=1.3, untargetable 
    KeeperOfKeys = 0x3034, // R3.23
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack1 = 23186, // Boss/AqueousAether->player, no cast, single-target
    AutoAttack2 = 872, // KeeperOfKeys->player, no cast, single-target

    Hydrowhirl = 21658, // Boss->self, 3.0s cast, range 8 circle
    Hypnowave = 21659, // Boss->self, 3.0s cast, range 30 120-degree cone, causes sleep
    HydrotaphVisual = 21661, // Boss->self, 4.0s cast, single-target
    Hydrotaph = 21662, // Helper->self, 4.0s cast, range 40 circle
    Hydrofan = 21663, // Bubble->self, 5.0s cast, range 44 30-degree cone
    Hydropins = 21660, // Boss->self, 2.5s cast, range 12 width 4 rect
    AquaGlobe = 21664, // AqueousAether->location, 3.0s cast, range 8 circle

    Telega = 9630, // KeeperOfKeys->self, no cast, single-target, bonus adds disappear
    Mash = 21767, // KeeperOfKeys->self, 3.0s cast, range 13 width 4 rect
    Inhale = 21770, // KeeperOfKeys->self, no cast, range 20 120-degree cone, attract 25 between hitboxes, shortly before Spin
    Spin = 21769, // KeeperOfKeys->self, 4.0s cast, range 11 circle
    Scoop = 21768 // KeeperOfKeys->self, 4.0s cast, range 15 120-degree cone
}

class Hydrofan(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Hydrofan), new AOEShapeCone(44, 15.Degrees()));
class Hypnowave(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Hypnowave), new AOEShapeCone(30, 60.Degrees()));
class Hydropins(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Hydropins), new AOEShapeRect(12, 2));
class AquaGlobe(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.AquaGlobe), 8);
class Hydrowhirl(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Hydrowhirl), new AOEShapeCircle(8));
class Hydrotaph(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.Hydrotaph));
class Spin(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Spin), new AOEShapeCircle(11));
class Mash(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Mash), new AOEShapeRect(13, 2));
class Scoop(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Scoop), new AOEShapeCone(15, 60.Degrees()));

class SecretUndineStates : StateMachineBuilder
{
    public SecretUndineStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<Hydrofan>()
            .ActivateOnEnter<Hypnowave>()
            .ActivateOnEnter<Hydropins>()
            .ActivateOnEnter<AquaGlobe>()
            .ActivateOnEnter<Hydrowhirl>()
            .ActivateOnEnter<Hydrotaph>()
            .ActivateOnEnter<Spin>()
            .ActivateOnEnter<Mash>()
            .ActivateOnEnter<Scoop>()
            .Raw.Update = () => module.Enemies(OID.AqueousAether).Concat([module.PrimaryActor]).Concat(module.Enemies(OID.KeeperOfKeys)).All(e => e.IsDeadOrDestroyed);
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 745, NameID = 9790)]
public class SecretUndine(WorldState ws, Actor primary) : BossModule(ws, primary, new(100, 100), new ArenaBoundsCircle(19))
{
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.AqueousAether));
        Arena.Actors(Enemies(OID.KeeperOfKeys), Colors.Vulnerable);
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        foreach (var e in hints.PotentialTargets)
        {
            e.Priority = (OID)e.Actor.OID switch
            {
                OID.KeeperOfKeys => 3,
                OID.AqueousAether => 2,
                OID.Boss => 1,
                _ => 0
            };
        }
    }
}
