namespace BossMod.Endwalker.DeepDungeon.EurekaOrthos.DD60ServomechanicalMinotaur16;

public enum OID : uint
{
    Boss = 0x3DA1, // R6.0
    BallOfLevin = 0x3DA2, // R1.3
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 6499, // Boss->player, no cast, single-target
    OctupleSwipeTelegraph = 31867, // Helper->self, 1.0s cast, range 40 90-degree cone
    OctupleSwipe = 31872, // Boss->self, 10.8s cast, range 40 90-degree cone
    BullishSwipe1 = 31868, // Boss->self, no cast, range 40 90-degree cone
    BullishSwipe2 = 31869, // Boss->self, no cast, range 40 90-degree cone
    BullishSwipe3 = 31870, // Boss->self, no cast, range 40 90-degree cone
    BullishSwipe4 = 31871, // Boss->self, no cast, range 40 90-degree cone
    DisorientingGroan = 31876, // Boss->self, 5.0s cast, range 60 circle, knockback 15, away from center
    BullishSwipe = 32795, // Boss->self, 5.0s cast, range 40 90-degree cone
    Thundercall = 31873, // Boss->self, 5.0s cast, range 60 circle
    Shock = 31874, // BallOfLevin->self, 2.5s cast, range 5 circle
    BullishSwing = 31875 // Boss->self, 5.0s cast, range 13 circle
}

class OctupleSwipe(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];
    private static readonly AOEShapeCone cone = new(40, 45.Degrees());
    private static readonly HashSet<AID> castEnd = [AID.OctupleSwipe, AID.BullishSwipe1, AID.BullishSwipe2, AID.BullishSwipe3, AID.BullishSwipe4];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_aoes.Count > 0)
            yield return _aoes[0] with { Color = Colors.Danger };
        if (_aoes.Count > 1)
            for (var i = 1; i < Math.Clamp(_aoes.Count, 0, 3); ++i)
                yield return _aoes[i];
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.OctupleSwipeTelegraph)
            _aoes.Add(new(cone, caster.Position, spell.Rotation, _aoes.Count == 0 ? Module.CastFinishAt(spell, 8.7f + 2 * _aoes.Count) : _aoes[0].Activation.AddSeconds(_aoes.Count * 2)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (_aoes.Count > 0 && castEnd.Contains((AID)spell.Action.ID))
            _aoes.RemoveAt(0);
    }
}

class BullishSwing(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BullishSwing), new AOEShapeCircle(13));
class BullishSwipe(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BullishSwipe), new AOEShapeCone(40, 45.Degrees()));

class DisorientingGroan(BossModule module) : Components.KnockbackFromCastTarget(module, ActionID.MakeSpell(AID.DisorientingGroan), 15)
{
    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var source = Sources(slot, actor).FirstOrDefault();
        if (source != default)
            hints.AddForbiddenZone(ShapeDistance.InvertedCircle(Arena.Center, 5), source.Activation);
    }
}

class Shock(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];
    private static readonly AOEShapeCircle circle = new(5);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.BallOfLevin)
            _aoes.Add(new(circle, actor.Position, default, WorldState.FutureTime(13)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.Shock)
            _aoes.Clear();
    }
}

class DD60ServomechanicalMinotaur16States : StateMachineBuilder
{
    public DD60ServomechanicalMinotaur16States(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<OctupleSwipe>()
            .ActivateOnEnter<BullishSwipe>()
            .ActivateOnEnter<BullishSwing>()
            .ActivateOnEnter<DisorientingGroan>()
            .ActivateOnEnter<Shock>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 902, NameID = 12267)]
public class DD60ServomechanicalMinotaur16(WorldState ws, Actor primary) : BossModule(ws, primary, new(-600, -300), new ArenaBoundsCircle(20));
