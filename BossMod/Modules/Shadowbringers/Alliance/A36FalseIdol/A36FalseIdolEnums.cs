namespace BossMod.Shadowbringers.Alliance.A36FalseIdol;

public enum OID : uint
{
    Helper = 0x233C, // R0.500, x21, 523 type
    Boss = 0x318D, // R26.000, x1
    Ally2B = 0x31A8, // R0.512, x1
}

public enum AID : uint
{
    BossAutoAttack = 24572, // Boss->player, no cast, single-target
    Eminence = 24021, // Boss->location, 5.0s cast, range 60 circle
    RhythmRings = 23508, // Boss->self, 3.0s cast, single-target
    MagicalInterference = 23509, // Helper->self, no cast, range 50 width 10 rect
    MadeMagic1 = 23510, // Boss->self, 7.0s cast, range 50 width 30 rect
    MadeMagic2 = 23511, // Boss->self, 7.0s cast, range 50 width 30 rect
    LighterNote1 = 23512, // Boss->self, 3.0s cast, single-target
    LighterNote2 = 23513, // Helper->location, no cast, range 6 circle
    LighterNote3 = 23514, // Helper->location, no cast, range 6 circle
    DarkerNote1 = 23515, // Boss->self, 5.0s cast, single-target
    DarkerNote2 = 23516, // Helper->players, 5.0s cast, range 6 circle
    ScreamingScore = 23517, // Boss->self, 5.0s cast, range 60 circle
    SeedOfMagic = 23518, // Boss->self, 3.0s cast, single-target
    ScatteredMagic = 23519, // Helper->location, 3.0s cast, range 4 circle

    DancingBlade = 23790, // Ally2B->Boss, no cast, width 2 rect charge
    BalancedEdge = 23791, // Ally2B->self, 2.0s cast, range 5 circle
    WhirlingAssault = 23792, // Ally2B->self, 2.0s cast, range 40 width 4 rect
    UnknownAbility = 18683, // Ally2B->location, no cast, single-target
    BladeFlurry = 23788, // Ally2B->Boss/HerInflorescence, no cast, single-target
    BladeFlurry2 = 23789, // Ally2B->Boss/HerInflorescence, no cast, single-target
}

public enum SID : uint
{
    VulnerabilityUp = 1789, // Boss/Helper->player, extra=0x1/0x2
    Weakness = 43, // none->player, extra=0x0
    Transcendent = 418, // none->player, extra=0x0
    DownForTheCount = 2408, // Boss->player, extra=0xEC7
}

public enum IconID : uint
{
    Icon1 = 1, // player
    Icon139 = 139, // player
}
