namespace BossMod.Shadowbringers.Foray.DelubrumReginae.DRS1TrinitySeeker;

class VerdantTempest(BossModule module) : Components.CastCounter(module, (uint)AID.VerdantTempestAOE);
class MercifulBreeze(BossModule module) : Components.SimpleAOEs(module, (uint)AID.MercifulBreeze, new AOEShapeRect(50, 2.5f));
class MercifulBlooms(BossModule module) : Components.SimpleAOEs(module, (uint)AID.MercifulBlooms, 20);
class MercifulArc(BossModule module) : Components.BaitAwayIcon(module, new AOEShapeCone(12, 45.Degrees()), (uint)IconID.MercifulArc, (uint)AID.MercifulArc); // TODO: verify angle

// TODO: depending on phantom edge, it's either a shared tankbuster cleave or a weird cleave ignoring closest target (?)
class BalefulOnslaught1(BossModule module) : Components.Cleave(module, (uint)AID.BalefulOnslaughtAOE1, new AOEShapeCone(10, 45.Degrees())); // TODO: verify angle
class BalefulOnslaught2(BossModule module) : Components.Cleave(module, (uint)AID.BalefulOnslaughtAOE2, new AOEShapeCone(10, 45.Degrees())); // TODO: verify angle

class BurningChains(BossModule module) : Components.Chains(module, (uint)TetherID.BurningChains, (uint)AID.ScorchingShackle);

// TODO: it's a line stack, but I don't think there's a way to determine cast target - so everyone should just stack?..
class IronImpact(BossModule module) : Components.CastCounter(module, (uint)AID.IronImpact);
class IronRose(BossModule module) : Components.SimpleAOEs(module, (uint)AID.IronRose, new AOEShapeRect(50, 4));

class DeadIron : Components.BaitAwayTethers
{
    public DeadIron(BossModule module) : base(module, new AOEShapeCone(50, 15.Degrees()), (uint)TetherID.DeadIron, (uint)AID.DeadIronAOE) { DrawTethers = false; }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, GroupType = BossModuleInfo.GroupType.CFC, GroupID = 761, NameID = 9834, PlanLevel = 80)]
public class DRS1(WorldState ws, Actor primary) : TrinitySeeker(ws, primary);
