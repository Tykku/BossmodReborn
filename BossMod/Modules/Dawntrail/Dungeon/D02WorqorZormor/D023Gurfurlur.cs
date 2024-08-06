namespace BossMod.Dawntrail.Dungeon.D02WorqorZormor.D023Gurfurlur;

public enum OID : uint
{
    Boss = 0x415F, // R7.000, x1
    AuraSphere = 0x4162, // R1.0
    BitingWind = 0x4160, // R1.0
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 872, // Boss->player, no cast, single-target

    HeavingHaymakerVisual = 36269, // Boss->self, 5.0s cast, single-target
    HeavingHaymaker = 36375, // Helper->self, 5.3s cast, range 60 circle

    Stonework = 36301, // Boss->self, 3.0s cast, single-target
    LithicImpact = 36302, // Helper->self, 6.8s cast, range 4 width 4 rect
    GreatFlood = 36307, // Helper->self, 7.0s cast, range 80 width 60 rect

    Allfire1 = 36303, // Helper->self, 7.0s cast, range 10 width 10 rect
    Allfire2 = 36304, // Helper->self, 8.5s cast, range 10 width 10 rect
    Allfire3 = 36305, // Helper->self, 10.0s cast, range 10 width 10 rect

    VolcanicDrop = 36306, // Helper->player, 5.0s cast, range 6 circle

    SledgeHammerMarker = 36315, // Helper->player, no cast, single-target
    Sledgehammer1 = 36313, // Boss->self/players, 5.0s cast, range 60 width 8 rect, line stack
    Sledgehammer2 = 36314, // Boss->self, no cast, range 60 width 8 rect
    Sledgehammer3 = 39260, // Boss->self, no cast, range 60 width 8 rect

    ArcaneStomp = 36319, // Boss->self, 3.0s cast, single-target

    ShroudOfEons1 = 36321, // AuraSphere->player, no cast, single-target
    ShroudOfEons2 = 36322, // AuraSphere->Boss, no cast, single-target

    EnduringGlory = 36320, // Boss->self, 6.0s cast, range 60 circle

    Windswrath1 = 36310, // Helper->self, 7.0s cast, range 40
    Windswrath2 = 39074, // Helper->self, 15.0s cast, range 40 circle

    Whirlwind = 36311 // Helper->self, no cast, range 5 circle
}

class HeavingHaymakerArenaChange(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCustom square = new([new Square(D023Gurfurlur.ArenaCenter, 25)], [new Square(D023Gurfurlur.ArenaCenter, 20)]);
    private AOEInstance? _aoe;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.HeavingHaymaker && Module.Arena.Bounds == D023Gurfurlur.StartingBounds)
            _aoe = new(square, Module.Center, default, Module.CastFinishAt(spell, 0.6f));
    }

    public override void OnEventEnvControl(byte index, uint state)
    {
        if (state == 0x00020001 && index == 0x18)
        {
            Module.Arena.Bounds = D023Gurfurlur.DefaultBounds;
            _aoe = null;
        }
    }
}

class Whirlwind(BossModule module) : Components.PersistentVoidzone(module, 5, m => m.Enemies(OID.BitingWind))
{
    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        base.AddAIHints(slot, actor, assignment, hints);
        foreach (var w in ActiveAOEs(slot, actor))
            hints.AddForbiddenZone(new AOEShapeCircle(5), w.Origin + 3 * w.Rotation.ToDirection());
    }
}

class AuraSphere(BossModule module) : BossComponent(module)
{
    private readonly IReadOnlyList<Actor> _orbs = module.Enemies(OID.AuraSphere);

    private IEnumerable<Actor> ActiveOrbs => _orbs.Where(orb => !orb.IsDead);

    public override void AddGlobalHints(GlobalHints hints)
    {
        if (ActiveOrbs.Any())
            hints.Add("Soak the orbs!");
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var orb = ActiveOrbs.FirstOrDefault();
        var orbs = new List<Func<WPos, float>>();
        if (ActiveOrbs.Any())
        {
            hints.ActionsToExecute.Push(ActionID.MakeSpell(ClassShared.AID.Sprint), actor, ActionQueue.Priority.High);
            foreach (var o in ActiveOrbs)
                orbs.Add(ShapeDistance.InvertedCircle(o.Position + 0.5f * o.Rotation.ToDirection(), 0.5f));
        }
        if (orbs.Count > 0)
            hints.AddForbiddenZone(p => orbs.Select(f => f(p)).Max());
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        foreach (var orb in ActiveOrbs)
            Arena.AddCircle(orb.Position, 1.4f, Colors.Safe);
    }
}

class SledgeHammer(BossModule module) : Components.LineStack(module, ActionID.MakeSpell(AID.SledgeHammerMarker), ActionID.MakeSpell(AID.Sledgehammer3), 4.9f);
class HeavingHaymaker(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.HeavingHaymaker));
class LithicImpact(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.LithicImpact), new AOEShapeRect(4, 2));

class GreatFlood(BossModule module) : Components.KnockbackFromCastTarget(module, ActionID.MakeSpell(AID.GreatFlood), 25, kind: Kind.DirForward)
{
    public (WPos, Angle, DateTime) Data;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        base.OnCastStarted(caster, spell);
        if (spell.Action == WatchedAction)
            Data = (caster.Position, caster.Rotation, Module.CastFinishAt(spell, 1));
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var source = Sources(slot, actor).FirstOrDefault();
        var component = Module.FindComponent<Allfire>()!.ActiveAOEs(slot, actor).Any();
        if (!component && (source != default || Data.Item3 > Module.WorldState.CurrentTime)) // 1s delay to wait for action effect
            hints.AddForbiddenZone(ShapeDistance.InvertedRect(Data.Item1, Data.Item2, 12, 0, 20), Data.Item3.AddSeconds(-1));
    }
}

class Allfire(BossModule module) : Components.GenericAOEs(module)
{
    private const string Risk2Hint = "Walk into safespot for knockback!";
    private const string StayHint = "Wait inside safespot for knockback!";
    private bool tutorial;
    private static readonly AOEShapeRect rect = new(5, 5, 5);
    private readonly List<AOEInstance> _aoesWave1 = [];
    private readonly List<AOEInstance> _aoesWave2 = [];
    private readonly List<AOEInstance> _aoesWave3 = [];
    private static readonly AOEShapeRect safespot = new(12, 10, InvertForbiddenZone: true);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var source = Module.FindComponent<GreatFlood>()!.Sources(slot, actor).FirstOrDefault();
        var data = Module.FindComponent<GreatFlood>()!.Data;
        if (source == default)
        {
            if (_aoesWave1.Count > 0)
                foreach (var a in _aoesWave1)
                    yield return new(a.Shape, a.Origin, a.Rotation, a.Activation, Colors.Danger);
            if (_aoesWave2.Count > 0)
                foreach (var a in _aoesWave2)
                    yield return new(a.Shape, a.Origin, a.Rotation, a.Activation, _aoesWave1.Count > 0 ? Colors.AOE : Colors.Danger, _aoesWave1.Count == 0);
            if (_aoesWave1.Count == 0 && _aoesWave3.Count > 0)
                foreach (var a in _aoesWave3)
                    yield return new(a.Shape, a.Origin, a.Rotation, a.Activation, _aoesWave2.Count > 0 ? Colors.AOE : Colors.Danger, _aoesWave2.Count == 0);
        }
        else if ((_aoesWave3.Count > 0 || _aoesWave1.Count > 0) && (source != default || Module.FindComponent<GreatFlood>()!.Data.Item3 > Module.WorldState.CurrentTime))
            yield return new(safespot, data.Item1, data.Item2, data.Item3, Colors.SafeFromAOE);

    }
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        var newAOE = new AOEInstance(rect, caster.Position, spell.Rotation, Module.CastFinishAt(spell));
        switch ((AID)spell.Action.ID)
        {
            case AID.Allfire1:
                _aoesWave1.Add(newAOE);
                break;
            case AID.Allfire2:
                _aoesWave2.Add(newAOE);
                break;
            case AID.Allfire3:
                _aoesWave3.Add(newAOE);
                break;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.Allfire1:
                _aoesWave1.Clear();
                if (!tutorial)
                    tutorial = true;
                break;
            case AID.Allfire2:
                _aoesWave2.Clear();
                break;
            case AID.Allfire3:
                _aoesWave3.Clear();
                break;
        }
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        var activeAOEs = ActiveAOEs(slot, actor).ToList();
        if (activeAOEs.Any(c => c.Shape != safespot))
            base.AddHints(slot, actor, hints);
        else
        {
            var safeSpots = activeAOEs.Where(c => c.Shape == safespot).ToList();
            if (safeSpots.Any(c => !c.Check(actor.Position)))
                hints.Add(Risk2Hint);
            else if (safeSpots.Any(c => c.Check(actor.Position)))
                hints.Add(StayHint, false);
        }
    }
}

class VolcanicDrop(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.VolcanicDrop), 6);
class EnduringGlory(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.EnduringGlory));
class Windswrath1Raidwide(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.Windswrath1));
class Windswrath2Raidwide(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.Windswrath2));
class GreatFloodRaidwide(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.GreatFlood));

class Windswrath1(BossModule module) : Components.KnockbackFromCastTarget(module, ActionID.MakeSpell(AID.Windswrath1), 15)
{
    private DateTime activation;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        base.OnCastStarted(caster, spell);
        if (spell.Action == WatchedAction)
            activation = Module.CastFinishAt(spell, 1);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (Sources(slot, actor).Any() || activation > Module.WorldState.CurrentTime) // 1s delay to wait for action effect
            hints.AddForbiddenZone(ShapeDistance.InvertedCircle(Module.Center, 5), activation.AddSeconds(-1));
    }
}

class Windswrath2(BossModule module) : Components.KnockbackFromCastTarget(module, ActionID.MakeSpell(AID.Windswrath2), 15)
{
    private DateTime activation;
    private enum Pattern { None, EWEW, WEWE }
    private Pattern CurrentPattern;
    private static readonly Angle a15 = 15.Degrees();
    private static readonly Angle a165 = 165.Degrees();
    private static readonly Angle a105 = 105.Degrees();
    private static readonly Angle a75 = 75.Degrees();

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.BitingWind)
        {
            if (actor.Position == new WPos(-74, -180))
                CurrentPattern = Pattern.EWEW;
            else if (actor.Position == new WPos(-74, -210))
                CurrentPattern = Pattern.WEWE;
        }
    }

    public override bool DestinationUnsafe(int slot, Actor actor, WPos pos) => (Module.FindComponent<Whirlwind>()?.ActiveAOEs(slot, actor).Any(z => z.Shape.Check(pos, z.Origin, z.Rotation)) ?? false) || !Module.InBounds(pos);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        base.OnCastStarted(caster, spell);
        if (spell.Action == WatchedAction)
            activation = Module.CastFinishAt(spell, 1);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var forbidden = new List<Func<WPos, float>>();
        var component = Module.FindComponent<Whirlwind>()?.ActiveAOEs(slot, actor)?.ToList();
        if (component != null && component.Count != 0 && Sources(slot, actor).Any() || activation > Module.WorldState.CurrentTime) // 1s delay to wait for action effect
        {
            base.AddAIHints(slot, actor, assignment, hints);
            var timespan = (float)(activation - Module.WorldState.CurrentTime).TotalSeconds;
            if (timespan <= 3)
            {
                var patternWEWE = CurrentPattern == Pattern.WEWE;
                forbidden.Add(ShapeDistance.InvertedCone(Module.Center - (patternWEWE ? new WDir(0, 1) : new(0, 1)), 5, patternWEWE ? a15 : -a15, a15));
                forbidden.Add(ShapeDistance.InvertedCone(Module.Center - (patternWEWE ? new WDir(0, -1) : new(0, -1)), 5, patternWEWE ? -a165 : a165, a15));
                forbidden.Add(ShapeDistance.InvertedCone(Module.Center, 5, patternWEWE ? a105 : -a105, a15));
                forbidden.Add(ShapeDistance.InvertedCone(Module.Center, 5, patternWEWE ? -a75 : a75, a15));
            }
            else
                forbidden.Add(ShapeDistance.InvertedCircle(Module.Center, 8));
            if (forbidden.Count > 0)
                hints.AddForbiddenZone(p => forbidden.Select(f => f(p)).Max(), activation.AddSeconds(-1));
        }
    }
}

class D023GurfurlurStates : StateMachineBuilder
{
    public D023GurfurlurStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<Components.StayInBounds>()
            .ActivateOnEnter<HeavingHaymakerArenaChange>()
            .ActivateOnEnter<HeavingHaymaker>()
            .ActivateOnEnter<Whirlwind>()
            .ActivateOnEnter<AuraSphere>()
            .ActivateOnEnter<LithicImpact>()
            .ActivateOnEnter<GreatFlood>()
            .ActivateOnEnter<GreatFloodRaidwide>()
            .ActivateOnEnter<Allfire>()
            .ActivateOnEnter<VolcanicDrop>()
            .ActivateOnEnter<EnduringGlory>()
            .ActivateOnEnter<SledgeHammer>()
            .ActivateOnEnter<Windswrath1>()
            .ActivateOnEnter<Windswrath1Raidwide>()
            .ActivateOnEnter<Windswrath2>()
            .ActivateOnEnter<Windswrath2Raidwide>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 824, NameID = 12705)]
public class D023Gurfurlur(WorldState ws, Actor primary) : BossModule(ws, primary, ArenaCenter, StartingBounds)
{
    public static readonly WPos ArenaCenter = new(-54, -195);
    public static readonly ArenaBoundsSquare StartingBounds = new(24.5f);
    public static readonly ArenaBoundsSquare DefaultBounds = new(20);
}
