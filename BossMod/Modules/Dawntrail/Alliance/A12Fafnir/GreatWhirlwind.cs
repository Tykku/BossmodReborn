namespace BossMod.Dawntrail.Alliance.A12Fafnir;

class GreatWhirlwindFirst1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.GreatWhirlwindFirst1), new AOEShapeCircle(10));
class GreatWhirlwindFirst2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.GreatWhirlwindFirst2), new AOEShapeCircle(3));

class Whirlwinds(BossModule module) : Components.GenericAOEs(module)
{
    private const int Length = 5;
    private static readonly AOEShapeCapsule capsuleSmall = new(3, Length), capsuleBig = new(9, Length);
    private readonly List<Actor> _smallWhirldwinds = [], _bigWhirldwinds = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var countSmall = _smallWhirldwinds.Count;
        var countBig = _bigWhirldwinds.Count;
        if (countSmall == 0 && countBig == 0)
            yield break;
        for (var i = 0; i < countSmall; ++i)
        {
            var w = _smallWhirldwinds[i];
            yield return new(capsuleSmall, w.Position, w.Rotation);
        }
        for (var i = 0; i < countBig; ++i)
        {
            var w = _bigWhirldwinds[i];
            yield return new(capsuleBig, w.Position, w.Rotation);
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.GreatWhirlwindFirst1)
            _bigWhirldwinds.Add(caster);
        else if ((AID)spell.Action.ID == AID.GreatWhirlwindFirst2)
            _smallWhirldwinds.Add(caster);
    }

    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if ((OID)actor.OID == OID.BitingWind && id == 0x1E3C)
            _smallWhirldwinds.Remove(actor);
        else if ((OID)actor.OID == OID.RavagingWind && id == 0x1E39)
            _bigWhirldwinds.Remove(actor);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var countSmall = _smallWhirldwinds.Count;
        var countBig = _bigWhirldwinds.Count;
        if (countSmall == 0 && countBig == 0)
            return;
        var forbidden = new List<Func<WPos, float>>(); // merging all forbidden zones into one to make pathfinding less demanding
        for (var i = 0; i < countBig; ++i)
        {
            var w = _bigWhirldwinds[i];
            forbidden.Add(ShapeDistance.Capsule(w.Position, w.Rotation, Length, 9));
        }
        for (var i = 0; i < countSmall; ++i)
        {
            var w = _smallWhirldwinds[i];
            forbidden.Add(ShapeDistance.Capsule(w.Position, w.Rotation, Length, 3));
        }
        hints.AddForbiddenZone(p => forbidden.Min(f => f(p)));
    }
}
