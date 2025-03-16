namespace BossMod.Stormblood.Extreme.Ex7Suzaku;

class MesmerizingMelody(BossModule module) : Components.SimpleKnockbacks(module, ActionID.MakeSpell(AID.MesmerizingMelody), 11f, kind: Kind.TowardsOrigin)
{
    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var source = Casters.Count != 0 ? Casters[0] : null;
        if (source != null)
            hints.AddForbiddenZone(ShapeDistance.Circle(source.Position, 14.5f), Module.CastFinishAt(source.CastInfo));
    }
}

class RuthlessRefrain(BossModule module) : Components.SimpleKnockbacks(module, ActionID.MakeSpell(AID.RuthlessRefrain), 11f, kind: Kind.AwayFromOrigin)
{
    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var source = Casters.Count != 0 ? Casters[0] : null;
        if (source != null)
            hints.AddForbiddenZone(ShapeDistance.InvertedCircle(source.Position, 11f), Module.CastFinishAt(source.CastInfo));
    }
}

class PayThePiper : Components.GenericForcedMarch
{
    public PayThePiper(BossModule module) : base(module)
    {
        OverrideDirection = true;
    }

    public override void OnTethered(Actor source, ActorTetherInfo tether)
    {
        var target = WorldState.Actors.Find(tether.Target)!;
        if (target == Module.PrimaryActor || (TetherID)tether.ID != TetherID.PayThePiper)
            return;
        AddForcedMovement(target, new Angle(MathF.Round(source.Rotation.Deg / 90f) * 90f) * Angle.DegToRad + 180f.Degrees(), 4f, WorldState.FutureTime(10d));
    }

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if (status.ID == (uint)SID.PayingThePiper)
        {
            State.GetOrAdd(actor.InstanceID).PendingMoves.Clear();
            ActivateForcedMovement(actor, status.ExpireAt);
        }
    }

    public override void OnStatusLose(Actor actor, ActorStatus status)
    {
        if (status.ID == (uint)SID.PayingThePiper)
            DeactivateForcedMovement(actor);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var state = State.GetValueOrDefault(actor.InstanceID);
        if (state == null || state.PendingMoves.Count == 0)
            return;

        // adding 1 unit of safety margin to everything to make it look less suspect
        var move0 = state.PendingMoves[0];
        var dir = move0.dir.ToDirection();
        var forbidden = new Func<WPos, float>[2];
        forbidden[0] = ShapeDistance.InvertedCircle(Ex7Suzaku.ArenaCenter - 25f * dir, 19f);
        forbidden[1] = ShapeDistance.Rect(Ex7Suzaku.ArenaCenter, -dir, 20f, default, 4.5f);
        hints.AddForbiddenZone(ShapeDistance.Union(forbidden), move0.activation);
    }
}
