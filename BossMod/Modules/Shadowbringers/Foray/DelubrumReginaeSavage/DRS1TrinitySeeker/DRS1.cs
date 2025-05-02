﻿namespace BossMod.Shadowbringers.Foray.DelubrumReginae.DRS1TrinitySeeker;

class VerdantTempest(BossModule module) : Components.CastCounter(module, (uint)AID.VerdantTempestAOE);
class MercifulBreeze(BossModule module) : Components.SimpleAOEs(module, (uint)AID.MercifulBreeze, new AOEShapeRect(50f, 2.5f));
class MercifulBlooms(BossModule module) : Components.SimpleAOEs(module, (uint)AID.MercifulBlooms, 20f);
class MercifulArc(BossModule module) : Components.BaitAwayIcon(module, new AOEShapeCone(12f, 45f.Degrees()), (uint)IconID.MercifulArc, (uint)AID.MercifulArc); // TODO: verify angle

// TODO: depending on phantom edge, it's either a shared tankbuster cleave or a weird cleave ignoring closest target (?)
abstract class BalefulOnslaught(BossModule module, uint aid) : Components.Cleave(module, aid, new AOEShapeCone(10f, 45f.Degrees())); // TODO: verify angle
class BalefulOnslaught1(BossModule module) : BalefulOnslaught(module, (uint)AID.BalefulOnslaughtAOE1);
class BalefulOnslaught2(BossModule module) : BalefulOnslaught(module, (uint)AID.BalefulOnslaughtAOE2);

class BurningChains(BossModule module) : Components.Chains(module, (uint)TetherID.BurningChains, (uint)AID.ScorchingShackle);

// TODO: it's a line stack, but I don't think there's a way to determine cast target - so everyone should just stack?..
class IronImpact(BossModule module) : Components.CastCounter(module, (uint)AID.IronImpact);
class IronRose(BossModule module) : Components.SimpleAOEs(module, (uint)AID.IronRose, new AOEShapeRect(50f, 4f));

class DeadIron : Components.BaitAwayTethers
{
    public DeadIron(BossModule module) : base(module, new AOEShapeCone(50f, 15f.Degrees()), (uint)TetherID.DeadIron, (uint)AID.DeadIronAOE) { DrawTethers = false; }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, GroupType = BossModuleInfo.GroupType.CFC, GroupID = 761, NameID = 9834, PlanLevel = 80)]
public class DRS1(WorldState ws, Actor primary) : TrinitySeeker(ws, primary);
