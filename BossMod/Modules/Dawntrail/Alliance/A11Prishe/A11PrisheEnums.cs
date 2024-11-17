namespace BossMod.Dawntrail.Alliance.A11Prishe;

public enum OID : uint
{
    Helper = 0x233C, // R0.500, x?, Helper type
    Unknown1 = 0x46CF, // R1.200, x?
    Unknown2 = 0x4696, // R0.465, x?
    SpatialRift = 0x1EBCD1, // R0.500, x?, EventObj type
    Boss = 0x4673, // R6.342, x?
    BakoolJaJa = 0x4697, // R3.150, x?
    LuminousRemnant = 0x4674, // R1.000-1.860, x?
    Actor1ebcc9 = 0x1EBCC9, // R0.500, x?, EventObj type
}

public enum AID : uint
{
    AutoAttack = 40934, // 4673->player, no cast, single-target
    Banishga = 40935, // 4673->self, 5.0s cast, range 80 circle
    UnknownAbility = 40933, // 4673->location, no cast, single-target

    KnuckleSandwich1 = 40936, // 4673->location, 12.0+1.0s cast, single-target
    KnuckleSandwich2 = 40937, // 4673->location, 12.0+1.0s cast, single-target
    KnuckleSandwich3 = 40938, // 4673->location, 12.0+1.0s cast, single-target
    KnuckleSandwichAOE1 = 40939, // 233C->self, 13.0s cast, range 9 circle
    KnuckleSandwichAOE2 = 40940, // 233C->self, 13.0s cast, range 18 circle
    KnuckleSandwichAOE3 = 40941, // 233C->self, 13.0s cast, range 27 circle

    BrittleImpact1 = 40942, // 233C->self, 14.5s cast, range ?-60 donut
    BrittleImpact2 = 40943, // 233C->self, 14.5s cast, range ?-60 donut
    BrittleImpact3 = 40944, // 233C->self, 14.5s cast, range ?-60 donut

    NullifyingDropkick1 = 40945, // 4673->players, 5.0+1.5s cast, range 6 circle
    NullifyingDropkick2 = 40957, // 233C->players, 6.5s cast, range 6 circle

    BanishStorm = 40946, // 4673->self, 4.0s cast, single-target // Marching AOEs
    Banish = 40947, // 233C->self, no cast, range 6 circle

    Holy1 = 40962, // 4673->self, 4.0+1.0s cast, single-target
    Holy2 = 40963, // 233C->players, 5.0s cast, range 6 circle // Spread Markers

    CrystallineThorns = 40948, // 4673->self, 4.0+1.0s cast, single-target // Arena Change
    Thornbite = 40949, // 233C->self, no cast, range 80 circle

    // AuroralUppercut1 = 40950, // 4673->self, 11.4+1.6s cast, single-target // Suspected single space cast
    AuroralUppercut2 = 40951, // 4673->self, 11.4+1.6s cast, single-target // Suspected two space cast
    AuroralUppercut3 = 40952, // 4673->self, 11.4+1.6s cast, single-target // Definitely 3 space cast
    AuroralUppercut4 = 40953, // 233C->self, no cast, range 80 circle

    BanishgaIV = 40954, // 4673->self, 5.0s cast, range 80 circle // Raidwide that spawns AOEs
    Explosion = 40955, // 4674->self, 5.0s cast, range 8 circle

    AsuranFists1 = 40956, // 4673->self, 6.5+0.5s cast, single-target
    AsuranFists2 = 40958, // 233C->self, no cast, range 6 circle
    AsuranFists3 = 40959, // 233C->self, no cast, range 6 circle
    AsuranFists4 = 40960, // 233C->self, no cast, range 6 circle
}

public enum SID : uint
{
    LightResistanceDown = 2278, // Helper->player, extra=0x0
    Stun1 = 4378, // none->player, extra=0x0
    Stun2 = 4374, // none->player, extra=0x0
    SustainedDamage1 = 2935, // Helper->player, extra=0x0
    SustainedDamage2 = 3795, // none->LuminousRemnant, extra=0x1
    Unknown1 = 2195, // none->Boss, extra=0x338
    Unknown2 = 2056, // none->Boss, extra=0x339
    Unknown3 = 3517, // Helper->player, extra=0x0
    VulnerabilityUp1 = 1789, // Helper/LuminousRemnant->player, extra=0x1/0x2/0x3/0x4
    VulnerabilityUp2 = 3366, // none->player, extra=0x0
}

public enum IconID : uint
{
    Icon570 = 570, // player->self
    Icon215 = 215, // player->self
}

public enum TetherID : uint
{
    Tether215 = 215, // Unknown1->Boss
    Tether297 = 297, // player->Unknown1
}
