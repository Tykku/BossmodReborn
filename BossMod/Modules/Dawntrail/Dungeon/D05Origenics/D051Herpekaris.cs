namespace BossMod.Dawntrail.Dungeon.D05Origenics.D051Herpekaris;

public enum OID : uint
{
    Boss = 0x4185, // R8.4
    PoisonVoidzone = 0x1E9E3C, // R0.5
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 872, // Boss->player, no cast, single-target
    Teleport = 36520, // Boss->location, no cast, single-target

    StridentShriek = 36519, // Boss->self, 5.0s cast, range 60 circle  raidwide

    Vasoconstrictor = 36459, // Boss->self, 3.0+1.2s cast, single-target, spawns poison
    PoisonHeartVoidzone = 36460, // Helper->location, 4.2s cast, range 2 circle
    PoisonHeartSpread = 37921, // Helper->player, 8.0s cast, range 5 circle

    PodBurst1 = 38518, // Helper->location, 5.0s cast, range 6 circle
    PodBurst2 = 38519, // Helper->location, 4.0s cast, range 6 circle

    Venomspill1 = 37451, // Boss->self, 5.0s cast, single-target
    Venomspill2 = 36452, // Boss->self, 5.0s cast, single-target
    Venomspill3 = 36453, // Boss->self, 4.0s cast, single-target
    Venomspill4 = 36454, // Boss->self, 4.0s cast, single-target

    WrithingRiotVisual = 36463, // Boss->self, 9.0s cast, single-target
    RightSweepTelegraph = 36465, // Helper->self, 2.0s cast, range 25 210-degree cone
    LeftSweepTelegraph = 36466, // Helper->self, 2.0s cast, range 25 210-degree cone
    RearSweepTelegraph = 36467, // Helper->self, 2.0s cast, range 25 90-degree cone
    RightSweep = 36469, // Boss->self, no cast, range 25 210-degree cone
    LeftSweep = 36470, // Boss->self, no cast, range 25 210-degree cone
    RearSweep = 36471, // Boss->self, no cast, range 25 90-degree cone

    CollectiveAgonyMarker = 36474, // Helper->player, no cast, single-target
    CollectiveAgony = 36473, // Boss->self/players, 5.5s cast, range 50 width 8 rect
    ConvulsiveCrush = 36518, // Boss->player, 5.0s cast, single-target, tb
}

class CollectiveAgony(BossModule module) : Components.LineStack(module, ActionID.MakeSpell(AID.CollectiveAgonyMarker), ActionID.MakeSpell(AID.CollectiveAgony), 5.6f);
class StridentShriek(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.StridentShriek));
class ConvulsiveCrush(BossModule module) : Components.SingleTargetDelayableCast(module, ActionID.MakeSpell(AID.ConvulsiveCrush));
class PoisonHeartSpread(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.PoisonHeartSpread), 5);
class PoisonHeartVoidzone(BossModule module) : Components.PersistentVoidzoneAtCastTarget(module, 2, ActionID.MakeSpell(AID.PoisonHeartVoidzone), m => m.Enemies(OID.PoisonVoidzone).Where(z => z.EventState != 7), 0.9f);

class PodBurst(BossModule module, AID aid) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(aid), 6);
class PodBurst1(BossModule module) : PodBurst(module, AID.PodBurst1);
class PodBurst2(BossModule module) : PodBurst(module, AID.PodBurst2);

class WrithingRiot(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];
    private static readonly AOEShapeCone coneLeftRight = new(25, 105.Degrees());
    private static readonly AOEShapeCone coneRear = new(25, 45.Degrees());

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_aoes.Count > 0)
            yield return _aoes[0] with { Color = Colors.Danger };
        if (_aoes.Count > 1)
            yield return _aoes[1] with { Risky = _aoes[0].Shape != _aoes[1].Shape };
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        var activation = WorldState.FutureTime(9.2f + _aoes.Count * 2);
        switch ((AID)spell.Action.ID)
        {
            case AID.RightSweepTelegraph:
            case AID.LeftSweepTelegraph:
                AddAOE(coneLeftRight, caster.Position, spell.Rotation, activation);
                break;
            case AID.RearSweepTelegraph:
                AddAOE(coneRear, caster.Position, spell.Rotation, activation);
                break;
        }
    }

    private void AddAOE(AOEShape shape, WPos position, Angle rotation, DateTime activation)
    {
        _aoes.Add(new(shape, position, rotation, activation));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (_aoes.Count > 0)
            switch ((AID)spell.Action.ID)
            {
                case AID.RearSweep:
                case AID.LeftSweep:
                case AID.RightSweep:
                    _aoes.RemoveAt(0);
                    break;
            }
    }
}

class D051HerpekarisStates : StateMachineBuilder
{
    public D051HerpekarisStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<CollectiveAgony>()
            .ActivateOnEnter<StridentShriek>()
            .ActivateOnEnter<PoisonHeartSpread>()
            .ActivateOnEnter<PoisonHeartVoidzone>()
            .ActivateOnEnter<PodBurst1>()
            .ActivateOnEnter<PodBurst2>()
            .ActivateOnEnter<WrithingRiot>()
            .ActivateOnEnter<ConvulsiveCrush>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 825, NameID = 12741, SortOrder = 1)]
public class D051Herpekaris(WorldState ws, Actor primary) : BossModule(ws, primary, new(-88, -180), new ArenaBoundsSquare(17.5f));
