namespace BossMod.Heavensward.Dungeon.D06AetherochemicalResearchFacility.D060FacilityDreadnaught;

public enum OID : uint
{
    Boss = 0xF54, // R3.0
    MonitoringDrone = 0xF55 // R2.4
}

public enum AID : uint
{
    AutoAttack = 872, // Boss->player, no cast, single-target

    AutoCannons = 4825, // MonitoringDrone->self, 3.0s cast, range 40+R width 5 rect
    Rotoswipe = 4556, // Boss->self, 3.0s cast, range 8+R 120-degree cone
    WreckingBall = 4557 // Boss->location, 4.0s cast, range 8 circle
}

class Rotoswipe(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Rotoswipe), new AOEShapeCone(11, 60.Degrees()));
class AutoCannons(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.AutoCannons), new AOEShapeRect(42.4f, 2.5f));
class WreckingBall(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.WreckingBall), 8);

class D060FacilityDreadnaughtStates : StateMachineBuilder
{
    public D060FacilityDreadnaughtStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<Rotoswipe>()
            .ActivateOnEnter<AutoCannons>()
            .ActivateOnEnter<WreckingBall>()
            .Raw.Update = () => module.Enemies(D060FacilityDreadnaught.Trash).All(x => x.IsDeadOrDestroyed);
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 38, NameID = 3836, SortOrder = 7)]
public class D060FacilityDreadnaught(WorldState ws, Actor primary) : BossModule(ws, primary, arena.Center, arena)
{
    private static readonly ArenaBoundsComplex arena = new([new Polygon(new(-360, -250), 9, 6)]);
    public static readonly uint[] Trash = [(uint)OID.Boss, (uint)OID.MonitoringDrone];

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actors(Enemies(Trash));
    }
}
