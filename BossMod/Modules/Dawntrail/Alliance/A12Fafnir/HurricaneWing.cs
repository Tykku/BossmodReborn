namespace BossMod.Dawntrail.Alliance.A12Fafnir;

class HurricaneWingRaidwide(BossModule module) : Components.CastCounterMulti(module, [ActionID.MakeSpell(AID.HurricaneWingRaidwideAOE1), ActionID.MakeSpell(AID.HurricaneWingRaidwideAOE2), ActionID.MakeSpell(AID.HurricaneWingRaidwideAOE3),
    ActionID.MakeSpell(AID.HurricaneWingRaidwideAOE4), ActionID.MakeSpell(AID.HurricaneWingRaidwideAOE5), ActionID.MakeSpell(AID.HurricaneWingRaidwideAOE6),
    ActionID.MakeSpell(AID.HurricaneWingRaidwideAOE7), ActionID.MakeSpell(AID.HurricaneWingRaidwideAOE8), ActionID.MakeSpell(AID.HurricaneWingRaidwideAOE9)]);

class HurricaneWingAOE(BossModule module) : Components.GenericAOEs(module)
{
    public override bool KeepOnPhaseChange => true;

    public readonly List<AOEInstance> AOEs = [];

    private static readonly AOEShape[] _shapes = [new AOEShapeCircle(9), new AOEShapeDonut(9, 16), new AOEShapeDonut(16, 23), new AOEShapeDonut(23, 30)];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => AOEs.Take(1);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        var shape = ShapeForAction(spell.Action);
        if (shape != null)
        {
            NumCasts = 0;
            AOEs.Add(new(shape, spell.LocXZ, default, Module.CastFinishAt(spell), ActorID: caster.InstanceID));
            AOEs.SortBy(aoe => aoe.Activation);
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        var shape = ShapeForAction(spell.Action);
        if (shape != null)
        {
            for (var i = 0; i < AOEs.Count; ++i)
            {
                var aoe = AOEs[i];
                if (aoe.ActorID == caster.InstanceID)
                {
                    AOEs.Remove(aoe);
                    break;
                }
            }
            ++NumCasts;
        }
    }

    private static AOEShape? ShapeForAction(ActionID aid) => (AID)aid.ID switch
    {
        AID.HurricaneWingLongExpanding1 or AID.HurricaneWingShortExpanding1 or AID.HurricaneWingLongShrinking4 or AID.HurricaneWingShortShrinking4 => _shapes[0],
        AID.HurricaneWingLongExpanding2 or AID.HurricaneWingShortExpanding2 or AID.HurricaneWingLongShrinking3 or AID.HurricaneWingShortShrinking3 => _shapes[1],
        AID.HurricaneWingLongExpanding3 or AID.HurricaneWingShortExpanding3 or AID.HurricaneWingLongShrinking2 or AID.HurricaneWingShortShrinking2 => _shapes[2],
        AID.HurricaneWingLongExpanding4 or AID.HurricaneWingShortExpanding4 or AID.HurricaneWingLongShrinking1 or AID.HurricaneWingShortShrinking1 => _shapes[3],
        _ => null
    };
}

class GreatWhirlwindLarge(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.GreatWhirlwindLarge), 10)
{
    public override bool KeepOnPhaseChange => true;
}

class GreatWhirlwindSmall(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.GreatWhirlwindSmall), 3)
{
    public override bool KeepOnPhaseChange => true;
}

class Whirlwinds(BossModule module) : Components.GenericAOEs(module)
{
    public override bool KeepOnPhaseChange => true;

    private const int Length = 5;
    private static readonly AOEShapeCapsule capsuleSmall = new(3, Length), capsuleBig = new(9, Length);
    private static readonly AOEShapeCircle circleSmall = new(3), circleBig = new(9);
    private readonly List<Actor> _smallWhirldwinds = [], _bigWhirldwinds = [];
    public bool Active => _smallWhirldwinds.Count != 0 || _bigWhirldwinds.Count != 0;
    private static readonly Angle a180 = 180.Degrees();
    private bool moving;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var countSmall = _smallWhirldwinds.Count;
        var countBig = _bigWhirldwinds.Count;
        var total = countSmall + countBig;
        if (total == 0)
            return [];
        List<AOEInstance> aoes = new(total);
        for (var i = 0; i < countSmall; ++i)
        {
            var w = _smallWhirldwinds[i];
            aoes.Add(new(moving ? capsuleSmall : circleSmall, w.Position, w.Rotation));
        }
        for (var i = 0; i < countBig; ++i)
        {
            var w = _bigWhirldwinds[i];
            aoes.Add(new(moving ? capsuleBig : circleBig, w.Position, w.Rotation));
        }
        return aoes;
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.GreatWhirlwindLarge)
        {
            _bigWhirldwinds.Add(caster);
            moving = false;
        }
        else if ((AID)spell.Action.ID == AID.GreatWhirlwindSmall)
            _smallWhirldwinds.Add(caster);
    }

    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if ((OID)actor.OID == OID.BitingWind && id == 0x1E3C)
            _smallWhirldwinds.Remove(actor);
        else if ((OID)actor.OID == OID.RavagingWind && id == 0x1E39)
            _bigWhirldwinds.Remove(actor);
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (!moving && (AID)spell.Action.ID is AID.GreatWhirlwindLargeAOE or AID.GreatWhirlwindSmallAOE)
            moving = true;
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var countSmall = _smallWhirldwinds.Count;
        var countBig = _bigWhirldwinds.Count;
        var total = countSmall + countBig;
        if (countSmall == 0 && countBig == 0)
            return;
        var forbidden = new List<Func<WPos, float>>(total); // merging all forbidden zones into one to make pathfinding less demanding

        const int length = Length + 5;
        for (var i = 0; i < countBig; ++i)
        {
            var w = _bigWhirldwinds[i];
            forbidden.Add(ShapeDistance.Capsule(w.Position, !moving ? w.Rotation + a180 : w.Rotation, length, 10));
        }
        for (var i = 0; i < countSmall; ++i)
        {
            var w = _smallWhirldwinds[i];
            forbidden.Add(ShapeDistance.Capsule(w.Position, !moving ? w.Rotation + a180 : w.Rotation, length, 5));
        }
        float minDistanceFunc(WPos pos)
        {
            var minDistance = float.MaxValue;
            for (var i = 0; i < forbidden.Count; ++i)
            {
                var distance = forbidden[i](pos);
                if (distance < minDistance)
                    minDistance = distance;
            }
            return minDistance;
        }
        hints.AddForbiddenZone(minDistanceFunc, WorldState.FutureTime(1.1f));
    }
}

class HorridRoarPuddle(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.HorridRoarPuddle), 4)
{
    public override bool KeepOnPhaseChange => true;
}

class HorridRoarSpread(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.HorridRoarSpread), 8)
{
    public override bool KeepOnPhaseChange => true;
}
