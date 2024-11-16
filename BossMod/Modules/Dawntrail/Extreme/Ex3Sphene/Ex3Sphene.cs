namespace BossMod.Dawntrail.Extreme.Ex3Sphene;

class ProsecutionOfWar(BossModule module) : Components.TankSwap(module, ActionID.MakeSpell(AID.ProsecutionOfWar), ActionID.MakeSpell(AID.ProsecutionOfWar), ActionID.MakeSpell(AID.ProsecutionOfWarAOE), 3.1f, null, true);
class DyingMemory(BossModule module) : Components.CastCounter(module, ActionID.MakeSpell(AID.DyingMemory));
class DyingMemoryLast(BossModule module) : Components.CastCounter(module, ActionID.MakeSpell(AID.DyingMemoryLast));

[ModuleInfo(BossModuleInfo.Maturity.Contributed, Contributors = "veyn, Malediktus", PlanLevel = 100, PrimaryActorOID = (uint)OID.BossP1, GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1017, NameID = 13029)]
public class Ex3Sphene(WorldState ws, Actor primary) : BossModule(ws, primary, ArenaCenter, NormalBounds)
{
    public static readonly WPos ArenaCenter = new(100, 100), LeftSplitCenter = new(108, 94), RightSplitCenter = new(92, 94),
    IceBoundsLeft = new(112, 95), IceBoundsRight = new(88, 95), IceBridge1 = new(100, 96), IceBridge2 = new(100, 104);
    public static readonly ArenaBoundsSquare NormalBounds = new(20);
    public static readonly ArenaBoundsComplex WindBounds = new([new Rectangle(new(100, 82.5f), 12.5f, 2.5f), new Rectangle(new(100, 102.5f), 12.5f, 2.5f),
    new Cross(new(100, 92.5f), 15, 2.5f, 45.Degrees())], Offset: -0.5f);
    public static readonly ArenaBoundsComplex EarthBounds = new([new Rectangle(LeftSplitCenter, 4, 8), new Rectangle(RightSplitCenter, 4, 8)]);
    private static readonly Rectangle[] iceRects1 = [new Rectangle(IceBoundsLeft, 4, 15), new Rectangle(IceBoundsRight, 4, 15), new Rectangle(ArenaCenter, 2, 10)];
    private static readonly Rectangle[] iceRects2 = [new Rectangle(IceBridge1, 8, 2), new Rectangle(IceBridge2, 8, 2)];
    public static readonly ArenaBoundsComplex IceBounds = new(iceRects1);
    public static readonly ArenaBoundsComplex IceBridgeBounds = new([.. iceRects1, .. iceRects2]);

    private Actor? _bossP2;
    public Actor? BossP1() => PrimaryActor;
    public Actor? BossP2() => _bossP2;

    protected override void UpdateModule()
    {
        // TODO: this is an ugly hack, think how multi-actor fights can be implemented without it...
        // the problem is that on wipe, any actor can be deleted and recreated in the same frame
        _bossP2 ??= StateMachine.ActivePhaseIndex > 0 ? Enemies(OID.BossP2).FirstOrDefault() : null;
    }

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actor(_bossP2);
    }
}
