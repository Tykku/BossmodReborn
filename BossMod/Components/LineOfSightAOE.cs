namespace BossMod.Components;

// generic component that shows line-of-sight cones for arbitrary origin and blocking shapes
// TODO: add support for multiple AOE sources at the same time (I simplified Hermes from 4 AOEs into one)
// add support for blockers that spawn or get destroyed after cast already started (Hermes: again a cheat here by only using that meteor that exists for the whole mechanic)
public abstract class GenericLineOfSightAOE(BossModule module, ActionID aid, float maxRange, bool blockersImpassable, bool rect = false) : GenericAOEs(module, aid, "Hide behind obstacle!")
{
    public DateTime NextExplosion;
    public bool BlockersImpassable = blockersImpassable;
    public float MaxRange { get; private set; } = maxRange;
    public bool Rect { get; private set; } = rect; // if the AOE is a rectangle instead of a circle
    public WPos? Origin { get; private set; } // inactive if null
    public List<(WPos Center, float Radius)> Blockers { get; private set; } = [];
    public List<(float Distance, Angle Dir, Angle HalfWidth)> Visibility { get; private set; } = [];
    public List<AOEInstance> InvertedAOE = [];
    public List<Shape> UnionShapes = [];
    public List<Shape> DifferenceShapes = [];
    private const float PCHitBoxRadius = 0.5f;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => InvertedAOE.Take(1);
    public void Modify(WPos? origin, IEnumerable<(WPos Center, float Radius)> blockers, DateTime nextExplosion = default)
    {
        NextExplosion = nextExplosion;
        Origin = origin;
        Blockers.Clear();
        Blockers.AddRange(blockers);
        Visibility.Clear();
        if (origin != null)
        {
            foreach (var b in Blockers)
            {
                var toBlock = b.Center - origin.Value;
                var dist = toBlock.Length();
                Visibility.Add((dist, Angle.FromDirection(toBlock), b.Radius < dist ? Angle.Asin(b.Radius / dist) : 90.Degrees()));
            }
        }
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (ActiveAOEs(slot, actor).Any(c => c.Risky && !c.Check(actor.Position)) && Origin != null && ((WPos)Origin - actor.Position).Length() < MaxRange)
            hints.Add(WarningText);
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (Origin != null && spell.Action == WatchedAction)
        {
            if (!Rect)
            {
                foreach (var v in Visibility)
                    UnionShapes.Add(new DonutSegmentHA(Origin.Value, v.Distance + 0.1f, MaxRange, v.Dir, v.HalfWidth));
                if (BlockersImpassable)
                    foreach (var b in Blockers)
                        DifferenceShapes.Add(new Circle(b.Center, b.Radius + PCHitBoxRadius));
            }
            else if (Rect)
            {
                foreach (var b in Blockers)
                {
                    UnionShapes.Add(new RectangleSE(b.Center, b.Center + MaxRange * caster.Rotation.ToDirection(), b.Radius));
                    if (BlockersImpassable)
                        DifferenceShapes.Add(new Circle(b.Center, b.Radius + PCHitBoxRadius));
                }
            }
            InvertedAOE.Add(new(new AOEShapeCustom(CopyShapes(UnionShapes), CopyShapes(DifferenceShapes), true), Module.Arena.Center, default, Module.CastFinishAt(spell), Colors.SafeFromAOE));
            UnionShapes.Clear();
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (InvertedAOE.Count > 0 && spell.Action == WatchedAction)
            InvertedAOE.RemoveAt(0);
    }

    private List<Shape> CopyShapes(List<Shape> shapes)
    {
        var copy = new List<Shape>();
        copy.AddRange(shapes);
        return copy;
    }
}

// simple line-of-sight aoe that happens at the end of the cast
public abstract class CastLineOfSightAOE : GenericLineOfSightAOE
{
    private readonly List<Actor> _casters = [];
    public Actor? ActiveCaster => _casters.MinBy(c => c.CastInfo!.RemainingTime);

    protected CastLineOfSightAOE(BossModule module, ActionID aid, float maxRange, bool blockersImpassable, bool rect = false) : base(module, aid, maxRange, blockersImpassable, rect)
    {
        Refresh();
    }

    public abstract IEnumerable<Actor> BlockerActors();

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action == WatchedAction)
        {
            _casters.Add(caster);
            Refresh();
        }
        base.OnCastStarted(caster, spell);
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action == WatchedAction)
        {
            _casters.Remove(caster);
            Refresh();
        }
        base.OnCastFinished(caster, spell);
    }

    private void Refresh()
    {
        var caster = ActiveCaster;
        WPos? position = caster != null ? (WorldState.Actors.Find(caster.CastInfo!.TargetID)?.Position ?? caster.CastInfo!.LocXZ) : null;
        Modify(position, BlockerActors().Select(b => (b.Position, b.HitboxRadius)), Module.CastFinishAt(caster?.CastInfo));
    }
}
