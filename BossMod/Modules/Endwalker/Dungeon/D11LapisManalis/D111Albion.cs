namespace BossMod.Endwalker.Dungeon.D11LapisManalis.D111Albion;

public enum OID : uint
{
    Boss = 0x3CFE, //R=4.6
    WildBeasts = 0x3D03, //R=0.5
    WildBeasts1 = 0x3CFF, // R1.32
    WildBeasts2 = 0x3D00, // R1.7
    WildBeasts3 = 0x3D02, // R4.0
    WildBeasts4 = 0x3D01, // R2.85
    IcyCrystal = 0x3D04, // R2.0
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 872, // Boss->player, no cast, single-target
    Teleport = 32812, // Boss->location, no cast, single-target, boss teleports mid

    CallOfTheMountain = 31356, // Boss->self, 3.0s cast, single-target, boss calls wild beasts
    WildlifeCrossing = 31357, // WildBeasts->self, no cast, range 7 width 10 rect
    AlbionsEmbrace = 31365, // Boss->player, 5.0s cast, single-target

    RightSlam = 32813, // Boss->self, 5.0s cast, range 80 width 20 rect
    LeftSlam = 32814, // Boss->self, 5.0s cast, range 80 width 20 rect

    KnockOnIceVisual = 31358, // Boss->self, 4.0s cast, single-target
    KnockOnIce = 31359, // Helper->self, 6.0s cast, range 5 circle

    Icebreaker = 31361, // Boss->IcyCrystal, 5.0s cast, range 17 circle
    IcyThroesVisual = 31362, // Boss->self, no cast, single-target
    IcyThroes1 = 32783, // Helper->self, 5.0s cast, range 6 circle
    IcyThroes2 = 32697, // Helper->self, 5.0s cast, range 6 circle
    IcyThroesSpread = 31363, // Helper->player, 5.0s cast, range 6 circle
    RoarOfAlbion = 31364 // Boss->self, 7.0s cast, range 60 circle
}

class WildlifeCrossing(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect rect = new(20, 5, 20);

    private Queue<Stampede> stampedes = new();
    private static readonly uint[] animals = [(uint)OID.WildBeasts1, (uint)OID.WildBeasts2, (uint)OID.WildBeasts3, (uint)OID.WildBeasts4];

    private static readonly WPos[] stampedePositions =
    [
        new(4, -759), new(44, -759),
        new(4, -749), new(44, -749),
        new(4, -739), new(44, -739),
        new(4, -729), new(44, -729)
    ];

    private static Stampede NewStampede(WPos stampede) => new(true, stampede, stampede.X == 4 ? Angle.AnglesCardinals[3] : Angle.AnglesCardinals[0], new(40));

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        foreach (var stampede in stampedes)
            if (stampede.Active)
                yield return stampede.Beasts.Count > 0 ? CreateAOEInstance(stampede) : new(rect, stampede.Position, Angle.AnglesCardinals[3]);
    }

    private static AOEInstance CreateAOEInstance(Stampede stampede)
    {
        return new(new AOEShapeRect(CalculateStampedeLength(stampede.Beasts) + 30, 5), new(stampede.Beasts[^1].Position.X, stampede.Position.Z), stampede.Rotation);
    }

    private static float CalculateStampedeLength(List<Actor> beasts) => (beasts[0].Position - beasts[^1].Position).Length();

    public override void OnEventEnvControl(byte index, uint state)
    {
        if (state != 0x00020001)
            return;

        var stampedePosition = GetStampedePosition(index);
        if (stampedePosition == null)
            return;

        var stampede = GetOrCreateStampede(stampedePosition.Value);
        if (!stampedes.Contains(stampede))
            stampedes.Enqueue(stampede);
    }

    private Stampede GetOrCreateStampede(WPos stampedePosition)
    {
        var inactiveStampede = stampedes.FirstOrDefault(s => !s.Active);

        if (inactiveStampede != default)
            return ResetStampede(inactiveStampede, stampedePosition);

        if (stampedes.Count < 2)
            return NewStampede(stampedePosition);

        var oldest = stampedes.Dequeue();
        return NewStampede(stampedePosition);
    }

    private static Stampede ResetStampede(Stampede stampede, WPos position)
    {
        stampede.Active = true;
        stampede.Position = position;
        stampede.Rotation = stampede.Position.X == 4 ? Angle.AnglesCardinals[3] : Angle.AnglesCardinals[0];
        stampede.Count = default;
        stampede.Reset = default;
        stampede.Beasts.Clear();
        return stampede;
    }

    private static WPos? GetStampedePosition(byte index)
    {
        return index switch
        {
            0x21 => stampedePositions[0],
            0x25 => stampedePositions[1],
            0x22 => stampedePositions[2],
            0x26 => stampedePositions[3],
            0x23 => stampedePositions[4],
            0x27 => stampedePositions[5],
            0x24 => stampedePositions[6],
            0x28 => stampedePositions[7],
            _ => default
        };
    }

    public override void Update()
    {
        var stampedeList = stampedes.ToList();
        for (var i = 0; i < stampedeList.Count; ++i)
        {
            var stampede = stampedeList[i];
            UpdateStampede(ref stampede);
            ResetStampede(ref stampede);
            stampedeList[i] = stampede;
        }
        stampedes = new Queue<Stampede>(stampedeList);
    }

    private void UpdateStampede(ref Stampede stampede)
    {
        var beasts = Module.Enemies(animals);
        var updatedBeasts = stampede.Beasts == null ? new HashSet<Actor>(40) : [.. stampede.Beasts]; // use HashSet to easily prevent duplicates
        for (var i = 0; i < beasts.Count; ++i)
        {
            var b = beasts[i];
            if (b.Position.InRect(stampede.Position, stampede.Rotation, 0, 10, 5) && stampede.Active)
                updatedBeasts.Add(b);
        }
        stampede.Beasts = [.. updatedBeasts];
    }

    private void ResetStampede(ref Stampede stampede)
    {
        if (stampede.Reset != default && WorldState.CurrentTime > stampede.Reset)
            stampede = new();
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.WildlifeCrossing)
        {
            var stampedeList = stampedes.ToList();
            for (var i = 0; i < stampedeList.Count; ++i)
            {
                var stampede = stampedeList[i];
                UpdateStampedeCount(ref stampede, caster.Position.Z);
                stampedeList[i] = stampede;
            }
            stampedes = new Queue<Stampede>(stampedeList);
        }
    }

    private void UpdateStampedeCount(ref Stampede stampede, float casterZ)
    {
        if (Math.Abs(casterZ - stampede.Position.Z) < 1)
            ++stampede.Count;

        if (stampede.Count == 30)
            stampede.Reset = WorldState.FutureTime(0.5f);
    }
}

public record struct Stampede(bool Active, WPos Position, Angle Rotation, List<Actor> Beasts)
{
    public int Count;
    public DateTime Reset;
    public List<Actor> Beasts = Beasts;
}

class Icebreaker : Components.SimpleAOEs
{
    public Icebreaker(BossModule module) : base(module, ActionID.MakeSpell(AID.Icebreaker), 17) { TargetIsLocation = true; }
}

class IcyThroes(BossModule module, AID aid) : Components.SimpleAOEs(module, ActionID.MakeSpell(aid), 6);
class IcyThroes1(BossModule module) : IcyThroes(module, AID.IcyThroes1);
class IcyThroes2(BossModule module) : IcyThroes(module, AID.IcyThroes2);

class IcyThroesSpread(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.IcyThroesSpread), 6);
class KnockOnIce(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.KnockOnIce), 5);

abstract class Slam(BossModule module, AID aid) : Components.SimpleAOEs(module, ActionID.MakeSpell(aid), new AOEShapeRect(80, 10));
class RightSlam(BossModule module) : Slam(module, AID.RightSlam);
class LeftSlam(BossModule module) : Slam(module, AID.LeftSlam);

class AlbionsEmbrace(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.AlbionsEmbrace));

class RoarOfAlbion(BossModule module) : Components.CastLineOfSightAOE(module, ActionID.MakeSpell(AID.RoarOfAlbion), 60)
{
    public override IEnumerable<Actor> BlockerActors() => Module.Enemies(OID.IcyCrystal);
}

class D111AlbionStates : StateMachineBuilder
{
    public D111AlbionStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<WildlifeCrossing>()
            .ActivateOnEnter<LeftSlam>()
            .ActivateOnEnter<RightSlam>()
            .ActivateOnEnter<AlbionsEmbrace>()
            .ActivateOnEnter<Icebreaker>()
            .ActivateOnEnter<KnockOnIce>()
            .ActivateOnEnter<IcyThroes1>()
            .ActivateOnEnter<IcyThroes2>()
            .ActivateOnEnter<IcyThroesSpread>()
            .ActivateOnEnter<RoarOfAlbion>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 896, NameID = 11992)]
public class D111Albion(WorldState ws, Actor primary) : BossModule(ws, primary, new(24, -744), new ArenaBoundsSquare(19.5f));
