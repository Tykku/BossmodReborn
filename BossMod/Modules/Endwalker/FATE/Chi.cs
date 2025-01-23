namespace BossMod.Endwalker.FATE.Chi;

public enum OID : uint
{
    Boss = 0x34CB, // R16.0
    Helper1 = 0x34CC, // used for thermobaric missiles and freefall bombs
    Helper2 = 0x34CD, // used for tankbusters
    BouncingBomb1 = 0x364C, // first bomb hit
    BouncingBomb2 = 0x364D, // remaining hits
    Bunkerbuster1 = 0x361E, // used by BB with 10s casttime
    Bunkerbuster2 = 0x3505 // used by BB with 12s casttime
}

public enum AID : uint
{
    AutoAttack = 25952, // Boss->player, no cast, single-target

    AssaultCarapace1 = 25954, // Boss->self, 5.0s cast, range 120 width 32 rect
    AssaultCarapace2 = 25173, // Boss->self, 8.0s cast, range 120 width 32 rect
    CarapaceRearGuns2dot0A = 25958, // Boss->self, 8.0s cast, range 120 width 32 rect
    CarapaceForeArms2dot0A = 25957, // Boss->self, 8.0s cast, range 120 width 32 rect
    AssaultCarapace3 = 25953, // Boss->self, 5.0s cast, range 16-60 donut
    CarapaceForeArms2dot0B = 25955, // Boss->self, 8.0s cast, range 16-60 donut
    CarapaceRearGuns2dot0B = 25956, // Boss->self, 8.0s cast, range 16-60 donut
    ForeArms1 = 25959, // Boss->self, 6.0s cast, range 45 180-degree cone
    ForeArms2 = 26523, // Boss->self, 6.0s cast, range 45 180-degree cone
    ForeArms2dot0 = 25961, // Boss->self, no cast, range 45 180-degree cone
    RearGuns2dot0 = 25964, // Boss->self, no cast, range 45 180-degree cone
    RearGuns1 = 25962, // Boss->self, 6.0s cast, range 45 180-degree cone
    RearGuns2 = 26524, // Boss->self, 6.0s cast, range 45 180-degree cone
    RearGunsForeArms2dot0 = 25963, // Boss->self, 6.0s cast, range 45 180-degree cone
    ForeArmsRearGuns2dot0 = 25960, // Boss->self, 6.0s cast, range 45 180-degree cone
    Hellburner = 25971, // Boss->self, no cast, single-target, circle tankbuster
    Hellburner2 = 25972, // Helper1->players, 5.0s cast, range 5 circle
    FreeFallBombsVisual = 25967, // Boss->self, no cast, single-target
    FreeFallBombs = 25968, // Helper1->location, 3.0s cast, range 6 circle
    MissileShowerVisual = 25969, // Boss->self, 4.0s cast, single-target
    MissileShower = 25970, // Helper2->self, no cast, range 30 circle
    Teleport = 25155, // Boss->location, no cast, single-target, boss teleports mid

    BunkerBusterVisual = 25975, // Boss->self, 3.0s cast, single-target
    BunkerBuster1 = 25101, // Helper3->self, 10.0s cast, range 20 width 20 rect
    BunkerBuster2 = 25976, // Helper6->self, 12.0s cast, range 20 width 20 rect

    BouncingBombVisual = 27484, // Boss->self, 3.0s cast, single-target
    BouncingBombFirst = 27485, // Helper4->self, 5.0s cast, range 20 width 20 rect
    BouncingBombRest = 27486, // Helper5->self, 1.0s cast, range 20 width 20 rect

    ThermobaricExplosive = 25965, // Boss->self, 3.0s cast, single-target
    ThermobaricExplosive2 = 25966 // Helper1->location, 10.0s cast, range 55 circle, damage fall off AOE
}

class Bunkerbuster(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = new(9);
    public static readonly AOEShapeRect Square = new(10, 10, 10);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = _aoes.Count;
        if (count == 0)
            return [];
        var max = count > 6 ? 6 : count;
        List<AOEInstance> aoes = new(max);
        for (var i = 0; i < max; ++i)
        {
            var aoe = _aoes[i];
            if ((aoe.Activation - _aoes[0].Activation).TotalSeconds <= 1)
                aoes.Add(count > 3 ? aoe with { Color = Colors.Danger } : aoe);
            else
                aoes.Add(aoe);
        }
        return aoes;
    }

    public override void OnActorCreated(Actor actor)
    {
        var activation = (OID)actor.OID switch
        {
            OID.Bunkerbuster1 => 14.8f,
            OID.Bunkerbuster2 => 16.9f,
            _ => default
        };
        if (activation != default)
            _aoes.Add(new(Square, actor.Position, Angle.AnglesCardinals[1], WorldState.FutureTime(activation)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (_aoes.Count != 0 && (AID)spell.Action.ID is AID.BunkerBuster1 or AID.BunkerBuster2)
            _aoes.RemoveAt(0);
    }
}

class BouncingBomb(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = new(15);

    // either 9 or 15 explosions depending on pattern
    // pattern 1: 1, 3, 5 explosions
    // pattern 2: 2, 5, 8 explosions
    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = _aoes.Count;
        if (count == 0)
            return [];
        var max = count is <= 8 or > 9 and not 15 ? count : count == 9 ? 4 : 7;
        var firstact = _aoes[0].Activation;
        var lastact = _aoes[count - 1].Activation;
        List<AOEInstance> aoes = new(max);
        for (var i = 0; i < max; ++i)
        {
            var aoe = _aoes[i];
            if ((aoe.Activation - firstact).TotalSeconds < 1)
                aoes.Add((lastact - aoe.Activation).TotalSeconds > 1 ? aoe with { Color = Colors.Danger } : aoe);
            else
                aoes.Add(aoe with { Risky = false });
        }
        return aoes;
    }

    public override void OnActorCreated(Actor actor)
    {
        var activation = (OID)actor.OID switch
        {
            OID.BouncingBomb1 => 10,
            OID.BouncingBomb2 => 5.7f,
            _ => default
        };
        if (activation != default)
            _aoes.Add(new(Bunkerbuster.Square, actor.Position, Angle.AnglesCardinals[1], WorldState.FutureTime(activation)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (_aoes.Count != 0 && (AID)spell.Action.ID is AID.BouncingBombFirst or AID.BouncingBombRest)
            _aoes.RemoveAt(0);
    }
}

class Combos(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = new(2);
    private static readonly AOEShapeCone cone = new(45, 90.Degrees());
    private static readonly AOEShapeDonut donut = new(16, 60);
    private static readonly AOEShapeRect rect = new(120, 16);
    private static readonly HashSet<AID> castEnd = [AID.CarapaceForeArms2dot0A, AID.CarapaceForeArms2dot0B,
    AID.CarapaceRearGuns2dot0A, AID.CarapaceRearGuns2dot0B, AID.RearGunsForeArms2dot0, AID.ForeArmsRearGuns2dot0,
    AID.RearGuns2dot0, AID.ForeArms2dot0];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = _aoes.Count;
        if (count == 0)
            return [];
        List<AOEInstance> aoes = new(count);
        for (var i = 0; i < count; ++i)
        {
            var aoe = _aoes[i];
            if (i == 0)
                aoes.Add(count > 1 ? aoe with { Color = Colors.Danger } : aoe);
            else
                aoes.Add(aoe with { Risky = false });
        }
        return aoes;
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        void AddAOEs(AOEShape shape, bool backwards = false)
        {
            _aoes.Add(new(shape, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell)));
            _aoes.Add(new(cone, shape == rect ? caster.Position : spell.LocXZ, spell.Rotation + (backwards ? 180.Degrees() : default), Module.CastFinishAt(spell, 3.1f)));
        }

        switch ((AID)spell.Action.ID)
        {
            case AID.CarapaceForeArms2dot0A:
                AddAOEs(rect);
                break;
            case AID.CarapaceForeArms2dot0B:
                AddAOEs(donut);
                break;
            case AID.CarapaceRearGuns2dot0A:
                AddAOEs(rect, true);
                break;
            case AID.CarapaceRearGuns2dot0B:
                AddAOEs(donut, true);
                break;
            case AID.RearGunsForeArms2dot0:
            case AID.ForeArmsRearGuns2dot0:
                AddAOEs(cone, true);
                break;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (_aoes.Count != 0 && castEnd.Contains((AID)spell.Action.ID))
            _aoes.RemoveAt(0);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        base.AddAIHints(slot, actor, assignment, hints);
        if (_aoes.Count != 2)
            return;
        // make ai stay close to boss to ensure successfully dodging the combo
        var aoe = _aoes[0];
        hints.AddForbiddenZone(ShapeDistance.InvertedRect(Module.PrimaryActor.Position, aoe.Rotation, 2, 2, 40), aoe.Activation);
    }
}

class Hellburner(BossModule module) : Components.BaitAwayCast(module, ActionID.MakeSpell(AID.Hellburner2), new AOEShapeCircle(5), true)
{
    public override void AddGlobalHints(GlobalHints hints)
    {
        if (CurrentBaits.Count != 0)
            hints.Add("Tankbuster cleave");
    }
}

class MissileShower(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.MissileShowerVisual), "Raidwide x2");
class ThermobaricExplosive(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.ThermobaricExplosive2), 25);

abstract class AssaultCarapace(BossModule module, AID aid) : Components.SimpleAOEs(module, ActionID.MakeSpell(aid), new AOEShapeRect(120, 16));
class AssaultCarapace1(BossModule module) : AssaultCarapace(module, AID.AssaultCarapace1);
class AssaultCarapace2(BossModule module) : AssaultCarapace(module, AID.AssaultCarapace2);

class AssaultCarapace3(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.AssaultCarapace3), new AOEShapeDonut(16, 60));

abstract class Cleave(BossModule module, AID aid) : Components.SimpleAOEs(module, ActionID.MakeSpell(aid), new AOEShapeCone(45, 90.Degrees()));
class ForeArms1(BossModule module) : Cleave(module, AID.ForeArms1);
class ForeArms2(BossModule module) : Cleave(module, AID.ForeArms2);
class RearGuns1(BossModule module) : Cleave(module, AID.RearGuns1);
class RearGuns2(BossModule module) : Cleave(module, AID.RearGuns2);

class FreeFallBombs(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.FreeFallBombs), 6);

class ChiStates : StateMachineBuilder
{
    public ChiStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<AssaultCarapace1>()
            .ActivateOnEnter<AssaultCarapace2>()
            .ActivateOnEnter<AssaultCarapace3>()
            .ActivateOnEnter<Combos>()
            .ActivateOnEnter<ForeArms1>()
            .ActivateOnEnter<ForeArms2>()
            .ActivateOnEnter<RearGuns1>()
            .ActivateOnEnter<RearGuns2>()
            .ActivateOnEnter<Hellburner>()
            .ActivateOnEnter<FreeFallBombs>()
            .ActivateOnEnter<ThermobaricExplosive>()
            .ActivateOnEnter<Bunkerbuster>()
            .ActivateOnEnter<BouncingBomb>()
            .ActivateOnEnter<MissileShower>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.Fate, GroupID = 1855, NameID = 10400)]
public class Chi(WorldState ws, Actor primary) : BossModule(ws, primary, new(650, 0), new ArenaBoundsSquare(29.5f));
