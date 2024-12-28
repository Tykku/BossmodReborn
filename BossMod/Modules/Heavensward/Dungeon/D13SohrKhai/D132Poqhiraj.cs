namespace BossMod.Heavensward.Dungeon.D13SohrKhai.D132Poqhiraj;

public enum OID : uint
{
    Boss = 0x155C, // R2.5
    PrayerWall = 0x155E, // R5.0
    DarkCloud = 0x155D, // R1.0
    WallHelperNW1 = 0x1EA07A, //R2.0 - 392.5, 89.161
    WallHelperNE1 = 0x1EA076, //R2.0 - 407.5, 89.161
    WallHelperNW2 = 0x1EA07B, //R2.0 - 392.5, 99.161
    WallHelperNE2 = 0x1EA077, //R2.0 - 407.5, 99.161
    WallHelperSW1 = 0x1EA07C, //R2.0 - 392.5, 109.161
    WallHelperSE1 = 0x1EA078, //R2.0 - 407.5, 109.161
    WallHelperSW2 = 0x1EA07D, //R2.0 - 392.5, 119.161
    WallHelperSE2 = 0x1EA079, //R2.0 - 407.5, 119.161
    Helper = 0x1B2
}

public enum AID : uint
{
    AutoAttack = 872, // Boss->player, no cast, single-target
    RearHoof = 6013, // Boss->player, no cast, single-target

    BurningBright = 6011, // Boss->self/players, 3.0s cast, range 26+R width 6 rect
    TouchdownVisual = 6240, // Helper->self, 3.0s cast, range 40+R circle
    Touchdown = 6012, // Boss->self, no cast, range 40+R circle
    GallopVisual = 5777, // Boss->location, 4.5s cast, width 10 rect charge
    GallopAOE = 5778, // Helper->self, 4.9s cast, range 40+R width 2 rect, player gets thrown into air + vuln stack
    GallopKB = 5823, // Helper->self, 4.9s cast, range 4+R width 40 rect, knockback 10, forward
    CloudCall = 6009, // Boss->self, 4.0s cast, single-target
    LightningBolt = 6010 // DarkCloud->self, 3.0s cast, range 8 circle
}

public enum IconID : uint
{
    CloudCall = 24 // player
}

class ArenaChanges(BossModule module) : BossComponent(module)
{
    private readonly GallopKB _kb = module.FindComponent<GallopKB>()!;
    private static readonly float[] xPositions = [395.25f, 404.75f];
    private static readonly WPos[] wallPositions = new WPos[8];
    private readonly List<Rectangle> removedWalls = [];
    private static readonly Rectangle[] baseArena = [new(D132Poqhiraj.ArenaCenter, 4.5f, 19.75f)];
    private static readonly WDir offset = new(0, 0.125f);

    static ArenaChanges()
    {
        const float zStart = 89.161f;
        const int zStep = 10;
        var index = 0;

        for (var i = 0; i < 2; ++i)
            for (var j = 0; j < 4; ++j)
                wallPositions[index++] = new(xPositions[i], zStart + j * zStep);
    }

    public override void OnActorEAnim(Actor actor, uint state)
    {
        if (state != 0x00100020)
            return;

        var wallIndex = (OID)actor.OID switch
        {
            OID.WallHelperNW1 => 0,
            OID.WallHelperNE1 => 4,
            OID.WallHelperNW2 => 1,
            OID.WallHelperNE2 => 5,
            OID.WallHelperSW1 => 2,
            OID.WallHelperSE1 => 6,
            OID.WallHelperSW2 => 3,
            OID.WallHelperSE2 => 7,
            _ => default
        };

        var wallPos = wallPositions[wallIndex];
        var adjustment = wallIndex is 0 or 4 ? offset : wallIndex is 3 or 7 ? -offset : default;
        removedWalls.Add(new(wallPos + adjustment, 0.25f, adjustment != default ? 4.875f : 5));
        _kb.safeWalls.RemoveAll(x => x.Vertex1 == new WPos(GallopKB.xPositions[wallIndex / 4], wallPos.Z - 5));
        ArenaBoundsComplex arena = new([.. baseArena, .. removedWalls]);
        Arena.Bounds = arena;
        Arena.Center = arena.Center;
    }
}

class GallopAOE(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.GallopAOE), new AOEShapeRect(40.5f, 1));

class GallopKB(BossModule module) : Components.Knockback(module)
{
    public static readonly float[] xPositions = [395.5f, 404.5f];
    private static readonly AOEShapeRect rect = new(4.5f, 20);
    private readonly List<Source> _sources = [];
    public readonly List<SafeWall> safeWalls = GenerateSafeWalls();

    private static List<SafeWall> GenerateSafeWalls()
    {
        const float zStart = 89.161f;
        const int zStep = 10;

        List<SafeWall> list = [];

        for (var i = 0; i < 2; ++i)
            for (var j = 0; j < 4; ++j)
                list.Add(new(new(xPositions[i], zStart + j * zStep - 5), new(xPositions[i], zStart + j * zStep + 5)));
        return list;
    }

    public override IEnumerable<Source> Sources(int slot, Actor actor) => _sources;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.GallopKB)
            _sources.Add(new(spell.LocXZ, 30, Module.CastFinishAt(spell), rect, spell.Rotation, Kind.DirForward, default, safeWalls));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.GallopKB)
            _sources.Clear();
    }
}

class GallopKBHint(BossModule module) : Components.GenericAOEs(module)
{
    private readonly GallopKB _kb = module.FindComponent<GallopKB>()!;
    private const string Risk2Hint = "Walk into safespot for knockback!";

    private static readonly Angle[] angles = [-89.982f.Degrees(), 89.977f.Degrees()];
    private AOEInstance? _aoe;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (_aoe == null && (AID)spell.Action.ID == AID.GallopKB)
        {
            var count = _kb.safeWalls.Count;
            if (count is 0 or 8)
                return;
            List<RectangleSE> rects = [];
            for (var i = 0; i < count; ++i)
            {
                var safeWall = _kb.safeWalls[i].Vertex1;
                var dir = (safeWall.X == GallopKB.xPositions[0] ? angles[0] : angles[1]).ToDirection();
                var pos = new WPos(safeWall.X, safeWall.Z + 5);
                rects.Add(new(pos + dir, pos - 3.5f * dir, 5));
            }
            AOEShapeCustom aoe = new(rects, InvertForbiddenZone: true);
            _aoe = new(aoe, Arena.Center, default, Module.CastFinishAt(spell), Colors.SafeFromAOE, true);
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.GallopKB)
            _aoe = null;
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (_aoe != null)
        {
            if (!ActiveAOEs(slot, actor).Any(c => c.Check(actor.Position)))
                hints.Add(Risk2Hint);
        }
    }
}

class Touchdown(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.TouchdownVisual), new AOEShapeCircle(25));

class BurningBright(BossModule module) : Components.BaitAwayCast(module, ActionID.MakeSpell(AID.BurningBright), new AOEShapeRect(28.5f, 3), endsOnCastEvent: true)
{
    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        base.DrawArenaForeground(pcSlot, pc);
        if (!ActiveBaits.Any(x => x.Target == pc))
            return;
        var walls = Module.Enemies(OID.PrayerWall);
        for (var i = 0; i < walls.Count; ++i)
        {
            var a = walls[i];
            Arena.AddCircle(a.Position, a.HitboxRadius, Colors.Danger);
        }
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (CurrentBaits.Any(x => x.Target != actor))
            base.AddHints(slot, actor, hints);
        else if (CurrentBaits.Any(x => x.Target == actor))
            hints.Add("Bait away, avoid intersecting wall hitboxes!");
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        base.AddAIHints(slot, actor, assignment, hints);
        var bait = ActiveBaitsOn(actor).FirstOrDefault();
        if (bait == default)
            return;
        var walls = Module.Enemies(OID.PrayerWall);
        var count = walls.Count;
        if (count <= 4) // don't care if most walls are up plus most of the arena would likely be forbidden anyway depending on player positioning
        {
            var forbidden = new List<Func<WPos, float>>();
            for (var i = 0; i < count; ++i)
            {
                var a = walls[i];
                forbidden.Add(ShapeDistance.Cone(bait.Source.Position, 100, bait.Source.AngleTo(a), Angle.Asin(8 / (a.Position - bait.Source.Position).Length())));
            }
            if (forbidden.Count != 0)
                hints.AddForbiddenZone(p => forbidden.Min(f => f(p)), bait.Activation);
        }
    }
}

class RearHoof(BossModule module) : Components.SingleTargetInstant(module, ActionID.MakeSpell(AID.RearHoof), 4)
{
    private bool start, firstKB;

    public override void Update()
    {
        if (!start)
        {
            Targets.Add((Raid.FindSlot(Module.PrimaryActor.TargetID), WorldState.FutureTime(6.1f))); // its assumed that the tank will aggro the boss first
            start = true;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (!firstKB && (AID)spell.Action.ID == AID.GallopKB)
        {
            Targets.Add((Raid.FindSlot(Module.PrimaryActor.TargetID), WorldState.FutureTime(7)));
            firstKB = true;
        }
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.DarkCloud)
            Targets.Add((Raid.FindSlot(actor.InstanceID), WorldState.FutureTime(4)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.RearHoof)
            Targets.Clear();
    }
}

class CloudCall(BossModule module) : Components.GenericBaitAway(module)
{
    public static readonly AOEShapeCircle Circle = new(8);

    public override void OnEventIcon(Actor actor, uint iconID, ulong targetID)
    {
        if (iconID == (uint)IconID.CloudCall)
            CurrentBaits.Add(new(actor, actor, Circle, WorldState.FutureTime(4.9f)));
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.DarkCloud)
            CurrentBaits.Clear();
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        base.AddAIHints(slot, actor, assignment, hints);
        if (CurrentBaits.Any(x => x.Target == actor))
            hints.AddForbiddenZone(ShapeDistance.Rect(Arena.Center, new WDir(0, 1), 19, 19, 5));
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (CurrentBaits.Any(x => x.Target != actor))
            base.AddHints(slot, actor, hints);
        else if (CurrentBaits.Any(x => x.Target == actor))
            hints.Add("Bait cloud away, avoid intersecting wall hitboxes!");
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        base.DrawArenaForeground(pcSlot, pc);
        if (!ActiveBaits.Any(x => x.Target == pc))
            return;
        var walls = Module.Enemies(OID.PrayerWall);
        for (var i = 0; i < walls.Count; ++i)
        {
            var a = walls[i];
            Arena.AddCircle(a.Position, a.HitboxRadius, Colors.Danger);
        }
    }
}

class LightningBolt(BossModule module) : Components.GenericAOEs(module)
{
    private AOEInstance? _aoe;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.DarkCloud)
            _aoe = new(CloudCall.Circle, actor.Position, default, WorldState.FutureTime(7.8f));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.LightningBolt)
            _aoe = null;
    }
}

class D132PoqhirajStates : StateMachineBuilder
{
    public D132PoqhirajStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<GallopKB>()
            .ActivateOnEnter<GallopKBHint>()
            .ActivateOnEnter<ArenaChanges>()
            .ActivateOnEnter<GallopAOE>()
            .ActivateOnEnter<Touchdown>()
            .ActivateOnEnter<BurningBright>()
            .ActivateOnEnter<CloudCall>()
            .ActivateOnEnter<LightningBolt>()
            .ActivateOnEnter<RearHoof>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 171, NameID = 4952, SortOrder = 4)]
public class D132Poqhiraj(WorldState ws, Actor primary) : BossModule(ws, primary, ArenaCenter, new ArenaBoundsRect(4.5f, 19.75f))
{
    public static readonly WPos ArenaCenter = new(400, 104.166f);
}
