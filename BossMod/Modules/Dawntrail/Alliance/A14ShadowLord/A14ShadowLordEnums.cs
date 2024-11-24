namespace BossMod.Dawntrail.Alliance.A14ShadowLord;

public enum OID : uint
{
    Helper = 0x233C, // R0.500, x40, Helper type
    LordlyShadow = 0x4659, // R10.502, x2
    Boss = 0x4657, // R10.502, x1
    Exit = 0x1E850B, // R0.500, x1, EventObj type
}

public enum AID : uint
{
    AutoAttack = 41747, // Boss->player, no cast, single-target

    GigaSlash1 = 40766, // Boss->self, 11.0+1.0s cast, single-target
    GigaSlash2 = 40767, // LordlyShadow->self, 11.0+1.0s cast, single-target
    GigaSlash3 = 40768, // Helper->self, 12.0s cast, range 60 225.000-degree cone
    GigaSlash4 = 40769, // Helper->self, 1.0s cast, range 60 270.000-degree cone
    GigaSlash5 = 40770, // Helper->self, 12.0s cast, range 60 225.000-degree cone
    GigaSlash6 = 40771, // Helper->self, 1.0s cast, range 60 270.000-degree cone

    UmbraSmash1 = 40787, // LordlyShadow->self, 4.5+0.5s cast, single-target
    UmbraSmash2 = 40788, // Helper->self, 5.0s cast, range 60 width 10 rect
    UmbraSmash3 = 40795, // Boss->self, 4.0+0.5s cast, single-target
    UmbraSmash4 = 40796, // Boss->self, no cast, single-target
    UmbraSmash5 = 40797, // Helper->self, 4.5s cast, range 60 width 10 rect
    UmbraSmash6 = 40798, // Helper->self, 7.2s cast, range 60 width 10 rect
    UmbraSmash7 = 40799, // Helper->self, 9.9s cast, range 60 width 10 rect
    UmbraSmash8 = 40800, // Helper->self, 12.6s cast, range 60 width 10 rect
    UmbraWave = 40801, // Helper->self, 2.0s cast, range 5 width 60 rect

    FlamesOfHatred = 40809, // Boss->self, 5.0s cast, range 100 circle // Raidwide

    ImplosionVisual1 = 40772, // Boss/LordlyShadow->self, 8.0+1.0s cast, single-target
    ImplosionVisual2 = 40773, // Boss/LordlyShadow->self, 8.0+1.0s cast, single-target
    Implosion1 = 40774, // Helper->self, 9.0s cast, range 12 180.000-degree cone
    Implosion2 = 40775, // Helper->self, 9.0s cast, range 12 180.000-degree cone
    Implosion3 = 40776, // Helper->self, 9.0s cast, range 12 180.000-degree cone
    Implosion4 = 40777, // Helper->self, 9.0s cast, range 12 180.000-degree cone

    CthonicFury1 = 40778, // Boss->self, 7.0s cast, range 100 circle // Raidwide and arena change
    CthonicFury2 = 40779, // Boss->self, 7.0s cast, range 100 circle // Raidwide and arena revert

    BurningBattlements = 40783, // Helper->self, 7.0s cast, ???
    BurningCourt = 40780, // Helper->self, 7.0s cast, range 8 circle // Circular spawn
    BurningKeep = 40782, // Helper->self, 7.0s cast, range 23 width 23 rect // Square spawn
    BurningMoat = 40781, // Helper->self, 7.0s cast, range ?-15 donut

    DarkNebula1 = 40784, // Boss->self, 3.0s cast, single-target
    DarkNebula2 = 40785, // Helper->self, 13.0s cast, range 60 width 100 rect //bi-directional knockback, likely needs work similar to NavigatorsTridentKnockback
    DarkNebula3 = 41532, // Helper->self, 5.0s cast, range 60 width 100 rect

    EchoesOfAgony1 = 41899, // Boss->self, 8.0s cast, single-target
    EchoesOfAgony2 = 41900, // Helper->players, no cast, range 5 circle

    Nightfall = 41144, // Boss->self, 5.0s cast, single-target // Boss phase change

    TeraSlash = 41145, // Helper->self, no cast, range 100 circle // Raidwide

    GigaSlashNightfallVisual1 = 42020, // Boss->self, 14.0+1.0s cast, single-target
    GigaSlashNightfallVisual2 = 42021, // Boss->self, 14.0+1.0s cast, single-target
    GigaSlashNightfallVisual3 = 42023, // Boss->self, 14.0+1.0s cast, single-target
    GigaSlashNightfallVisual4 = 42022, // Boss->self, 14.0+1,0s cast, single-target
    GigaSlashNightfall1 = 42024, // Helper->self, 1.0s cast, range 60 210-degree cone
    GigaSlashNightfall2 = 42025, // Helper->self, 1.0s cast, range 60 210-degree cone
    GigaSlashNightfall3 = 42027, // Helper->self, 15.0s cast, range 60 225-degree cone
    GigaSlashNightfall4 = 42028, // Helper->self, 1.0s cast, range 60 270-degree cone
    GigaSlashNightfall5 = 42029, // Helper->self, 15.0s cast, range 60 225-degree cone
    GigaSlashNightfall6 = 42030, // Helper->self, 1.0s cast, range 60 270-degree cone

    ShadowSpawn = 40786, // Boss->self, 3.0s cast, single-target // Summons two untargetable Lordly Shadow clones

    UnbridledRage1 = 40807, // Boss->self, 5.0s cast, single-target // Line AoE physical tankbuster on all 3 tanks
    UnbridledRage2 = 40808, // Helper->self, no cast, range 100 width 8 rect

    DarkNova = 41335, // Helper->players, 6.0s cast, range 6 circle // spread marker AoEs

    BindingSigil = 40789, // Boss->self, 12.0+1.0s cast, single-target
    SoulBinding = 41514, // Helper->self, 1.0s cast, range 9 circle

    DamningStrikes1 = 40791, // Boss->self, 8.0s cast, single-target // Tower Stacks
    DamningStrikes2 = 40793, // Boss->self, no cast, single-target
    DamningStrikes3 = 42052, // Boss->self, no cast, single-target
    DamningStrikes4 = 41054, // Boss->self, 8.7s cast, single-target
    DamningStrikes5 = 42055, // Boss->location, no cast, single-target
    DamningStrikes6 = 42053, // Boss->self, no cast, single-target
    DamningStrikes7 = 42054, // Boss->location, no cast, single-target
    DamningStrikes8 = 40794, // Boss->location, no cast, single-target

    Shockwave = 41112, // Helper->self, no cast, range 100 circle

    Impact1 = 40792, // Helper->self, 10.5s cast, range 3 circle
    Impact2 = 41110, // Helper->self, 13.0s cast, range 3 circle
    Impact3 = 41111, // Helper->self, 15.7s cast, range 3 circle

    DoomArc = 40806, // Boss->self, 15.0s cast, range 100 circle // Raidwide and Damage Up

    Burst = 41531, // Helper->self, no cast, ???

    UnknownAbility1 = 40810, // Boss->location, no cast, single-target
    UnknownAbility2 = 41513, // Helper->self, 1.5s cast, range 9 circle
    UnknownAbility3 = 41530, // Helper->player, no cast, single-target
    UnknownAbility4 = 41815, // Boss->location, no cast, single-target
    UnknownAbility5 = 41816, // Boss->location, no cast, single-target
}

public enum SID : uint
{
    SustainedDamage = 2935, // Helper->player, extra=0x0
    VulnerabilityUp = 1789, // Helper->player, extra=0x1/0x2/0x3
    RedrawnDomain = 4357, // none->Helper, extra=0x1E0
    Burns1 = 3065, // none->player, extra=0x0
    Burns2 = 3066, // none->player, extra=0x0
    DirectionalDisregard = 3808, // none->Boss, extra=0x0
    UnknownStatus1 = 2273, // Boss->Boss, extra=0x21D
    LordUnshadowed = 4351, // none->Boss, extra=0x0
    MagicVulnerabilityUp = 2941, // Helper->player, extra=0x0
    BondsOfLoathing = 4185, // Helper->player, extra=0x0
    Levitation = 4219, // none->player, extra=0x9C4
    UnknownStatus2 = 2970, // none->player, extra=0x32D
    UnknownStatus3 = 2279, // none->player, extra=0xEC4
    Stun = 2656, // none->player, extra=0x0
    Bleeding = 2088, // Boss->player, extra=0x0
    DamageUp = 2550, // Boss->Boss, extra=0x1
}

public enum IconID : uint
{
    Icon311 = 311, // player->self
    Icon471 = 471, // player->self
    Icon545 = 545, // player->self
}
