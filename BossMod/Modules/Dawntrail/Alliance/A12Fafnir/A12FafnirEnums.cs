namespace BossMod.Dawntrail.Alliance.A12Fafnir;

public enum OID : uint
{
    Boss = 0x41E5, // R17.0
    BitingWind = 0x41E8, // R1.0
    Darter = 0x41E9, // R2.25
    RavagingWind = 0x41E7, // R3.0
    SpatialRift = 0x1EBCD6, // R0.5
    FireVoidzone = 0x1EBCCB, // R0.5
    Helper2 = 0x41E6, // R1.0
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack1 = 40633, // Darter->player, no cast, single-target
    AutoAttack2 = 40860, // Boss->player, no cast, single-target

    AbsoluteTerrorVisual = 40845, // Boss->self, 6.0+1.4s cast, single-target
    AbsoluteTerror = 40846, // Helper->self, 7.4s cast, range 70 width 20 rect

    BalefulNailVisual1 = 40812, // Boss->location, no cast, single-target, four hit line AOE
    BalefulNailVisual2 = 40813, // Boss->location, no cast, single-target
    BalefulBreathVisual1 = 39922, // Boss->self, 8.0s cast, single-target
    BalefulBreathVisual2 = 39798, // Boss->self, no cast, single-target
    BalefulBreath1 = 40852, // Boss->self, no cast, range 70 width 6 rect
    BalefulBreath2 = 40853, // Boss->self, no cast, range 70 width 6 rect

    DarkMatterBlast = 40854, // Boss->self, 5.0s cast, range 70 circle

    GreatWhirlwindFirst1 = 39873, // RavagingWind->self, 4.5s cast, range 10 circle
    GreatWhirlwindFirst2 = 39874, // BitingWind->self, 4.5s cast, range 3 circle
    GreatWhirlwindrest1 = 40843, // Helper->self, no cast, range 9 circle
    GreatWhirlwindRest2 = 40844, // Helper->self, no cast, range 3 circle

    HorridRoarVisual = 40849, // Boss->self, 3.0s cast, single-target
    HorridRoarAOE = 40850, // Helper->location, 4.0s cast, range 4 circle
    HorridRoarSpread = 40851, // Helper->players, 5.0s cast, range 8 circle, spreadmarker

    HurricaneWingVisual = 40817, // Boss->self, 3.0+2.5s cast, single-target
    HurricaneWing1 = 40818, // Helper->self, no cast, range 70 circle
    HurricaneWing2 = 40819, // Helper->self, no cast, range 70 circle
    HurricaneWing3 = 40820, // Helper->self, no cast, range 70 circle
    HurricaneWing4 = 40821, // Helper->self, no cast, range 70 circle
    HurricaneWing5 = 40822, // Helper->self, no cast, range 70 circle
    HurricaneWing6 = 41302, // Helper->self, no cast, range 70 circle
    HurricaneWing7 = 41303, // Helper->self, no cast, range 70 circle
    HurricaneWing8 = 41304, // Helper->self, no cast, range 70 circle
    HurricaneWing9 = 41305, // Helper->self, no cast, range 70 circle

    // Concentric AOEs
    HurricaneWingConcentricA1 = 40829, // Helper->self, 5.0s cast, range 9 circle
    HurricaneWingConcentricA2 = 40830, // Helper->self, 7.0s cast, range 9-16 donut
    HurricaneWingConcentricA3 = 40831, // Helper->self, 9.0s cast, range 16-23 donut
    HurricaneWingConcentricA4 = 40832, // Helper->self, 11.0s cast, range 23-30 donut

    HurricaneWingConcentricB1 = 40839, // Helper->self, 4.0s cast, range 9 circle
    HurricaneWingConcentricB2 = 40840, // Helper->self, 6.0s cast, range 9-16 donut
    HurricaneWingConcentricB3 = 40841, // Helper->self, 8.0s cast, range 16-23 donut
    HurricaneWingConcentricB4 = 40842, // Helper->self, 10.0s cast, range 23-30 donut

    HurricaneWingConcentricC1 = 40834, // Helper->self, 4.0s cast, range 23-30 donut
    HurricaneWingConcentricC2 = 40835, // Helper->self, 6.0s cast, range 16-23 donut
    HurricaneWingConcentricC3 = 40836, // Helper->self, 8.0s cast, range 9-16 donut
    HurricaneWingConcentricC4 = 40837, // Helper->self, 10.0s cast, range 9 circle

    HurricaneWingConcentricD1 = 40824, // Helper->self, 5.0s cast, range 23-30 donut
    HurricaneWingConcentricD2 = 40825, // Helper->self, 7.0s cast, range 16-23 donut
    HurricaneWingConcentricD3 = 40826, // Helper->self, 9.0s cast, range 9-16 donut
    HurricaneWingConcentricD4 = 40827, // Helper->self, 11.0s cast, range 9 circle

    OffensivePostureVisual1 = 40811, // Boss->self, 8.0+1.0s cast, single-target, touchdown
    OffensivePostureVisual2 = 40814, // Boss->self, 8.0+1.1s cast, single-target, dragon breath
    OffensivePostureVisual3 = 40816, // Boss->self, 8.0+1.2s cast, single-target, spike flail

    SpikeFlail = 41114, // Helper->self, 9.0s cast, range 80 270-degree cone, rear cone aoe
    DragonBreath = 40815, // Helper->self, no cast, range 16-30 donut segment
    Touchdown = 41116, // Helper->self, 9.2s cast, range 24 circle, point blank AOE, likely slightly larger than Boss hitbox

    PestilentSphere = 40859, // Darter->player, 5.0s cast, single-target

    SharpSpikeVisual = 40855, // Boss->self, 5.0+1.0s cast, single-target, AOE tankbusters on all 3 tanks
    SharpSpike = 40856, // Helper->player, no cast, range 4 circle

    ShudderingEarth = 40857, // Boss->location, no cast, single-target

    WingedTerrorVisual = 40847, // Boss->self, 6.0+1.4s cast, single-target
    WingedTerror = 40848, // Helper->self, 7.4s cast, range 70 width 25 rect

    Venom = 40858 // Darter->self, 4.0s cast, range 30 120-degree cone
}

public enum IconID : uint
{
    LineStack = 568, // Boss->player
    Tankbuster = 342 // player->self
}
