namespace BossMod.Endwalker.DeepDungeon.EurekaOrthos.DD99Excalibur;

public enum OID : uint
{
    Boss = 0x3CFC, // R5.1
    Caliburnus = 0x3CFD, // R1.0
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 31355, // Boss->player, no cast, single-target
    Teleport = 31326, // Boss->location, no cast, single-target

    ParadoxumVisual = 31353, // Boss->self, 5.0s cast, single-target
    Paradoxum = 31354, // Helper->self, no cast, range 100 circle

    Caliburni = 31333, // Boss->self, 5.0+0,5s cast, single-target
    Steelstrike1 = 31334, // Caliburnus->location, 2.0s cast, width 4 rect charge
    Steelstrike2 = 31335, // Caliburnus->location, 2.0s cast, width 4 rect charge
    ThermalDivideVisual1 = 31349, // Boss->self, 5.0+2,0s cast, single-target, ice left, fire right
    ThermalDivideVisual2 = 31350, // Boss->self, 5.0+2,0s cast, single-target, fire left, ice right
    ThermalDivide1 = 32701, // Helper->self, 6.7s cast, range 40 width 8 rect
    ThermalDivide2 = 31351, // Helper->self, no cast, range 40 width 40 rect

    CallDimensionBlade = 31338, // Boss->self, 4.0s cast, single-target
    SolidSoulsCaliber = 31327, // Boss->self, 6.0s cast, range 10 circle
    EmptySoulsCaliber = 31328, // Boss->self, 6.0s cast, range 5-40 donut
    VacuumSlash = 31342, // Helper->self, 7.0s cast, range 80 45-degree cone
    Flameforge = 31329, // Boss->self, 3.0s cast, single-target
    Frostforge = 31330, // Boss->self, 3.0s cast, single-target
    SteelFlame = 31336, // Caliburnus->location, 2.0s cast, width 4 rect charge
    SteelFrost = 31337, // Caliburnus->location, 2.0s cast, width 4 rect charge

    AbyssalSlash1 = 31339, // Helper->self, 7.0s cast, range 2-7 donut
    AbyssalSlash2 = 31340, // Helper->self, 7.0s cast, range 7-12 donut
    AbyssalSlash3 = 31341, // Helper->self, 7.0s cast, range 17-22 donut

    FlamesRevelation = 31331, // Helper->self, no cast, range 60 circle
    FrostRevelation = 31332, // Helper->self, no cast, range 60 circle

    ExflammeusVisual = 31343, // Boss->self, 4.0s cast, single-target
    Exflammeus = 31344, // Helper->self, 5.0s cast, range 8 circle

    Exglacialis = 31343, // Boss->self, 4.0s cast, single-target
    IceShoot = 31346, // Helper->self, 5.0s cast, range 6 circle
    IceBloomCircle = 31347, // Helper->self, 4.0s cast, range 6 circle
    IceBloomCross = 31348 // Helper->self, no cast, range 40 width 5 cross
}

public enum SID : uint
{
    SoulOfIce = 3411, // Helper->player, extra=0x0
    SoulOfFire = 3410, // Helper->player, extra=0x0
    CaliburnusElement = 2552 // none->Caliburnus, extra=0x219/0x21A (0x21A = frost, 0x219 = fire)
}

class Steelstrike(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect rect = new(20, 2);
    private static readonly AOEShapeRect rect2 = new(20, 2, 20);
    private readonly HashSet<AOEInstance> _aoes = [];
    private readonly List<Angle> angles = [];
    private readonly List<AOEInstance> swordsFire = [];
    private readonly List<AOEInstance> swordsIce = [];
    private static readonly Angle[] offsets = [default, 120.Degrees(), 240.Degrees()];
    private static readonly Angle a20 = 20.Degrees();
    private static readonly Angle a10 = 10.Degrees();
    private static readonly Angle a45 = 45.Degrees();
    private bool? nextRaidwide; // null = none, false = fire, true = ice

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (nextRaidwide == null)
            return _aoes;

        var fire = actor.FindStatus(SID.SoulOfFire) != null;
        var ice = actor.FindStatus(SID.SoulOfIce) != null;

        return ice && (bool)nextRaidwide ? GetSafeAOEs(swordsFire)
            : fire && !(bool)nextRaidwide ? GetSafeAOEs(swordsIce)
            : _aoes;
    }

    private IEnumerable<AOEInstance> GetSafeAOEs(List<AOEInstance> swordAOEs)
    {
        var result = _aoes.Except([swordAOEs.First(), swordAOEs.Last()]).ToList();
        var safeAOEs = new[]
        {
            swordAOEs.First() with { Color = Colors.SafeFromAOE },
            swordAOEs.Last() with { Color = Colors.SafeFromAOE }
        };
        return result.Concat(safeAOEs);
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.Caliburni:
                AddAOEs(spell.Rotation);
                ++NumCasts;
                break;
            case AID.ThermalDivide1:
            case AID.ParadoxumVisual:
                foreach (var a in angles)
                    _aoes.Add(new(rect2, Arena.Center, a, Module.CastFinishAt(spell, 3.4f)));
                break;
            case AID.Flameforge:
                nextRaidwide = false;
                break;
            case AID.Frostforge:
                nextRaidwide = true;
                break;
        }
    }

    public override void AddGlobalHints(GlobalHints hints)
    {
        if (nextRaidwide != null)
            hints.Add((bool)!nextRaidwide ? "Next raidwide: Fire" : "Next raidwide: Ice");
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        base.AddHints(slot, actor, hints);
        if (nextRaidwide != null)
        {
            var fire = actor.FindStatus(SID.SoulOfFire) != null;
            var ice = actor.FindStatus(SID.SoulOfIce) != null;
            if (ice && (bool)nextRaidwide)
                hints.Add("Get hit by a fire blade!");
            else if (fire && !(bool)nextRaidwide)
                hints.Add("Get hit by a single ice blade!");
        }
    }

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if ((SID)status.ID == SID.CaliburnusElement)
        {
            AddSwordAOE(actor, status.Extra == 0x219 ? swordsFire : swordsIce);
            if (swordsFire.Count == 5 && swordsIce.Count == 5)
            {
                swordsFire.Sort((a, b) => a.Rotation.Rad.CompareTo(b.Rotation.Rad));
                swordsIce.Sort((a, b) => a.Rotation.Rad.CompareTo(b.Rotation.Rad));
            }
        }
    }

    private void AddSwordAOE(Actor actor, List<AOEInstance> swordAOEs)
    {
        swordAOEs.Add(_aoes.FirstOrDefault(x => x.Origin.AlmostEqual(actor.Position, 1)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.Steelstrike1:
                _aoes.Clear();
                break;
            case AID.Steelstrike2:
            case AID.SteelFlame:
            case AID.SteelFrost:
                _aoes.Clear();
                angles.Clear();
                swordsFire.Clear();
                swordsIce.Clear();
                break;
        }
    }

    private void AddAOEs(Angle rotation)
    {
        var numAOEs = NumCasts == 0 ? 10 : 5;
        var startAngle = NumCasts == 0 ? -a45 : -a20;

        for (var i = 0; i < numAOEs; ++i)
        {
            var rot = rotation + startAngle + i * a10;
            if (NumCasts == 0)
                AddSingleAOE(rot);
            else
                foreach (var o in offsets)
                    AddSingleAOE(rot + o);
        }
    }

    private void AddSingleAOE(Angle rotation)
    {
        _aoes.Add(new(rect, Arena.Center, rotation));
        angles.Add(rotation);
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.FlamesRevelation or AID.FrostRevelation)
            nextRaidwide = null;
    }
}

class ThermalDivideSides(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCone cone = new(40, 90.Degrees());
    private static readonly Angle a90 = 90.Degrees();
    private bool? pattern; // null = none, false = ice left, fire right, true = fire left, ice right
    private Angle rotation;
    private WDir offset;
    private DateTime activation;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (pattern != null)
        {
            var fire = actor.FindStatus(SID.SoulOfFire) != null;
            var ice = actor.FindStatus(SID.SoulOfIce) != null;

            return (bool)pattern ? HandleAOEs(fire) : HandleAOEs(ice);
        }
        else
            return [];
    }

    private IEnumerable<AOEInstance> HandleAOEs(bool condition)
    {
        var pos1 = Arena.Center + offset;
        var pos2 = Arena.Center - offset;

        if (condition)
        {
            yield return new(cone, pos1, rotation + a90, activation);
            yield return new(cone with { InvertForbiddenZone = true }, pos2, rotation - a90, activation, Colors.SafeFromAOE);
        }
        else
        {
            yield return new(cone, pos2, rotation - a90, activation);
            yield return new(cone with { InvertForbiddenZone = true }, pos1, rotation + a90, activation, Colors.SafeFromAOE);
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.ThermalDivideVisual1 or AID.ThermalDivideVisual2)
        {
            pattern = (AID)spell.Action.ID == AID.ThermalDivideVisual2;
            rotation = spell.Rotation;
            offset = 4 * (spell.Rotation + a90).ToDirection();
            activation = Module.CastFinishAt(spell, 1.9f);
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.ThermalDivide2)
            pattern = null;
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (pattern == null)
            return;
        if (ActiveAOEs(slot, actor).Any(c => c.Color == Colors.SafeFromAOE && !c.Check(actor.Position)))
            hints.Add("Go to safe side!");
    }
}

class IceBloomCross(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = new(6);
    private static readonly AOEShapeCross cross = new(40, 2.5f);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = _aoes.Count;
        if (count == 0)
            return [];
        var max = count > 6 ? 6 : count;
        List<AOEInstance> aoes = new(max);
        for (var i = 0; i < max; ++i)
            aoes.Add(_aoes[i]);
        return aoes;
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.IceShoot)
        {
            var pos = spell.LocXZ;
            var activation = Module.CastFinishAt(spell, 6);
            _aoes.Add(new(cross, pos, spell.Rotation, activation));
            _aoes.Add(new(cross, pos, spell.Rotation + 45.Degrees(), activation));
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (_aoes.Count != 0 && (AID)spell.Action.ID == AID.IceBloomCross)
            _aoes.RemoveAt(0);
    }
}

class AbyssalSlash1(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.AbyssalSlash1), new AOEShapeDonutSector(2, 7, 90.Degrees()));
class AbyssalSlash2(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.AbyssalSlash2), new AOEShapeDonutSector(7, 12, 90.Degrees()));
class AbyssalSlash3(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.AbyssalSlash3), new AOEShapeDonutSector(17, 22, 90.Degrees()));
class VacuumSlash(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.VacuumSlash), new AOEShapeCone(80, 22.5f.Degrees()));
class ThermalDivide(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.ThermalDivide1), new AOEShapeRect(40, 4));
class Exflammeus(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Exflammeus), 8);
class IceShoot(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.IceShoot), 6);
class IceBloomCircle(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.IceBloomCircle), 6);
class EmptySoulsCaliber(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.EmptySoulsCaliber), new AOEShapeDonut(5, 40));
class SolidSoulsCaliber(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.SolidSoulsCaliber), 10);

class DD99ExcaliburStates : StateMachineBuilder
{
    public DD99ExcaliburStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<ThermalDivideSides>()
            .ActivateOnEnter<AbyssalSlash1>()
            .ActivateOnEnter<AbyssalSlash2>()
            .ActivateOnEnter<AbyssalSlash3>()
            .ActivateOnEnter<VacuumSlash>()
            .ActivateOnEnter<ThermalDivide>()
            .ActivateOnEnter<IceShoot>()
            .ActivateOnEnter<IceBloomCircle>()
            .ActivateOnEnter<Exflammeus>()
            .ActivateOnEnter<EmptySoulsCaliber>()
            .ActivateOnEnter<SolidSoulsCaliber>()
            .ActivateOnEnter<Steelstrike>()
            .ActivateOnEnter<IceBloomCross>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 906, NameID = 12100)]
public class DD99Excalibur(WorldState ws, Actor primary) : BossModule(ws, primary, new(-600, -300), new ArenaBoundsCircle(19.5f));