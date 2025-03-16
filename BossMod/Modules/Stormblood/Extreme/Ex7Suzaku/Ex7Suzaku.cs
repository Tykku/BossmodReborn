namespace BossMod.Stormblood.Extreme.Ex7Suzaku;

class Cremate(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.Cremate));
class ScreamsOfTheDamned(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.ScreamsOfTheDamned));
class AshesToAshes(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.AshesToAshes));
class ScarletFever(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.ScarletFever));
class SouthronStar(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.SouthronStar));
class Rout(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Rout), new AOEShapeRect(55, 3));

class FleetingSummer(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.FleetingSummer), new AOEShapeCone(40f, 45f.Degrees()));

class WellOfFlame(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.WellOfFlame), new AOEShapeRect(41f, 10f));
class ScathingNetStack(BossModule module) : Components.StackWithIcon(module, (uint)IconID.Stackmarker, ActionID.MakeSpell(AID.ScathingNetStack), 6f, 5.1f, 8);
class PhantomFlurryAOE(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.PhantomFlurryAOE), new AOEShapeCone(41f, 90f.Degrees()));
class Hotspot(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Hotspot), new AOEShapeCone(21f, 45f.Degrees()));

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "Kismet", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 597, NameID = 7702)]
public class Ex7Suzaku(WorldState ws, Actor primary) : BossModule(ws, primary, ArenaCenter, Phase1Bounds)
{
    public static readonly WPos ArenaCenter = new(100, 100);
    public static readonly ArenaBoundsComplex Phase1Bounds = new([new Polygon(ArenaCenter, 19.5f, 80)]);
    public static readonly ArenaBoundsComplex Phase2Bounds = new([new DonutV(ArenaCenter, 3.5f, 20f, 80)]);

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies((uint)OID.ScarletLady), Colors.Vulnerable);
        Arena.Actors(Enemies((uint)OID.ScarletPlume));
    }
}
