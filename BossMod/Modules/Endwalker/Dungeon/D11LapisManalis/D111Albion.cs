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
    KnockOnIce = 31358, // Boss->self, 4.0s cast, single-target
    KnockOnIce2 = 31359, // Helper->self, 6.0s cast, range 5 circle
    Icebreaker = 31361, // Boss->3D04, 5.0s cast, range 17 circle
    IcyThroes = 31362, // Boss->self, no cast, single-target
    IcyThroes2 = 32783, // Helper->self, 5.0s cast, range 6 circle
    IcyThroes3 = 31363, // Helper->player, 5.0s cast, range 6 circle
    IcyThroes4 = 32697, // Helper->self, 5.0s cast, range 6 circle
    RoarOfAlbion = 31364 // Boss->self, 7.0s cast, range 60 circle
}

public enum IconID : uint
{
    Tankbuster = 218, // player
    Target = 210, // IceCrystal
    Spreadmarker = 139, // player
}

class WildlifeCrossing(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect rect = new(20, 5, 20);
    private static readonly Angle Rot90 = 90.Degrees();
    private static readonly Angle RotM90 = -90.Degrees();
    private Queue<Stampede> stampedes = new();

    private static readonly (WPos, Angle)[] stampedePositions =
    [
        (new(4, -759), Rot90), (new(44, -759), RotM90),
        (new(4, -749), Rot90), (new(44, -749), RotM90),
        (new(4, -739), Rot90), (new(44, -739), RotM90),
        (new(4, -729), Rot90), (new(44, -729), RotM90)
    ];

    private static Stampede NewStampede((WPos, Angle) stampede) => new(true, stampede.Item1, stampede.Item2, []);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        foreach (var stampede in stampedes)
            if (stampede.Active)
                yield return stampede.Beasts.Count > 0 ? CreateAOEInstance(stampede) : new(rect, stampede.Position, Rot90);
    }

    private static AOEInstance CreateAOEInstance(Stampede stampede)
    {
        var length = CalculateStampedeLength(stampede.Beasts) + 30;
        var position = new WPos(stampede.Beasts[^1].Position.X, stampede.Position.Z);
        return new(new AOEShapeRect(length, 5), position, stampede.Rotation);
    }

    private static float CalculateStampedeLength(IReadOnlyList<Actor> beasts) => (beasts[0].Position - beasts[^1].Position).Length();

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

    private Stampede GetOrCreateStampede((WPos, Angle) stampedePosition)
    {
        var inactiveStampede = stampedes.FirstOrDefault(s => !s.Active);

        if (inactiveStampede != default)
            return ResetStampede(inactiveStampede, stampedePosition);

        if (stampedes.Count < 2)
            return NewStampede(stampedePosition);

        var oldest = stampedes.Dequeue();
        return NewStampede(stampedePosition);
    }

    private static Stampede ResetStampede(Stampede stampede, (WPos, Angle) position)
    {
        stampede.Active = true;
        stampede.Position = position.Item1;
        stampede.Rotation = position.Item2;
        stampede.Count = 0;
        stampede.Reset = default;
        stampede.Beasts = [];
        return stampede;
    }

    private (WPos, Angle)? GetStampedePosition(byte index)
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
        for (var i = 0; i < stampedeList.Count; i++)
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
        foreach (var oid in new[] { OID.WildBeasts4, OID.WildBeasts3, OID.WildBeasts2, OID.WildBeasts1 })
        {
            var beasts = Module.Enemies(oid);
            var updatedBeasts = stampede.Beasts.ToList();
            foreach (var b in beasts)
                if (b.Position.InRect(stampede.Position, stampede.Rotation, 0, 10, 5) && !updatedBeasts.Contains(b) && stampede.Active)
                    updatedBeasts.Add(b);
            stampede = new Stampede(stampede.Active, stampede.Position, stampede.Rotation, updatedBeasts);
        }
    }

    private void ResetStampede(ref Stampede stampede)
    {
        if (stampede.Reset != default && WorldState.CurrentTime > stampede.Reset)
            stampede = new();
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID != AID.WildlifeCrossing)
            return;

        var stampedeList = stampedes.ToList();
        for (var i = 0; i < stampedeList.Count; i++)
        {
            var stampede = stampedeList[i];
            UpdateStampedeCount(ref stampede, caster.Position.Z);
            stampedeList[i] = stampede;
        }
        stampedes = new Queue<Stampede>(stampedeList);
    }

    private void UpdateStampedeCount(ref Stampede stampede, float casterZ)
    {
        if (MathF.Abs(casterZ - stampede.Position.Z) < 1)
            ++stampede.Count;

        if (stampede.Count == 30)
            stampede.Reset = WorldState.FutureTime(0.5f);
    }
}

public record struct Stampede(bool Active, WPos Position, Angle Rotation, IReadOnlyList<Actor> Beasts)
{
    public int Count;
    public DateTime Reset;
}

class IcyThroes(BossModule module) : Components.GenericBaitAway(module)
{
    private readonly List<Actor> _targets = [];

    public override void OnEventIcon(Actor actor, uint iconID)
    {
        if (iconID == (uint)IconID.Spreadmarker)
        {
            CurrentBaits.Add(new(Module.PrimaryActor, actor, new AOEShapeCircle(6)));
            _targets.Add(actor);
            CenterAtTarget = true;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.IcyThroes3)
        {
            CurrentBaits.Clear();
            _targets.Clear();
        }
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (_targets.Contains(actor))
            hints.Add("Bait away!");
    }
}

class Icebreaker(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<Actor> _casters = [];
    private static readonly AOEShapeCircle circle = new(17);
    private DateTime _activation;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_casters.Count > 0)
            foreach (var c in _casters)
                yield return new(circle, c.Position, default, _activation);
    }

    public override void OnEventIcon(Actor actor, uint iconID)
    {
        if (iconID == (uint)IconID.Target)
        {
            _casters.Add(actor);
            _activation = WorldState.FutureTime(6);
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.Icebreaker)
            _activation = Module.CastFinishAt(spell);
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.Icebreaker)
            _casters.Clear();
    }
}

class IcyThroes2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.IcyThroes2), new AOEShapeCircle(6));
class IcyThroes4(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.IcyThroes4), new AOEShapeCircle(6));
class KnockOnIce(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.KnockOnIce2), new AOEShapeCircle(5));
class RightSlam(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.RightSlam), new AOEShapeRect(20, 80, 0, -90.Degrees())); //full width = half width in this case + angle is detected incorrectly, length and width are also switched
class LeftSlam(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.LeftSlam), new AOEShapeRect(20, 80, 0, 90.Degrees())); //full width = half width in this case + angle is detected incorrectly, length and width are also switched
class AlbionsEmbrace(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.AlbionsEmbrace));

class RoarOfAlbion(BossModule module) : Components.CastLineOfSightAOE(module, ActionID.MakeSpell(AID.RoarOfAlbion), 60, false)
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
            .ActivateOnEnter<IcyThroes>()
            .ActivateOnEnter<IcyThroes2>()
            .ActivateOnEnter<IcyThroes4>()
            .ActivateOnEnter<RoarOfAlbion>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 896, NameID = 11992)]
public class D111Albion(WorldState ws, Actor primary) : BossModule(ws, primary, new(24, -744), new ArenaBoundsSquare(19.5f));
