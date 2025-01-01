﻿namespace BossMod.Endwalker.FATE.Chi;

public enum OID : uint
{
    Boss = 0x34CB, // R=16.0
    Helper1 = 0x34CC, //R=0.5
    Helper2 = 0x34CD, //R=0.5
    Helper3 = 0x361E, //R=0.5
    Helper4 = 0x364C, //R=0.5
    Helper5 = 0x364D, //R=0.5
    Helper6 = 0x3505, //R=0.5
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
    FreeFallBombs = 25967, // Boss->self, no cast, single-target
    FreeFallBombs2 = 25968, // Helper1->location, 3.0s cast, range 6 circle
    MissileShower = 25969, // Boss->self, 4.0s cast, single-target
    MissileShower2 = 25970, // Helper2->self, no cast, range 30 circle
    Teleport = 25155, // Boss->location, no cast, single-target, boss teleports mid
    BunkerBuster = 25975, // Boss->self, 3.0s cast, single-target
    BunkerBuster2 = 25101, // Helper3->self, 10.0s cast, range 20 width 20 rect
    BunkerBuster3 = 25976, // Helper6->self, 12.0s cast, range 20 width 20 rect
    BouncingBomb = 27484, // Boss->self, 3.0s cast, single-target
    BouncingBomb2 = 27485, // Helper4->self, 5.0s cast, range 20 width 20 rect
    BouncingBomb3 = 27486, // Helper5->self, 1.0s cast, range 20 width 20 rect
    ThermobaricExplosive = 25965, // Boss->self, 3.0s cast, single-target
    ThermobaricExplosive2 = 25966, // Helper1->location, 10.0s cast, range 55 circle, damage fall off AOE
}

class Bunkerbuster(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<Actor> _casters = [];
    private DateTime _activation;
    private int NumCastsStarted;

    private static readonly AOEShapeRect rect = new(10, 10, 10);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_casters.Count >= 3)
            for (var i = 0; i < 3; ++i)
                yield return new(rect, _casters[i].Position, _casters[i].Rotation, _activation, Colors.Danger);
        if (_casters.Count >= 6)
            for (var i = 3; i < 6; ++i)
                yield return new(rect, _casters[i].Position, _casters[i].Rotation, _activation.AddSeconds(1.9f));
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.BunkerBuster2 && NumCastsStarted == 0)
        {
            _activation = Module.CastFinishAt(spell);
            ++NumCastsStarted;
        }
        else if ((AID)spell.Action.ID is AID.BunkerBuster3 && NumCastsStarted == 0)
        {
            _activation = Module.CastFinishAt(spell);
            ++NumCastsStarted;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.BunkerBuster2 or AID.BunkerBuster3)
        {
            ++NumCasts;
            if (_casters.Count > 0)
                _casters.Remove(caster);
            if (NumCasts is 3 or 6 or 9 or 12 or 15)
                _activation = _activation.AddSeconds(1.9f);
            if (_casters.Count == 0 && NumCasts != 0)
            {
                NumCasts = 0;
                NumCastsStarted = 0;
            }
        }
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID is OID.Helper3 or OID.Helper6)
        {
            _casters.Add(actor);
            if (_casters.Count == 1)
                _activation = WorldState.FutureTime(20); // placeholder value that gets overwritten when cast actually starts
        }
    }
}

class BouncingBomb(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<Actor> _casters = [];
    private DateTime _activation;
    private int bombcount;

    private static readonly AOEShapeRect rect = new(10, 10, 10);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (bombcount == 1)
        {
            if (_casters.Count >= 1 && NumCasts == 0)
                yield return new(rect, _casters[0].Position, _casters[0].Rotation, _activation, Colors.Danger);
            if (_casters.Count >= 4 && NumCasts == 0)
                for (var i = 1; i < 4; ++i)
                    yield return new(rect, _casters[i].Position, _casters[i].Rotation, _activation.AddSeconds(2.8f));
            if (_casters.Count >= 3 && NumCasts == 1)
                for (var i = 0; i < 3; ++i)
                    yield return new(rect, _casters[i].Position, _casters[i].Rotation, _activation, Colors.Danger);
            if (_casters.Count >= 8 && NumCasts == 1)
                for (var i = 3; i < 8; ++i)
                    yield return new(rect, _casters[i].Position, _casters[i].Rotation, _activation.AddSeconds(2.8f));
            if (_casters.Count >= 5 && NumCasts == 4)
                for (var i = 0; i < 5; ++i)
                    yield return new(rect, _casters[i].Position, _casters[i].Rotation, _activation, Colors.Danger);
        }
        if (bombcount == 2)
        {
            if (_casters.Count >= 2 && NumCasts == 0)
                for (var i = 0; i < 2; ++i)
                    yield return new(rect, _casters[i].Position, _casters[i].Rotation, _activation, Colors.Danger);
            if (_casters.Count >= 7 && NumCasts == 0)
                for (var i = 2; i < 7; ++i)
                    yield return new(rect, _casters[i].Position, _casters[i].Rotation, _activation.AddSeconds(2.8f), Risky: false);
            if (_casters.Count >= 5 && NumCasts == 2)
                for (var i = 0; i < 5; ++i)
                    yield return new(rect, _casters[i].Position, _casters[i].Rotation, _activation, Colors.Danger);
            if (_casters.Count >= 13 && NumCasts == 2)
                for (var i = 5; i < 13; ++i)
                    yield return new(rect, _casters[i].Position, _casters[i].Rotation, _activation.AddSeconds(2.8f), Risky: false);
            if (_casters.Count >= 8 && NumCasts == 7)
                for (var i = 0; i < 8; ++i)
                    yield return new(rect, _casters[i].Position, _casters[i].Rotation, _activation, Colors.Danger);
        }
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.Helper4)
        {
            _activation = WorldState.FutureTime(10);  // placeholder value that gets overwritten when cast actually starts
            ++bombcount;
        }
        if ((OID)actor.OID is OID.Helper4 or OID.Helper5)
            _casters.Add(actor);
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.BouncingBomb2)
            _activation = Module.CastFinishAt(spell);
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.BouncingBomb2 or AID.BouncingBomb3)
        {
            ++NumCasts;
            if (_casters.Count > 0)
                _casters.Remove(caster);
            if (bombcount == 1 && NumCasts is 1 or 4 || bombcount == 2 && NumCasts is 2 or 7)
                _activation = _activation.AddSeconds(2.8f);
            if (_casters.Count == 0 && bombcount != 0)
            {
                bombcount = 0;
                NumCasts = 0;
            }
        }
    }
}

class Combos(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCone cone = new(45, 90.Degrees());
    private static readonly AOEShapeDonut donut = new(16, 60);
    private static readonly AOEShapeRect rect = new(60, 16, 60);
    private static readonly Angle Angle180 = 180.Degrees();
    private (AOEShape shape1, AOEShape shape2, DateTime activation1, DateTime activation2, bool offset, Angle rotation) combo;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (combo != default)
        {
            if (NumCasts == 0)
            {
                yield return new(combo.shape1, Module.PrimaryActor.Position, combo.rotation, combo.activation1, Colors.Danger);
                yield return !combo.offset
                    ? new(combo.shape2, Module.PrimaryActor.Position, combo.rotation, combo.activation2, Risky: combo.shape1 != combo.shape2)
                    : new(combo.shape2, Module.PrimaryActor.Position, combo.rotation + Angle180, combo.activation2, Risky: combo.shape1 != combo.shape2);
            }
            if (NumCasts == 1)
            {
                yield return !combo.offset
                    ? new(combo.shape2, Module.PrimaryActor.Position, combo.rotation, combo.activation2, Colors.Danger)
                    : new(combo.shape2, Module.PrimaryActor.Position, combo.rotation + Angle180, combo.activation2, Colors.Danger);
            }
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        var activation = Module.CastFinishAt(spell, 3.1f);
        switch ((AID)spell.Action.ID)
        {
            case AID.CarapaceForeArms2dot0A:
                combo = (rect, cone, Module.CastFinishAt(spell), activation, false, spell.Rotation);
                break;
            case AID.CarapaceForeArms2dot0B:
                combo = (donut, cone, Module.CastFinishAt(spell), activation, false, spell.Rotation);
                break;
            case AID.CarapaceRearGuns2dot0A:
                combo = (rect, cone, Module.CastFinishAt(spell), activation, true, spell.Rotation);
                break;
            case AID.CarapaceRearGuns2dot0B:
                combo = (donut, cone, Module.CastFinishAt(spell), activation, true, spell.Rotation);
                break;
            case AID.RearGunsForeArms2dot0:
            case AID.ForeArmsRearGuns2dot0:
                combo = (cone, cone, Module.CastFinishAt(spell), activation, true, spell.Rotation);
                break;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.CarapaceForeArms2dot0A or AID.CarapaceForeArms2dot0B or AID.CarapaceRearGuns2dot0A or AID.CarapaceRearGuns2dot0B or AID.RearGunsForeArms2dot0 or AID.ForeArmsRearGuns2dot0)
            ++NumCasts;
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.RearGuns2dot0 or AID.ForeArms2dot0)
        {
            NumCasts = 0;
            combo = default;
        }
    }
}

class Hellburner(BossModule module) : Components.BaitAwayCast(module, ActionID.MakeSpell(AID.Hellburner2), new AOEShapeCircle(5), true)
{
    public override void AddGlobalHints(GlobalHints hints)
    {
        if (CurrentBaits.Count > 0)
            hints.Add("Tankbuster cleave");
    }
}

class MissileShower(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.MissileShower), "Raidwide x2");
class ThermobaricExplosive(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.ThermobaricExplosive2), 25);

abstract class AssaultCarapaceRect(BossModule module, AID aid) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(aid), new AOEShapeRect(60, 16, 60));
class AssaultCarapace1(BossModule module) : AssaultCarapaceRect(module, AID.AssaultCarapace1);
class AssaultCarapace2(BossModule module) : AssaultCarapaceRect(module, AID.AssaultCarapace2);

class AssaultCarapace3(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.AssaultCarapace3), new AOEShapeDonut(16, 60));

abstract class Cleave(BossModule module, AID aid) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(aid), new AOEShapeCone(45, 90.Degrees()));
class ForeArms1(BossModule module) : Cleave(module, AID.ForeArms1);
class ForeArms2(BossModule module) : Cleave(module, AID.ForeArms2);
class RearGuns1(BossModule module) : Cleave(module, AID.RearGuns1);
class RearGuns2(BossModule module) : Cleave(module, AID.RearGuns2);

class FreeFallBombs(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.FreeFallBombs2), 6);

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
