namespace BossMod.Shadowbringers.Foray.DelubrumReginae.Normal.DRN1TrinitySeeker;

class MercifulBreeze(BossModule module) : Components.SimpleAOEs(module, (uint)AID.MercifulBreeze, new AOEShapeRect(50f, 2.5f));
class MercifulBlooms(BossModule module) : Components.SimpleAOEs(module, (uint)AID.MercifulBlooms, 20f);
class MercifulArc(BossModule module) : Components.BaitAwayIcon(module, new AOEShapeCone(12f, 45f.Degrees()), (uint)IconID.MercifulArc, (uint)AID.MercifulArc); // TODO: verify angle

class BurningChains(BossModule module) : Components.Chains(module, (uint)TetherID.BurningChains, (uint)AID.ScorchingShackle);

class IronImpact(BossModule module) : Components.CastCounter(module, (uint)AID.IronImpact);
class IronRose(BossModule module) : Components.SimpleAOEs(module, (uint)AID.IronRose, new AOEShapeRect(50f, 4f));

class DeadIron : Components.BaitAwayTethers
{
    public DeadIron(BossModule module) : base(module, new AOEShapeCone(50, 15.Degrees()), (uint)TetherID.DeadIron, (uint)AID.DeadIronAOE) { DrawTethers = false; }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 760, NameID = 9834)]
public class DRN1TrinitySeeker(WorldState ws, Actor primary) : TrinitySeeker(ws, primary);
