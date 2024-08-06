namespace BossMod.Dawntrail.Raid.M01NBlackCat;

class ElevateAndEviscerate(BossModule module) : Components.Knockback(module, ignoreImmunes: true, stopAfterWall: true)
{
    private DateTime activation;
    public (Actor source, Actor target) Tether;

    public override IEnumerable<Source> Sources(int slot, Actor actor)
    {
        if (Tether != default && actor == Tether.target)
            yield return new(Tether.source.Position, 10, activation);
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.ElevateAndEviscerate)
            activation = Module.CastFinishAt(spell);
    }

    public override void OnTethered(Actor source, ActorTetherInfo tether)
    {
        if (tether.ID is (uint)TetherID.ElevateAndEviscerateGood or (uint)TetherID.ElevateAndEviscerateBad)
            Tether = (source, WorldState.Actors.Find(tether.Target)!);
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.ElevateAndEviscerate)
        {
            Tether = default;
            ++NumCasts;
        }
    }

    public override bool DestinationUnsafe(int slot, Actor actor, WPos pos) => (Module.FindComponent<ElevateAndEviscerateHint>()?.ActiveAOEs(slot, actor).Any(z => z.Shape.Check(pos, z.Origin, z.Rotation)) ?? false) || !Module.InBounds(pos);
}

class ElevateAndEviscerateHint(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect rect = new(5, 5, 5);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var tether = Module.FindComponent<ElevateAndEviscerate>()!.Tether;
        if (tether != default && actor == tether.target)
        {
            var tiles = Module.FindComponent<ArenaChanges>()!.DamagedTiles;
            foreach (var t in tiles.OfType<Square>())
                yield return new(rect, t.Center, Color: Colors.FutureVulnerable, Risky: false);
        }
    }
}
