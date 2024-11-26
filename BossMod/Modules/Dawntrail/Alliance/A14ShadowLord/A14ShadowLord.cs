namespace BossMod.Dawntrail.Alliance.A14ShadowLord;

class TeraSlash(BossModule module) : Components.CastCounter(module, ActionID.MakeSpell(AID.TeraSlash));
class UnbridledRage(BossModule module) : Components.BaitAwayIcon(module, new AOEShapeRect(100, 4), (uint)IconID.UnbridledRage, ActionID.MakeSpell(AID.UnbridledRageAOE), 5.9f);
class DarkNova(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.DarkNova), 6);
public class StayInBounds(BossModule module) : BossComponent(module)
{
    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (!Arena.InBounds(actor.Position))
            hints.AddForbiddenZone(ShapeDistance.InvertedDonut(Arena.Center, 11, 12));
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1015, NameID = 13653, SortOrder = 8)]
public class A14ShadowLord(WorldState ws, Actor primary) : BossModule(ws, primary, ArenaCenter, DefaultBounds)
{
    private const int RadiusSmall = 8;
    public static readonly WPos ArenaCenter = new(150, 800);
    public static readonly ArenaBoundsCircle DefaultBounds = new(30);
    private static readonly Circle[] circles = [new(new(166, 800), RadiusSmall), new(new(134, 800), RadiusSmall),
    new(new(150, 816), RadiusSmall), new(new(150, 784), RadiusSmall)];
    private static readonly RectangleSE[] rects = [new(circles[1].Center, circles[2].Center, 2), new(circles[1].Center, circles[3].Center, 2),
    new(circles[3].Center, circles[0].Center, 2), new(circles[0].Center, circles[2].Center, 2)];
    public static readonly Shape[] Combined = [.. circles, .. rects];
    public static readonly ArenaBoundsComplex ComplexBounds = new(Combined);
}
