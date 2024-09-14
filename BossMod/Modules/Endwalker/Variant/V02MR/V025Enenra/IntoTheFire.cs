namespace BossMod.Endwalker.VariantCriterion.V02MR.V025Enenra;

class IntoTheFire(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect rect = new(50, 25);
    private readonly List<AOEInstance> _aoes = [];
    private static readonly float offset = 11 * MathF.Sqrt(2);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;

    public override void OnActorEAnim(Actor actor, uint state)
    {
        if (state == 0x00010002)
            HandleSmokeVisuals(actor, WorldState.FutureTime(16.6f));
    }

    private void HandleSmokeVisuals(Actor actor, DateTime activation)
    {
        var positionOffset = GetPositionOffsetForActor(actor);
        var rotation = GetRotationForActor(actor);

        if (positionOffset != null && rotation != null)
        {
            var roundedPosition = RoundPosition(actor.Position + positionOffset.Value);
            _aoes.Add(new(rect, roundedPosition, rotation.Value, activation));
        }
    }

    private static WDir? GetPositionOffsetForActor(Actor actor)
    {
        var actorDirection = actor.Rotation.ToDirection();

        return (OID)actor.OID switch
        {
            OID.SmokeVisual1 => offset * (actor.Rotation - 45.Degrees()).ToDirection(),
            OID.SmokeVisual2 => offset * (actor.Rotation + 45.Degrees()).ToDirection(),
            OID.SmokeVisual3 => 22 * actorDirection,
            _ => null
        };
    }

    private static Angle? GetRotationForActor(Actor actor)
    {
        return (OID)actor.OID switch
        {
            OID.SmokeVisual1 => actor.Rotation + 90.Degrees(),
            OID.SmokeVisual2 => actor.Rotation - 90.Degrees(),
            OID.SmokeVisual3 => actor.Rotation + 180.Degrees(),
            _ => null
        };
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (_aoes.Count > 0 && (AID)spell.Action.ID == AID.IntoTheFire)
            _aoes.RemoveAt(0);
    }

    private static WPos RoundPosition(WPos position) => new(MathF.Round(position.X), MathF.Round(position.Z));
}