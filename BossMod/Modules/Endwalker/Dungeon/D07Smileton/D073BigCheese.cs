﻿namespace BossMod.Endwalker.Dungeon.D07Smileton.D073BigCheese;

public enum OID : uint
{
    Boss = 0x34D3, // R=19.6
    Helper = 0x233C,
    Bomb = 0x3585, // R0.500, x2
    ExcavationBomb1 = 0x38CF, // R0.500, x2
    ExcavationBomb2 = 0x3741, // R1.000, x0 (spawn during fight)
}

public enum AID : uint
{
    AutoAttack = 26444, // Boss->player, no cast, single-target
    DispenseExplosives = 27696, // Boss->self, 3.0s cast, single-target
    ElectricArc = 26451, // Boss->players, 5.0s cast, range 8 circle //Stack mechanic

    Excavated = 27698, // ExcavationBomb1->self, no cast, range 8 circle
    ExplosivePower = 27697, // Boss->self, 3.0s cast, single-target
    ExplosivesDistribution = 26446, // Boss->self, 3.0s cast, single-target

    IronKiss = 26445, // Bomb->location, 1.5s cast, range 16 circle

    RightDisassembler = 26447, // Boss->self, 8.0s cast, range 30 width 10 rect //Cleave
    LeftDisassembler = 26448, // Boss->self, 8.0s cast, range 30 width 10 rect //Cleave

    LevelingMissile1 = 26452, // Boss->self, 5.0s cast, single-target
    LevelingMissile2 = 26453, // Helper->player, 5.0s cast, range 6 circle //spread mechanic

    PiercingMissile = 26449, // Boss->player, 5.0s cast, single-target //Tankbuster

    UnknownAbility = 27700, // ExcavationBomb1->self, no cast, single-target
}

class Disassembler(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.RightDisassembler, (uint)AID.LeftDisassembler], new AOEShapeRect(30f, 5f));
class LevelingMissile2(BossModule module) : Components.SpreadFromCastTargets(module, (uint)AID.LevelingMissile2, 6f);
class ElectricArc(BossModule module) : Components.StackWithCastTargets(module, (uint)AID.ElectricArc, 6f, 4, 4);

class Excavated(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Excavated, 8f);

class IronKiss(BossModule module) : Components.SimpleAOEs(module, (uint)AID.IronKiss, 16f);

class PiercingMissile(BossModule module) : Components.SingleTargetCast(module, (uint)AID.PiercingMissile);

class D073BigCheeseStates : StateMachineBuilder
{
    public D073BigCheeseStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<LevelingMissile2>()
            .ActivateOnEnter<Disassembler>()
            .ActivateOnEnter<ElectricArc>()
            .ActivateOnEnter<PiercingMissile>()
            .ActivateOnEnter<IronKiss>()
            .ActivateOnEnter<Excavated>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 794, NameID = 10336)]
public class D073BigCheese(WorldState ws, Actor primary) : BossModule(ws, primary, new(-22, -44), new ArenaBoundsRect(14.5f, 10));
