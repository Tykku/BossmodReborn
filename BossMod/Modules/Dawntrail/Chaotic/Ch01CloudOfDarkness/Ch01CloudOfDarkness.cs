namespace BossMod.Dawntrail.Chaotic.Ch01CloudOfDarkness;

class Flare(BossModule module) : Components.SpreadFromIcon(module, (uint)IconID.Flare, ActionID.MakeSpell(AID.FlareAOE), 25, 8.1f);
class StygianShadow(BossModule module) : Components.Adds(module, (uint)OID.StygianShadow);
class Atomos(BossModule module) : Components.Adds(module, (uint)OID.Atomos);
class GhastlyGloomCross(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.GhastlyGloomCrossAOE), new AOEShapeCross(40, 15));
class GhastlyGloomDonut(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.GhastlyGloomDonutAOE), new AOEShapeDonut(21, 40));
class FloodOfDarknessAdd(BossModule module) : Components.CastInterruptHint(module, ActionID.MakeSpell(AID.FloodOfDarknessAdd)); // TODO: only if add is player's?..
class Excruciate(BossModule module) : Components.BaitAwayCast(module, ActionID.MakeSpell(AID.Excruciate), new AOEShapeCircle(4), true);
class LoomingChaos(BossModule module) : Components.CastCounter(module, ActionID.MakeSpell(AID.LoomingChaosAOE));

// TODO: tankswap hints component for phase1
// TODO: phase 2 teleport zones?
// TODO: grim embrace / curse of darkness prevent turning

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1010, NameID = 13624, PlanLevel = 100)]
public class Ch01CloudOfDarkness(WorldState ws, Actor primary) : BossModule(ws, primary, DefaultCenter, DefaultArena)
{
    public static readonly WPos DefaultCenter = new(100, 100);
    public static readonly WPos Phase1BoundsCenter = new(100, 76.28427f);
    public static readonly PolygonCustom[] Diamond = [new([new(115, 63), new(128.28427f, 76.28427f), new(100, 104.56854f), new(71.71573f, 76.28427f), new(85, 63)])];
    private static readonly DonutV[] donut = [new(DefaultCenter, 34, 40, 80)];
    public static readonly Square[] IntersectionBlockers = [.. GenerateIntersectionBlockers()];
    public static readonly Shape[] Phase2ShapesND = [new Rectangle(new(100, 115), 24, 3), new Rectangle(new(100, 85), 24, 3), new Rectangle(new(115, 100), 3, 24),
    new Rectangle(new(85, 100), 3, 24), new Square(new(126.5f, 100), 7.5f), new Square(new(73.5f, 100), 7.5f)];
    public static readonly Shape[] Phase2ShapesWD = [.. donut, .. Phase2ShapesND];
    public static readonly ArenaBoundsCircle DefaultArena = new(40);
    public static readonly ArenaBoundsComplex Phase1Bounds = new(Diamond, ScaleFactor: 1.414f);
    public static readonly ArenaBoundsComplex Phase2BoundsWD = new(Phase2ShapesWD, IntersectionBlockers);
    public static readonly ArenaBoundsComplex Phase2BoundsND = new(Phase2ShapesND, [.. IntersectionBlockers, .. donut]);

    private static Square[] GenerateIntersectionBlockers() // at intersections there are small blockers to prevent players from skipping tiles
    {
        var a45 = 45.Degrees();
        var a135 = 135.Degrees();
        WDir[] dirs = [a45.ToDirection(), a135.ToDirection(), (-a45).ToDirection(), (-a135).ToDirection()];
        WPos[] pos = [new(85, 85), new(115, 85), new(115, 115), new(85, 115)];
        var distance = 3 * MathF.Sqrt(2);

        var squares = new Square[16];
        var index = 0;
        for (var i = 0; i < 4; ++i)
            for (var j = 0; j < 4; ++j)
                squares[index++] = new(pos[i] + distance * dirs[j], 1, a45);
        return squares;
    }
}
