namespace BossMod.Components;

// component for ThinIce mechanic
// observation: for SID 911 the distance is 0.1 * status extra
public abstract class ThinIce(BossModule module, float distance, bool createforbiddenzones = false, uint statusID = 911, bool stopAtWall = false, bool stopAfterWall = false) : Knockback(module, stopAtWall: stopAtWall, stopAfterWall: stopAfterWall)
{
    public readonly uint StatusID = statusID;
    public readonly float Distance = distance;
    private static readonly WDir offset = new(0, 1);
    public BitMask Mask;

    public override IEnumerable<Source> Sources(int slot, Actor actor)
    {
        if (Mask[slot] != default)
            yield return new(actor.Position, Distance, default, default, actor.Rotation, Kind.DirForward);
    }

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if (status.ID == StatusID)
            Mask.Set(Raid.FindSlot(actor.InstanceID));
    }

    public override void OnStatusLose(Actor actor, ActorStatus status)
    {
        if (status.ID == StatusID)
            Mask[Raid.FindSlot(actor.InstanceID)] = default;
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (CalculateMovements(slot, actor).Any(e => DestinationUnsafe(slot, actor, e.to)))
            hints.Add("You are risking to slide into danger!");
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        base.DrawArenaForeground(pcSlot, pc);
        if (Mask[pcSlot] != default)
            Arena.AddCircle(pc.Position, Distance, Colors.Vulnerable);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (createforbiddenzones && Mask[slot] != default)
        {
            var pos = actor.Position;
            var ddistance = 2 * Distance;
            var forbidden = new List<Func<WPos, float>>
            {
                ShapeDistance.InvertedDonut(pos, Distance, Distance + 0.5f),
                ShapeDistance.InvertedDonut(pos, ddistance, ddistance + 0.5f),
                ShapeDistance.InvertedRect(pos, offset, 0.5f, 0.5f, 0.5f)
            };
            float maxDistanceFunc(WPos pos)
            {
                var minDistance = float.MinValue;
                for (var i = 0; i < forbidden.Count; ++i)
                {
                    var distance = forbidden[i](pos);
                    if (distance > minDistance)
                        minDistance = distance;
                }
                return minDistance;
            }
            hints.AddForbiddenZone(maxDistanceFunc, DateTime.MaxValue);
        }
    }
}
