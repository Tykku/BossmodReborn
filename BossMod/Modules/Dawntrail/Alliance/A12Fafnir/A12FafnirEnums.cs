namespace BossMod.Dawntrail.Alliance.A12Fafnir;

public enum OID : uint
{
    Actor1e8f2f = 0x1E8F2F, // R0.500, x1, EventObj type
    Actor1e8fb8 = 0x1E8FB8, // R2.000, x1, EventObj type
    Actor1ebccb = 0x1EBCCB, // R0.500, x4, EventObj type
    BakoolJaJa = 0x4697, // R3.150, x1
    BitingWind = 0x41E8, // R1.000, x6
    Boss = 0x41E5, // R17.000, x1
    Darter = 0x41E9, // R2.250, x0 (spawn during fight)
    Helper = 0x233C, // R0.500, x43, Helper type
    RavagingWind = 0x41E7, // R3.000, x3
    SpatialRift = 0x1EBCD6, // R0.500, x1, EventObj type
    Teleporter = 0x1EBCCE, // R0.500, x1, EventObj type
    Unknown = 0x41E6, // R1.000, x3
}

public enum AID : uint
{
    AutoAttack1 = 40633, // Darter->player, no cast, single-target
    AutoAttack2 = 40860, // Boss->player, no cast, single-target

    AbsoluteTerror1 = 40845, // Boss->self, 6.0+1.4s cast, single-target
    AbsoluteTerror2 = 40846, // Helper->self, 7.4s cast, range 70 width 20 rect

    BalefulNail = 40812, // Boss->location, no cast, single-target // Four hit line AOE
    DarkMatterBlast = 40854, // Boss->self, 5.0s cast, range 70 circle

    GreatWhirlwind1 = 39873, // RavagingWind->self, 4.5s cast, range 10 circle
    GreatWhirlwind2 = 39874, // BitingWind->self, 4.5s cast, range 3 circle
    GreatWhirlwind3 = 40843, // Helper->self, no cast, range 9 circle
    GreatWhirlwind4 = 40844, // Helper->self, no cast, range 3 circle

    HorridRoar1 = 40849, // Boss->self, 3.0s cast, single-target
    HorridRoar2 = 40850, // Helper->location, 4.0s cast, range 4 circle
    HorridRoar3 = 40851, // Helper->players, 5.0s cast, range 8 circle // Spreadmarker

    HurricaneWing1 = 40817, // Boss->self, 3.0+2.5s cast, single-target
    HurricaneWing2 = 40818, // Helper->self, no cast, range 70 circle
    HurricaneWing3 = 40819, // Helper->self, no cast, range 70 circle
    HurricaneWing4 = 40820, // Helper->self, no cast, range 70 circle
    HurricaneWing5 = 40821, // Helper->self, no cast, range 70 circle
    HurricaneWing6 = 40822, // Helper->self, no cast, range 70 circle

    // Concentric AOEs
    HurricaneWing7 = 40829, // Helper->self, 5.0s cast, range 9 circle
    HurricaneWing8 = 40830, // Helper->self, 7.0s cast, range 9-16 donut
    HurricaneWing9 = 40831, // Helper->self, 9.0s cast, range 16-23 donut
    HurricaneWing10 = 40832, // Helper->self, 11.0s cast, range 23-30 donut

    // Concentric AOEs
    HurricaneWing11 = 40839, // Helper->self, 4.0s cast, range 9 circle
    HurricaneWing12 = 40840, // Helper->self, 6.0s cast, range 9-16 donut
    HurricaneWing13 = 40841, // Helper->self, 8.0s cast, range 16-23 donut
    HurricaneWing14 = 40842, // Helper->self, 10.0s cast, range 23-30 donut

    HurricaneWing15 = 41302, // Helper->self, no cast, range 70 circle
    HurricaneWing16 = 41303, // Helper->self, no cast, range 70 circle
    HurricaneWing17 = 41304, // Helper->self, no cast, range 70 circle
    HurricaneWing18 = 41305, // Helper->self, no cast, range 70 circle

    OffensivePosture1 = 40811, // Boss->self, 8.0+1.0s cast, single-target
    OffensivePosture2 = 40814, // Boss->self, 8.0+1.1s cast, single-target
    OffensivePosture3 = 40816, // Boss->self, 8.0+1.2s cast, single-target

    SpikeFlail = 41114, // Helper->self, 9.0s cast, range 80 270.000-degree cone // Rear cone aoe
    DragonBreath = 40815, // Helper->self, no cast, range ?-30 donut // Donut, inner circle is boss hitbox 
    Touchdown = 41116, // Helper->self, 9.2s cast, range 24 circle // Point blank AOE, likely slightly larger than Boss hitbox

    PestilentSphere = 40859, // Darter->player, 5.0s cast, single-target

    SharpSpike1 = 40855, // Boss->self, 5.0+1.0s cast, single-target // AOE tankbusters on all 3 tanks
    SharpSpike2 = 40856, // Helper->player, no cast, range 4 circle

    ShudderingEarth = 40857, // Boss->location, no cast, single-target

    WingedTerror1 = 40847, // Boss->self, 6.0+1.4s cast, single-target
    WingedTerror2 = 40848, // Helper->self, 7.4s cast, range 70 width 25 rect
}

public enum SID : uint
{
    Bleeding = 3077, // none->player, extra=0x0
    Burns1 = 1787, // Boss->player, extra=0x0
    Burns2 = 2194, // Boss->player, extra=0x0
    Concussion = 997, // Helper->player, extra=0xF43
    DirectionalDisregard = 3808, // none->Boss, extra=0x0
    Hysteria = 296, // Helper->player, extra=0x0
    Liftoff = 4377, // Helper->player, extra=0x0
    Stun = 4433, // Helper->player, extra=0x0
    Unknown = 2056, // none->Boss, extra=0x328/0x346/0x347/0x327/0x348
    VulnerabilityUp = 1789, // Helper->player, extra=0x1/0x4/0x2/0x3/0x5
    Windburn = 2947, // Helper->player, extra=0x0
}

public enum IconID : uint
{
    Icon342 = 342, // player->self
    Icon499 = 499, // player->self
}
