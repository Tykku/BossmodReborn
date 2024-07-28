namespace BossMod.Dawntrail.Hunt.RankS.AtticusThePrimogenitor;

public enum OID : uint
{
    Boss = 0x416D // R4.7
}

public enum AID : uint
{
    AutoAttack = 39015, // Boss->player, no cast, single-target
    HeadSpeaks = 39883, // Boss->self, no cast, single-target
    HeadSpeaks2 = 39884, // Boss->self, no cast, single-target
    BreathSequenceFirstFront = 39003, // Boss->self, 5.0s cast, range 60 120-degree cone
    BreathSequenceFirstRight = 39004, // Boss->self, 5.0s cast, range 60 120-degree cone
    BreathSequenceFirstLeft = 39005, // Boss->self, 5.0s cast, range 60 120-degree cone
    BreathSequenceRestFront = 39009, // Boss->self, no cast, range 60 120-degree cone
    BreathSequenceRestRight = 39010, // Boss->self, no cast, range 60 120-degree cone
    BreathSequenceRestLeft = 39011, // Boss->self, no cast, range 60 120-degree cone
    Intimidation = 39014, // Boss->self, 4.0s cast, range 40 circle
    Brutality = 39012, // Boss->self, 3.0s cast, single-target, applies Haste
    PyricBlast = 39013 // Boss->players, 5.0s cast, range 6 circle, stack
}

public enum NPCYell : ushort
{
    AttackThreeTimes1 = 16904,
    AttackSixTimes1 = 16905,
    Brutality = 16906,
    FrontHead1 = 16881,
    FrontHead2 = 16882,
    FrontHead3 = 16888,
    FrontHead4 = 16889,
    FrontHead5 = 16874,
    FrontHead6 = 16896,
    FrontHead7 = 16885,
    FrontHead8 = 16900,
    FrontHead9 = 16897,
    FrontHead10 = 16903,
    RightHead1 = 16879,
    RightHead2 = 16887,
    RightHead3 = 16893,
    RightHead4 = 16875,
    RightHead5 = 16894,
    RightHead6 = 16886,
    RightHead7 = 16901,
    RightHead8 = 16878,
    RightHead9 = 16902,
    RightHead10 = 16890,
    LeftHead1 = 16883,
    LeftHead2 = 16880,
    LeftHead3 = 16884,
    LeftHead4 = 16892,
    LeftHead5 = 16876,
    LeftHead6 = 16895,
    LeftHead7 = 16899,
    LeftHead8 = 16877,
    LeftHead9 = 16898,
    LeftHead10 = 16891
}

class PyricBlast(BossModule module) : Components.StackWithCastTargets(module, ActionID.MakeSpell(AID.PyricBlast), 6, 8);
class Intimidation(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.Intimidation));
class Brutality(BossModule module) : Components.CastHint(module, ActionID.MakeSpell(AID.Brutality), "Applies Haste");

class BreathSequence(BossModule module) : Components.GenericAOEs(module)
{
    private List<AOEInstance> _aoes = [];
    private static readonly Angle angle = 120.Degrees();
    private static readonly AOEShapeCone cone = new(40, 60.Degrees());

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_aoes.Count > 0)
            yield return _aoes[0] with { Color = Colors.Danger };
        if (_aoes.Count > 1)
            yield return _aoes[1];
    }

    public override void OnActorNpcYell(Actor actor, ushort id)
    {
        var activation = Module.WorldState.FutureTime(10.5f); // placeholder, gets replaced when sequence starts
        switch (id)
        {
            case (ushort)NPCYell.FrontHead1:
            case (ushort)NPCYell.FrontHead2:
            case (ushort)NPCYell.FrontHead3:
            case (ushort)NPCYell.FrontHead4:
            case (ushort)NPCYell.FrontHead5:
            case (ushort)NPCYell.FrontHead6:
            case (ushort)NPCYell.FrontHead7:
            case (ushort)NPCYell.FrontHead8:
            case (ushort)NPCYell.FrontHead9:
            case (ushort)NPCYell.FrontHead10:
                _aoes.Add(new(cone, actor.Position, actor.Rotation, activation));
                break;
            case (ushort)NPCYell.LeftHead1:
            case (ushort)NPCYell.LeftHead2:
            case (ushort)NPCYell.LeftHead3:
            case (ushort)NPCYell.LeftHead4:
            case (ushort)NPCYell.LeftHead5:
            case (ushort)NPCYell.LeftHead6:
            case (ushort)NPCYell.LeftHead7:
            case (ushort)NPCYell.LeftHead8:
            case (ushort)NPCYell.LeftHead9:
            case (ushort)NPCYell.LeftHead10:
                _aoes.Add(new(cone, actor.Position, actor.Rotation + angle, activation));
                break;
            case (ushort)NPCYell.RightHead1:
            case (ushort)NPCYell.RightHead2:
            case (ushort)NPCYell.RightHead3:
            case (ushort)NPCYell.RightHead4:
            case (ushort)NPCYell.RightHead5:
            case (ushort)NPCYell.RightHead6:
            case (ushort)NPCYell.RightHead7:
            case (ushort)NPCYell.RightHead8:
            case (ushort)NPCYell.RightHead9:
            case (ushort)NPCYell.RightHead10:
                _aoes.Add(new(cone, actor.Position, actor.Rotation - angle, activation));
                break;
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (_aoes.Count > 0)
            switch ((AID)spell.Action.ID)
            {
                case AID.BreathSequenceFirstFront:
                case AID.BreathSequenceFirstLeft:
                case AID.BreathSequenceFirstRight:
                    var updatedAOEs = new List<AOEInstance>();
                    for (var i = 0; i < _aoes.Count; i++)
                    {
                        var a = _aoes[i];
                        var activationTime = 2.3f * i;
                        var updatedAOE = new AOEInstance(a.Shape, a.Origin, a.Rotation, Module.CastFinishAt(spell, activationTime));
                        updatedAOEs.Add(updatedAOE);
                    }
                    _aoes = updatedAOEs;
                    break;
            }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (_aoes.Count > 0)
            switch ((AID)spell.Action.ID)
            {
                case AID.BreathSequenceFirstFront:
                case AID.BreathSequenceFirstLeft:
                case AID.BreathSequenceFirstRight:
                    _aoes.RemoveAt(0);
                    break;
            }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (_aoes.Count > 0)
            switch ((AID)spell.Action.ID)
            {
                case AID.BreathSequenceRestFront:
                case AID.BreathSequenceRestLeft:
                case AID.BreathSequenceRestRight:
                    _aoes.RemoveAt(0);
                    break;
            }
    }
}

class AtticusThePrimogenitorStates : StateMachineBuilder
{
    public AtticusThePrimogenitorStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<BreathSequence>()
            .ActivateOnEnter<Brutality>()
            .ActivateOnEnter<Intimidation>()
            .ActivateOnEnter<PyricBlast>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.Hunt, GroupID = (uint)BossModuleInfo.HuntRank.S, NameID = 13156)]
public class AtticusThePrimogenitor(WorldState ws, Actor primary) : SimpleBossModule(ws, primary);
