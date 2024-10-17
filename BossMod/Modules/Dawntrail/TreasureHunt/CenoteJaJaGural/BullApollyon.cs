namespace BossMod.Dawntrail.TreasureHunt.CenoteJaJaGural.BullApollyon;

public enum OID : uint
{
    Boss = 0x4305, // R7
    TuraliOnion = 0x4300, // R0.84, icon 1, needs to be killed in order from 1 to 5 for maximum rewards
    TuraliEggplant = 0x4301, // R0.84, icon 2, needs to be killed in order from 1 to 5 for maximum rewards
    TuraliGarlic = 0x4302, // R0.84, icon 3, needs to be killed in order from 1 to 5 for maximum rewards
    TuraliTomato = 0x4303, // R0.84, icon 4, needs to be killed in order from 1 to 5 for maximum rewards
    TuligoraQueen = 0x4304, // R0.84, icon 5, needs to be killed in order from 1 to 5 for maximum rewards
    UolonOfFortune = 0x42FF, // R3.5
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 870, // Boss->player, no cast, single-target
    Teleport = 38334, // Boss->location, no cast, single-target

    Blade = 38261, // Boss->player, 5s cast, single-target tankbuster

    PyreburstVisual = 38263, // Helper->self, no cast, range 60 circle visual
    Pyreburst = 38262, // Boss->self, 5s cast, single-target raidwide

    FlameBladeVisual = 38248, // Boss->self, no cast, single-target
    FlameBlade1 = 38249, // Boss->self, 4s cast, range 40 width 10 rect
    FlameBlade2 = 38250, // Helper->self, 11s cast, range 40 width 10 rect
    FlameBlade3 = 38251, // Helper->self, 2.5s cast, range 40 width 5 rect

    BlazingBreathVisual = 38257, // Boss->self, 2.3+0.7s cast, single-target visual
    BlazingBreath = 38258, // Helper->player, 3s cast, range 44 width 10 rect

    CrossfireBlade1 = 38253, // Boss->self, 4s cast, range 20 width 10 cross
    CrossfireBlade2 = 38254, // Helper->self, 11s cast, range 20 width 10 cross
    CrossfireBlade3 = 38255, // Helper->self, 2.5s cast, range 40 width 5 rect

    BlazingBlastVisual = 38259, // Boss->self, 3s cast, single-target visual
    BlazingBlast = 38260, // Helper->location, 3s cast, range 6 circle
}

class Blade(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.Blade));
class Pyreburst(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.Pyreburst));

abstract class RectWide(BossModule module, AID aid) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(aid), new AOEShapeRect(40, 5, 40));
class FlameBlade1(BossModule module) : RectWide(module, AID.FlameBlade1);
class FlameBlade2(BossModule module) : RectWide(module, AID.FlameBlade2);

abstract class RectNarrow(BossModule module, AID aid) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(aid), new AOEShapeRect(40, 2.5f, 40));
class CrossfireBlade3(BossModule module) : RectNarrow(module, AID.CrossfireBlade3);
class FlameBlade3(BossModule module) : RectNarrow(module, AID.FlameBlade3);

abstract class Crosses(BossModule module, AID aid) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(aid), new AOEShapeCross(20, 5));
class CrossfireBlade1(BossModule module) : Crosses(module, AID.CrossfireBlade1);
class CrossfireBlade2(BossModule module) : Crosses(module, AID.CrossfireBlade2);

class BlazingBreath(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BlazingBreath), new AOEShapeRect(44, 5));
class BlazingBlast(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.BlazingBlast), 6);

class BullApollyonStates : StateMachineBuilder
{
    public BullApollyonStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<Blade>()
            .ActivateOnEnter<Pyreburst>()
            .ActivateOnEnter<FlameBlade1>()
            .ActivateOnEnter<FlameBlade2>()
            .ActivateOnEnter<FlameBlade3>()
            .ActivateOnEnter<BlazingBreath>()
            .ActivateOnEnter<CrossfireBlade1>()
            .ActivateOnEnter<CrossfireBlade2>()
            .ActivateOnEnter<CrossfireBlade3>()
            .ActivateOnEnter<BlazingBlast>()
            .Raw.Update = () => module.Enemies(OID.TuraliTomato).Concat([module.PrimaryActor]).Concat(module.Enemies(OID.TuraliEggplant)).Concat(module.Enemies(OID.TuligoraQueen))
            .Concat(module.Enemies(OID.TuraliOnion)).Concat(module.Enemies(OID.TuraliGarlic)).Concat(module.Enemies(OID.UolonOfFortune)).All(e => e.IsDeadOrDestroyed);
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Contributed, Contributors = "Kismet", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 993, NameID = 13247)]
public class BullApollyon(WorldState ws, Actor primary) : BossModule(ws, primary, new(0, -372), new ArenaBoundsCircle(20))
{
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.TuraliEggplant).Concat(Enemies(OID.TuraliTomato)).Concat(Enemies(OID.TuligoraQueen)).Concat(Enemies(OID.TuraliGarlic))
        .Concat(Enemies(OID.TuraliOnion)).Concat(Enemies(OID.UolonOfFortune)), Colors.Vulnerable);
    }
}
