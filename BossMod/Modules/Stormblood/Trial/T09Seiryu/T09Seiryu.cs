namespace BossMod.Stormblood.Trial.T09Seiryu;

class HundredTonzeSwing(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.HundredTonzeSwing), 16);
class CoursingRiver(BossModule module) : Components.KnockbackFromCastTarget(module, ActionID.MakeSpell(AID.CoursingRiverAOE), 25, true, kind: Kind.DirForward)
{
    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (!Module.FindComponent<Handprint>()!.ActiveAOEs(slot, actor).Any())
            foreach (var c in Casters)
                hints.AddForbiddenZone(ShapeDistance.Rect(c.CastInfo!.Rotation.AlmostEqual(90.Degrees(), Angle.DegToRad) ? c.Position - new WDir(12.5f, 0) : c.Position - new WDir(-12.5f, 0), c.Rotation, 50, default, 20), Module.CastFinishAt(c.CastInfo));
    }
}

class DragonsWake(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.DragonsWake2));
class FifthElement(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.FifthElement));
class FortuneBladeSigil(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.FortuneBladeSigil), new AOEShapeRect(100, 2));

class InfirmSoul(BossModule module) : Components.BaitAwayCast(module, ActionID.MakeSpell(AID.InfirmSoul), new AOEShapeCircle(4), true)
{
    public override void AddGlobalHints(GlobalHints hints)
    {
        if (CurrentBaits.Count > 0)
            hints.Add("Tankbuster cleave");
    }
}

class SerpentDescending(BossModule module) : Components.SpreadFromIcon(module, (uint)IconID.Spreadmarker, ActionID.MakeSpell(AID.SerpentDescending), 5, 6);
class YamaKagura(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.YamaKagura), new AOEShapeRect(60, 3));
class Handprint(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Handprint2), new AOEShapeCone(40, 90.Degrees()));

class ForceOfNature1(BossModule module) : Components.KnockbackFromCastTarget(module, ActionID.MakeSpell(AID.ForceOfNature1), 10)
{
    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (Sources(slot, actor).Any())
            hints.AddForbiddenZone(ShapeDistance.InvertedCircle(Arena.Center, 10), Sources(slot, actor).FirstOrDefault().Activation);
    }
}
class ForceOfNature2(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.ForceOfNature2), 5);
class KanaboBait(BossModule module) : Components.BaitAwayTethers(module, new AOEShapeCone(45, 30.Degrees()), (uint)TetherID.BaitAway, ActionID.MakeSpell(AID.KanaboVisual2), (uint)OID.IwaNoShiki, 5.9f)
{
    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        base.AddAIHints(slot, actor, assignment, hints);
        if (CurrentBaits.Any(x => x.Target == actor))
            hints.AddForbiddenZone(ShapeDistance.Circle(Arena.Center, 19), WorldState.FutureTime(ActivationDelay));
    }
}

class KanaboAOE(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Kanabo), new AOEShapeCone(45, 30.Degrees()));
class BlueBolt(BossModule module) : Components.LineStack(module, ActionID.MakeSpell(AID.BlueBoltMarker), ActionID.MakeSpell(AID.BlueBolt), 5.9f, 83, 2.5f);
class ForbiddenArts(BossModule module) : Components.LineStack(module, ActionID.MakeSpell(AID.ForbiddenArtsMarker), ActionID.MakeSpell(AID.ForbiddenArtsSecond), 5.2f, 84.4f, 4); // this hits twice
class RedRush(BossModule module) : Components.BaitAwayTethers(module, new AOEShapeRect(82.6f, 2.5f), (uint)TetherID.BaitAway, ActionID.MakeSpell(AID.RedRush), (uint)OID.AkaNoShiki, 6)
{
    public override void OnTethered(Actor source, ActorTetherInfo tether)
    {
        base.OnTethered(source, tether);
        var (player, enemy) = DetermineTetherSides(source, tether);
        if (player != null && enemy != null)
            Module.FindComponent<BlueBolt>()!.ForbiddenActors.Add(player);
    }

    public override void OnUntethered(Actor source, ActorTetherInfo tether)
    {
        base.OnUntethered(source, tether);
        var (player, enemy) = DetermineTetherSides(source, tether);
        if (player != null && enemy != null)
            Module.FindComponent<BlueBolt>()!.ForbiddenActors.Remove(player);
    }
    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        base.AddAIHints(slot, actor, assignment, hints);
        var activation = WorldState.FutureTime(ActivationDelay);
        if (CurrentBaits.Any(x => x.Target == actor) && Arena.Bounds == T09Seiryu.Phase2Bounds)
            hints.AddForbiddenZone(ShapeDistance.InvertedCircle(Arena.Center, 5), activation);
        else if (CurrentBaits.Any(x => x.Target == actor) && Arena.Bounds == T09Seiryu.Phase1Bounds)
            hints.AddForbiddenZone(ShapeDistance.Circle(Arena.Center, 18.5f), activation);
    }
}

class ArenaChange(BossModule module) : BossComponent(module)
{

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.StrengthOfSpirit) // in phase 2 the arena no longer got a wall and we need to add back the player hitboxradius
            Arena.Bounds = T09Seiryu.Phase2Bounds;
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 637, NameID = 7922)]
public class T09Seiryu(WorldState ws, Actor primary) : BossModule(ws, primary, new(100, 100), Phase1Bounds)
{
    public static readonly ArenaBounds Phase1Bounds = new ArenaBoundsCircle(19.5f);
    public static readonly ArenaBounds Phase2Bounds = new ArenaBoundsCircle(20);

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.DoroNoShiki));
        Arena.Actors(Enemies(OID.NumaNoShiki));
    }
}
