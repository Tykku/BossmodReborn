using BossMod.Components;

namespace BossMod.RealmReborn.Dungeon.D27SastashaHard.D271Karlabos;

public enum OID : uint
{
    Boss = 0xCBA,
    Tail = 0xD08
}

public enum AID : uint
{
    AutoAttack = 872, // Boss->player, no cast, single-target
    Impale = 3054, // Boss->player, no cast, single-target
    MarkOfDeath = 3055, // Boss->player, no cast, single-target
    Inspire = 3056, // Boss->player, no cast, single-target
    StunClaws = 3057, // Boss->player, no cast, single-target
    Expire = 3058, // Boss->self, no cast, single-target
    AquaBall = 3059, // D08->player, no cast, range 5 circle
    TailScrew = 3060, // Boss->player, 3.0s cast, single-target
    WildInstinct = 3061, // Boss->self, no cast, single-target
}

public enum SID : uint
{
    Slime = 569, // Tail->player, extra=0x0
    Haste = 226, // Boss->Boss, extra=0x0
    DamageUp = 290, // Boss->Boss, extra=0x0
    Prey = 420, // Boss->player, extra=0x0
}

class MarkOfDeath(BossModule module) : SingleTargetInstant(module, (uint)AID.MarkOfDeath)
{
    private BitMask _marked;

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if (status.ID == (uint)SID.Prey)
        {
            _marked.Set(Raid.FindSlot(actor.InstanceID));
        }
    }

    public override void OnStatusLose(Actor actor, ActorStatus status)
    {
        if (status.ID == (uint)SID.Prey)
        {
            _marked.Clear(Raid.FindSlot(actor.InstanceID));
        }
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        // Tanks should pass the stun to a DPS so they can stun TailScrew later
        if (_marked[slot] && actor.Role == Role.Tank)
        {
            hints.Add("Run through a DPS!");
        }
    }
}

// Cast is vulnerable to stun, but not interrupt
class TailScrew(BossModule module) : CastInterruptHint(module, (uint)AID.TailScrew, false, true);

class AquaBall(BossModule module) : BossComponent(module)
{
    // A non-distinct list of actors with the debuff
    private readonly List<Actor> _slimed = [];

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if (status.ID == (uint)SID.Slime)
        {
            _slimed.Add(actor);
        }
    }

    public override void OnStatusLose(Actor actor, ActorStatus status)
    {
        if (status.ID == (uint)SID.Slime)
            _slimed.Remove(actor);
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (_slimed.Count > 0 && actor.Role == Role.Healer)
        {
            // A player can have more than one stack of the debuff, but only print their name once
            foreach (var a in _slimed.DistinctBy(a => a.InstanceID))
            {
                hints.Add($"Esuna {a.Name} ASAP!");
            }
        }
        else if (_slimed.Contains(actor) && actor.Class == Class.BRD)
        {
            hints.Add("Cleanse self with Warden's Paean ASAP!");
        }
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (actor.Role == Role.Healer)
        {
            // the same player may appear twice
            _slimed.ForEach(s => hints.ActionsToExecute.Push(ActionID.MakeSpell(ClassShared.AID.Esuna), s,
                ActionQueue.Priority.High, castTime: 1));
        }
        // BRD cast WardensPaean
        else if (actor.Class == Class.BRD)
        {
            // BRD should prioritize themselves first, if applicable
            if (_slimed.Contains(actor))
            {
                hints.ActionsToExecute.Push(ActionID.MakeSpell(BRD.AID.WardensPaean), actor, ActionQueue.Priority.High);
            }
            // Otherwise, select anybody to cleanse to assist healer
            else
            {
                hints.ActionsToExecute.Push(ActionID.MakeSpell(BRD.AID.WardensPaean), _slimed.First(),
                    ActionQueue.Priority.High);
            }
        }
    }
}

class D271KarlabosStates : StateMachineBuilder
{
    public D271KarlabosStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<AquaBall>()
            .ActivateOnEnter<TailScrew>()
            .ActivateOnEnter<MarkOfDeath>()
            ;
    }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP, GroupType = BossModuleInfo.GroupType.CFC, GroupID = 28, NameID = 3014,
    Contributors = "Zaventh")]
public class D271Karlabos(WorldState ws, Actor primary)
    : BossModule(ws, primary, new(166, -85), new ArenaBoundsCircle(21));
