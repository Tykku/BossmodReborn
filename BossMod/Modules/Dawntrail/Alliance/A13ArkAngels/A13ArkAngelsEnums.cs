namespace BossMod.Dawntrail.Alliance.A13ArkAngels;

public enum OID : uint
{
    Unknown1 = 0x4649, // R5.250, x?
    Unknown2 = 0x4694, // R0.500, x?

    Actor1e8f2f = 0x1E8F2F, // R0.500, x?, EventObj type
    Actor1e8fb8 = 0x1E8FB8, // R2.000, x?, EventObj type
    Actor1ebcc5 = 0x1EBCC5, // R0.500, x?, EventObj type
    Actor1ebcc6 = 0x1EBCC6, // R0.500, x?, EventObj type
    Actor1ebcc7 = 0x1EBCC7, // R0.500, x?, EventObj type
    Actor1ebcc8 = 0x1EBCC8, // R0.500, x?, EventObj type

    ArkAngelEV = 0x4682, // R3.300, x?
    ArkAngelGK = 0x4685, // R3.300, x?
    ArkAngelHM = 0x4681, // R3.300, x?
    ArkAngelHM2 = 0x4686, // R3.300, x? (ArkAngelHM
    Boss = 0x4683, // R3.300, x?
    ArkAngelTT = 0x4684, // R3.300, x?
    ArkShield = 0x4754, // R2.200, x?, Part type

    BakoolJaJa = 0x4697, // R3.150, x?
    Helper = 0x233C, // R0.500, x?, Helper type
    Teleporter = 0x1EBCD0, // R0.500, x?, EventObj type
}

public enum AID : uint
{
    AutoAttack1 = 1461, // ArkAngelGK->player, no cast, single-target
    AutoAttack2 = 40623, // ArkAngelEV->player, no cast, single-target
    AutoAttack3 = 870, // Boss/ArkAngelTT/ArkAngelHM->player, no cast, single-target

    TheDecisiveBattle1 = 41057, // Boss->self, 4.0s cast, ???
    TheDecisiveBattle2 = 41058, // ArkAngelTT->self, 4.0s cast, ???
    TheDecisiveBattle3 = 41059, // ArkAngelGK->self, 4.0s cast, ???

    Cloudsplitter1 = 41078, // Boss->self, 5.0s cast, single-target //MR targets each tank with a telegraphed AoE magical tankbuster
    Cloudsplitter2 = 41079, // Helper->player, 5.5s cast, range 6 circle

    TachiGekko = 41082, // Helper->self, 7.0s cast, range 50 circle // gaze marker
    TachiKasha = 41083, // Helper->self, 12.0s cast, range 4 circle // pink flower AoE
    TachiYukikaze = 41081, // Helper->self, 3.0s cast, range 50 width 5 rect // crisscrossing ice line AoEs
    ConcertedDissolution = 41084, // Helper->self, 6.0s cast, range 40 ?-degree cone // need to confirm angle
    LightsChain = 41085, // Helper->self, 8.0s cast, range ?-40 donut

    MeteorVisual = 41098, // ArkAngelTT->self, 11.0s cast, single-target, interruptible VERY heavy raidwide
    Meteor = 41099, // Helper->location, no cast, range 100 circle

    Aethersplit1 = 41104, // Boss->ArkAngelEV/ArkAngelTT, no cast, single-target
    Aethersplit2 = 41105, // ArkAngelGK->ArkAngelTT/ArkAngelEV, no cast, single-target
    Aethersplit3 = 41106, // ArkAngelHM->ArkAngelEV/ArkAngelTT, no cast, single-target
    Aethersplit4 = 41107, // ArkAngelEV->ArkAngelTT, no cast, single-target
    Aethersplit5 = 41103, // ArkAngelTT->Boss/ArkAngelGK/ArkAngelHM, no cast, single-target

    ArroganceIncarnate1 = 41095, // ArkAngelEV->self, 5.0s cast, single-target
    ArroganceIncarnate2 = 41096, // Helper->players, no cast, range 6 circle

    CriticalReaver1 = 41275, // ArkAngelHM->self, 10.0s cast, range 100 circle
    CriticalReaver2 = 41365, // ArkAngelHM->self, no cast, range 100 circle

    CriticalStrikes = 41090, // ArkAngelHM2->player, no cast, single-target

    Dragonfall1 = 41086, // ArkAngelGK->self, 9.0s cast, single-target // Gk tetheres each a healer in each alliance then stackmarkers
    Dragonfall2 = 41087, // ArkAngelGK->players, no cast, range 6 circle

    Guillotine1 = 41063, // ArkAngelTT->self, 10.5s cast, range 40 ?-degree cone
    Guillotine2 = 41064, // Helper->self, no cast, range 40 ?-degree cone
    Guillotine3 = 41065, // Helper->self, no cast, range 40 ?-degree cone

    //
    DominionSlash = 41093, // ArkAngelEV->self, 5.0s cast, range 100 circle
    DivineDominion1 = 41094, // Helper->self, 2.0s cast, range 6 circle
    DivineDominion2 = 40628, // Helper->self, no cast, range 6 circle

    Utsusemi = 41088, // ArkAngelHM->self, 3.0s cast, single-target
    MightyStrikes1 = 41089, // ArkAngelHM/ArkAngelHM2->self, 5.0s cast, single-target
    MightyStrikes2 = 41364, // ArkAngelHM->self, 5.0s cast, single-target

    CrossReaver1 = 41091, // ArkAngelHM->self, 3.0s cast, single-target
    CrossReaver2 = 41092, // Helper->self, 6.0s cast, range 50 width 12 cross

    Holy = 41097, // ArkAngelEV->self, 5.0s cast, range 100 circle

    // Rotating conal aoes
    HavocSpiral1 = 41067, // Boss->self, 5.0+0.5s cast, single-target
    HavocSpiral2 = 41070, // Helper->self, 5.5s cast, range 30 30-degree cone
    HavocSpiral3 = 41071, // Helper->self, no cast, range 30 30-degree cone

    SpiralFinish1 = 41068, // Boss->self, 11.0+0.5s cast, single-target
    SpiralFinish2 = 41069, // Helper->self, 11.5s cast, range 100 circle

    MeikyoShisui = 41080, // ArkAngelGK->self, 4.0s cast, single-target

    MijinGakure = 41100, // ArkAngelHM->self, 30.0s cast, range 100 circle
    ProudPalisade = 42056, // ArkAngelEV->self, no cast, single-target

    Rampage1 = 41072, // Boss->self, 8.0s cast, single-target
    Rampage2 = 41075, // Boss->location, no cast, width 10 rect charge
    Rampage3 = 41076, // Boss->location, no cast, single-target
    Rampage4 = 41077, // Helper->location, 0.5s cast, range 20 circle

    UnknownAbility1 = 41060, // ArkAngelTT->location, no cast, single-target
    UnknownAbility2 = 41066, // Boss->location, no cast, single-target
    UnknownAbility3 = 40617, // ArkAngelHM->location, no cast, single-target
    UnknownAbility4 = 41814, // ArkAngelEV->location, no cast, single-target

    UnknownWeaponskill1 = 41366, // Unknown1->self, 7.0s cast, range 50 circle
    UnknownWeaponskill2 = 41073, // Helper->location, 3.0s cast, width 10 rect charge
    UnknownWeaponskill3 = 41074, // Helper->location, 3.0s cast, range 20 circle

    Raiton = 41109, // ArkAngelHM->self, 5.0s cast, range 100 circle
}

public enum SID : uint
{
    AreaOfInfluenceUp = 1749, // none->Helper, extra=0x1/0x2/0x3/0x4/0x5/0x6/0x7/0x8/0x9/0xA/0xB/0xC/0xD/0xE/0xF/0x10
    Electrocution1 = 3073, // none->player, extra=0x0
    Electrocution2 = 3074, // none->player, extra=0x0
    EpicHero = 4192, // none->player, extra=0x0
    EpicVillain = 4193, // none->Boss, extra=0x334
    FatedHero = 4194, // none->player, extra=0x0
    FatedVillain = 4195, // none->ArkAngelGK, extra=0x335
    Invincibility1 = 1570, // none->player, extra=0x0
    Invincibility2 = 4410, // none->ArkAngelHM, extra=0x0
    MeatAndMead = 360, // none->player, extra=0xA
    MightyStrikes = 4198, // ArkAngelHM/ArkAngelHM2->ArkAngelHM/ArkAngelHM2, extra=0x2C
    Petrification = 1511, // Helper->player, extra=0x0
    PiercingResistanceDown = 3131, // ArkAngelGK->player, extra=0x0
    TheHeatOfBattle = 365, // none->player, extra=0xA
    Uninterrupted = 4416, // none->ArkAngelHM, extra=0x0
    UnknownStatus = 4408, // ArkAngelEV->ArkAngelEV, extra=0x0
    VauntedHero = 4196, // none->player, extra=0x0
    VauntedVillain = 4197, // none->ArkAngelTT, extra=0x336
    VulnerabilityUp = 1789, // Helper/ArkAngelHM2/Boss->player, extra=0x1/0x2/0x3/0x4/0x5/0x6
}

public enum IconID : uint
{
    Icon168 = 168, // Boss->self // RotateCCW
    Icon305 = 305, // player->self
    Icon464 = 464, // player->self
    Icon557 = 557, // player->self
    Icon566 = 566, // player->self
    Icon567 = 567, // player->self
}

public enum TetherID : uint
{
    Tether249 = 249, // player->ArkAngelGK
    Tether293 = 293, // ArkAngelHM2->player
    Tether299 = 299, // player->ArkAngelTT/ArkAngelGK/Boss
}
