namespace BossMod.Dawntrail.Dungeon.D10Underkeep.D101Gargant;

public enum OID : uint
{
    Boss = 0x4791, // R4.2
    SandSphere = 0x4792, // R1.0
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 870, // Boss->player, no cast, single-target

    ChillingChirp = 42547, // Boss->self, 5.0s cast, range 30 circle
    AlmightyRacket = 42546, // Boss->self, 4.0s cast, range 30 width 30 rect
    AerialAmbushVisual = 42542, // Boss->location, 3.0s cast, single-target
    AerialAmbush = 42543, // Helper->self, 3.5s cast, range 30 width 15 rect
    FoundationalDebris = 43161, // Helper->location, 6.0s cast, range 10 circle
    SedimentaryDebris = 43160, // Helper->players, 5.0s cast, range 5 circle, spread
    Earthsong = 42544, // Boss->self, 5.0s cast, range 30 circle
    SphereShatter1 = 42545, // SandSphere->self, 2.0s cast, range 6 circle
    SphereShatter2 = 43135, // SandSphere->self, 2.0s cast, range 6 circle
    TrapJaws = 42548 // Boss->player, 5.0s cast, single-target
}

class AerialAmbush(BossModule module) : Components.SimpleAOEs(module, (uint)AID.AerialAmbush, new AOEShapeRect(30f, 7.5f));
class AlmightyRacket(BossModule module) : Components.SimpleAOEs(module, (uint)AID.AlmightyRacket, new AOEShapeRect(30f, 15f));
class FoundationalDebris(BossModule module) : Components.SimpleAOEs(module, (uint)AID.FoundationalDebris, 10f);
class SedimentaryDebris(BossModule module) : Components.SpreadFromCastTargets(module, (uint)AID.SedimentaryDebris, 5f);
class Earthsong(BossModule module) : Components.RaidwideCast(module, (uint)AID.Earthsong);
class ChillingChirp(BossModule module) : Components.RaidwideCast(module, (uint)AID.ChillingChirp);
class TrapJaws(BossModule module) : Components.SingleTargetDelayableCast(module, (uint)AID.TrapJaws);

class SphereShatter(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCircle circle = new(6f);
    private readonly List<AOEInstance> _aoes = new(13);

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = _aoes.Count;
        if (count == 0)
            return [];
        var aoes = CollectionsMarshal.AsSpan(_aoes);
        var deadline = aoes[0].Activation.AddSeconds(1d);

        var index = 0;
        while (index < count && aoes[index].Activation < deadline)
            ++index;

        return aoes[..index];
    }

    public override void OnActorCreated(Actor actor)
    {
        if (actor.OID == (uint)OID.SandSphere)
            _aoes.Add(new(circle, WPos.ClampToGrid(actor.Position), default, WorldState.FutureTime(7.9d)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.SphereShatter1 or (uint)AID.SphereShatter2)
            _aoes.RemoveAt(0);
    }
}

class D101GargantStates : StateMachineBuilder
{
    public D101GargantStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<AerialAmbush>()
            .ActivateOnEnter<AlmightyRacket>()
            .ActivateOnEnter<FoundationalDebris>()
            .ActivateOnEnter<SedimentaryDebris>()
            .ActivateOnEnter<Earthsong>()
            .ActivateOnEnter<ChillingChirp>()
            .ActivateOnEnter<TrapJaws>()
            .ActivateOnEnter<SphereShatter>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1027, NameID = 13753)]
public class D101Gargant(WorldState ws, Actor primary) : BossModule(ws, primary, new(-248f, 122f), new ArenaBoundsCircle(14.5f));
