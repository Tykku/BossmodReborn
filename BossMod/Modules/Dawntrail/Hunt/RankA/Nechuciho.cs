namespace BossMod.Dawntrail.Hunt.RankA.Nechuciho;

public enum OID : uint
{
    Boss = 0x452C // R3.42
}

public enum AID : uint
{
    AutoAttack = 872, // Boss->player, no cast, single-target
    WordOfTheWood = 39872, // Boss->self, 5.0s cast, range 30 180.000-degree cone
    WhisperOfTheWood = 39495, // Boss->self, 3.0s cast, single-target
    WordOfTheWood2 = 39496, // Boss->self, 7.0s cast, single-target
    WordOfTheWood3 = 39786, // Boss->self, no cast, range 30 180.000-degree cone
    WordOfTheWood4 = 39787, // Boss->self, no cast, range 30 180.000-degree cone
    WordOfTheWood5 = 39788, // Boss->self, no cast, range 30 180.000-degree cone
    WordOfTheWood6 = 39785, // Boss->self, no cast, range 30 180.000-degree cone
    WhisperOfTheWood2 = 39494, // Boss->self, 3.0s cast, single-target
    SentinelRoar = 39491, // Boss->self, 5.0s cast, range 40 circle
    Level5DeathSentence = 39492, // Boss->self, 5.0s cast, range 30 circle
}

public enum SID : uint
{
    ForwardOmen = 4153,
    RearwardOmen = 4154,
    LeftwardOmen = 4155,
    RightwardOmen = 4156
}

class Level5DeathSentence(BossModule module) : Components.CastInterruptHint(module, ActionID.MakeSpell(AID.Level5DeathSentence), true, false, "Applies Doom");
class SentinelRoar(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.SentinelRoar));
class WordOfTheWood(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.WordOfTheWood), new AOEShapeCone(30, 90.Degrees()));

class WhispersOfTheWood(BossModule module) : Components.GenericRotatingAOE(module)
{
    private static readonly AOEShapeCone _coneShape = new(30, 90.Degrees());
    private readonly List<Angle> _omenDirections = [];
    private static readonly HashSet<uint> _relevantOmens = [(uint)SID.ForwardOmen, (uint)SID.RearwardOmen, (uint)SID.LeftwardOmen, (uint)SID.RightwardOmen];

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if (actor != Module.PrimaryActor || !_relevantOmens.Contains(status.ID))
            return;

        var directionChange = status.ID switch
        {
            (uint)SID.ForwardOmen => 0.Degrees(),
            (uint)SID.RearwardOmen => 180.Degrees(),
            (uint)SID.LeftwardOmen => 90.Degrees(),
            (uint)SID.RightwardOmen => -90.Degrees(),
            _ => 0.Degrees()
        };

        _omenDirections.Add(directionChange);
        var newFacingDirection = actor.Rotation;
        _omenDirections.ForEach(change => newFacingDirection += change);
        Sequences.Add(new(_coneShape, actor.Position, newFacingDirection, directionChange, WorldState.FutureTime(15), 0, 1));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (caster == Module.PrimaryActor && (AID)spell.Action.ID is AID.WordOfTheWood3 or AID.WordOfTheWood4 or AID.WordOfTheWood5 or AID.WordOfTheWood6)
        {
            AdvanceSequence(caster.Position, caster.Rotation, WorldState.CurrentTime);
            _omenDirections.Clear();
        }
    }

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = 0;
        foreach (var s in Sequences)
        {
            if (s.NumRemainingCasts <= 0)
                continue;
            yield return new(s.Shape, s.Origin, s.Rotation, s.NextActivation, count == 0 ? ImminentColor : FutureColor, count == 0);
            if (++count >= 2)
                break;
        }
    }

    public new void AdvanceSequence(int index, DateTime currentTime, bool removeWhenFinished = true)
    {
        if (index < 0 || index >= Sequences.Count)
            return;
        var s = Sequences[index];
        if (--s.NumRemainingCasts <= 0 && removeWhenFinished)
            Sequences.RemoveAt(index);
        else
        {
            s.Rotation += s.Increment;
            s.NextActivation = currentTime.AddSeconds(s.SecondsBetweenActivations);
            Sequences[index] = s;
        }
    }

    public new bool AdvanceSequence(WPos origin, Angle rotation, DateTime currentTime, bool removeWhenFinished = true)
    {
        var index = Sequences.FindIndex(s => s.Origin.AlmostEqual(origin, 1) && s.Rotation.AlmostEqual(rotation, 0.05f));
        if (index < 0)
            return false;
        AdvanceSequence(index, currentTime, removeWhenFinished);
        return true;
    }
}

class NechucihoStates : StateMachineBuilder
{
    public NechucihoStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<Level5DeathSentence>()
            .ActivateOnEnter<SentinelRoar>()
            .ActivateOnEnter<WordOfTheWood>()
            .ActivateOnEnter<WhispersOfTheWood>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Contributed, Contributors = "Shinryin", GroupType = BossModuleInfo.GroupType.Hunt, GroupID = (uint)BossModuleInfo.HuntRank.A, NameID = 13362)]
public class Nechuciho(WorldState ws, Actor primary) : SimpleBossModule(ws, primary);
