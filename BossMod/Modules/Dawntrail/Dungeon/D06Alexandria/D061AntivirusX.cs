namespace BossMod.Dawntrail.Dungeon.D06Alexandria.D061AntivirusX;

public enum OID : uint
{
    Boss = 0x4173, // R8.0
    ElectricCharge = 0x18D6, // R0.5
    InterferonC = 0x4175, // R1.0
    InterferonR = 0x4174, // R1.0
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 36388, // Boss->player, no cast, single-target

    ImmuneResponseVisualSmall = 36378, // Boss->self, 5.0s cast, single-target
    ImmuneResponseSmall = 36379, // Helper->self, 6.0s cast, range 40 120-degree cone, frontal AOE cone
    ImmuneResponseVisualBig = 36380, // Boss->self, 5.0s cast, single-target
    ImmuneResponseBig = 36381, // Helper->self, 6.0s cast, range 40 240-degree cone, side and back AOE cone

    PathocrossPurge = 36383, // InterferonC->self, 1.0s cast, range 40 width 6 cross
    PathocircuitPurge = 36382, // InterferonR->self, 1.0s cast, range 4-40 donut

    QuarantineVisual = 36384, // Boss->self, 3.0s cast, single-target
    Quarantine = 36386, // Helper->players, no cast, range 6 circle, stack
    Disinfection = 36385, // Helper->player, no cast, range 6 circle, tankbuster cleave

    Cytolysis = 36387, // Boss->self, 5.0s cast, range 40 circle
}

public enum IconID : uint
{
    Tankbuster = 344, // player
    Stackmarker = 62, // player
}

class ImmuneResponseArenaChange(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCustom rect = new([new Rectangle(D061AntivirusX.ArenaCenter, 23, 18)], [new Rectangle(D061AntivirusX.ArenaCenter, 20, 15)]);
    private AOEInstance? _aoe;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.ImmuneResponseVisualSmall && Module.Arena.Bounds == D061AntivirusX.StartingBounds)
            _aoe = new(rect, Module.Center, default, Module.CastFinishAt(spell, 0.8f));
    }

    public override void OnEventEnvControl(byte index, uint state)
    {
        if (state == 0x00020001 && index == 0x03)
        {
            Module.Arena.Bounds = D061AntivirusX.DefaultBounds;
            _aoe = null;
        }
    }
}

class PathoCircuitCrossPurge(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeDonut donut = new(4, 40);
    private static readonly AOEShapeCross cross = new(40, 3);
    private static readonly AOEShapeCone coneSmall = new(40, 60.Degrees());
    private static readonly AOEShapeCone coneBig = new(40, 120.Degrees());
    private readonly List<AOEInstance> _aoes = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_aoes.Count > 0)
            yield return _aoes[0] with { Color = _aoes.Count > 1 ? Colors.Danger : Colors.AOE };
        if (_aoes.Count > 1)
        {
            var isDonuts = _aoes[0].Shape == donut && _aoes[1].Shape == donut;
            var isConeWithDelay = (_aoes[1].Shape == coneBig || _aoes[1].Shape == coneSmall) && (_aoes[1].Activation - _aoes[0].Activation).TotalSeconds > 2;
            var isCross = _aoes[0].Shape == cross;
            var isFrontDonutAndConeSmall = _aoes[1].Origin == new WPos(852, 823) && _aoes[1].Shape == donut && _aoes[0].Shape == coneSmall;
            var isRisky = !isDonuts && !isConeWithDelay && !isFrontDonutAndConeSmall || isCross;
            yield return _aoes[1] with { Risky = isRisky };
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.ImmuneResponseBig or AID.ImmuneResponseSmall)
        {
            var coneType = spell.Action.ID == (int)AID.ImmuneResponseBig ? coneBig : coneSmall;
            AddAOE(new(coneType, caster.Position, spell.Rotation, Module.CastFinishAt(spell)));
        }
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID is OID.InterferonR or OID.InterferonC)
        {
            AOEShape shape = actor.OID == (int)OID.InterferonR ? donut : cross;
            var activationTime = _aoes.Count == 0 ? Module.WorldState.FutureTime(9.9f) : _aoes[0].Activation.AddSeconds(2.5f * _aoes.Count);
            AddAOE(new(shape, actor.Position, actor.Rotation, activationTime));
        }
    }

    private void AddAOE(AOEInstance aoe)
    {
        _aoes.Add(aoe);
        _aoes.SortBy(x => x.Activation);
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.PathocrossPurge:
            case AID.PathocircuitPurge:
            case AID.ImmuneResponseBig:
            case AID.ImmuneResponseSmall:
                _aoes.RemoveAt(0);
                break;
        }
    }
}

class Cytolysis(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.Cytolysis));

class Quarantine(BossModule module) : Components.StackWithIcon(module, (uint)IconID.Stackmarker, ActionID.MakeSpell(AID.Quarantine), 6, 5.1f, 3, 3)
{
    private readonly Disinfection _tb = module.FindComponent<Disinfection>()!;
    public override void Update()
    {
        if (!ActiveStacks.Any())
            return;
        var forbidden = Raid.WithSlot().WhereActor(p => _tb.ActiveBaits.Any(x => x.Target == p)).Mask();
        foreach (ref var t in Stacks.AsSpan())
            t.ForbiddenPlayers = forbidden;
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (!_tb.CurrentBaits.Any(x => x.Target == actor) && actor == ActiveStacks.FirstOrDefault().Target)
        {
            var party = Raid.WithoutSlot().Where(x => !x.IsDead);
            List<Actor> exclude = [actor, _tb.CurrentBaits[0].Target];
            var closestAlly = party.Exclude(exclude).Closest(actor.Position);
            if (closestAlly != null)
                hints.AddForbiddenZone(ShapeDistance.InvertedCircle(closestAlly.Position, 3), ActiveStacks.First().Activation);
        }
        else
            base.AddAIHints(slot, actor, assignment, hints);
    }
}

class Disinfection(BossModule module) : Components.BaitAwayIcon(module, new AOEShapeCircle(6), (uint)IconID.Tankbuster, ActionID.MakeSpell(AID.Disinfection), centerAtTarget: true)
{
    public override void AddGlobalHints(GlobalHints hints)
    {
        if (CurrentBaits.Count > 0)
            hints.Add("Tankbuster cleave");
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (!CurrentBaits.Any(x => x.Target == actor) && Module.FindComponent<Quarantine>()!.ActiveStacks.Any(x => x.Activation.AddSeconds(-2) >= Module.WorldState.CurrentTime))
        { }
        else
            base.AddAIHints(slot, actor, assignment, hints);
    }
}

class D061AntivirusXStates : StateMachineBuilder
{
    public D061AntivirusXStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<ImmuneResponseArenaChange>()
            .ActivateOnEnter<PathoCircuitCrossPurge>()
            .ActivateOnEnter<Disinfection>()
            .ActivateOnEnter<Quarantine>()
            .ActivateOnEnter<Cytolysis>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS), erdelf", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 827, NameID = 12844)]
public class D061AntivirusX(WorldState ws, Actor primary) : BossModule(ws, primary, ArenaCenter, StartingBounds)
{
    public static readonly WPos ArenaCenter = new(852, 823);
    public static readonly ArenaBoundsRect StartingBounds = new(22.5f, 17.5f);
    public static readonly ArenaBoundsRect DefaultBounds = new(20, 15);
}
