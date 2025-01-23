namespace BossMod.Stormblood.Foray.BaldesionArsenal.BA3AbsoluteVirtue;

class BrightDarkAuroraExplosion(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCircle circle = new(8);
    private readonly List<(Actor source, ulong target)> tetherByActor = new(8);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = tetherByActor.Count;
        if (count == 0)
            return [];

        var isActorTarget = false;

        for (var i = 0; i < count; ++i)
        {
            var tether = tetherByActor[i];
            if (tether.target == actor.InstanceID)
            {
                isActorTarget = true;
                break;
            }
        }

        List<AOEInstance> aoes = new(count);
        for (var i = 0; i < count; ++i)
        {
            var tether = tetherByActor[i];
            if (tether.target != actor.InstanceID)
                aoes.Add(new(circle, tetherByActor[i].source.Position, Risky: !isActorTarget));
        }
        return aoes;
    }

    public override void OnTethered(Actor source, ActorTetherInfo tether)
    {
        tetherByActor.Add((source, tether.Target));
    }

    public override void OnUntethered(Actor source, ActorTetherInfo tether)
    {
        tetherByActor.Remove((source, tether.Target));
    }
}

abstract class Towers(BossModule module, OID oid, TetherID tid) : Components.GenericTowersOpenWorld(module)
{
    private readonly List<(Actor source, Actor target)> tetherByActor = new(4);
    private const string Hint = "Stand in a tower of opposite tether element!";

    public override void OnActorEAnim(Actor actor, uint state)
    {
        if ((OID)actor.OID == oid)
        {
            if (state == 0x00040008)
            {
                for (var i = 0; i < Towers.Count; ++i)
                {
                    var tower = Towers[i];
                    if (tower.Position == actor.Position)
                    {
                        Towers.Remove(tower);
                        break;
                    }
                }
            }
        }
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == oid)
            Towers.Add(new(actor.Position, 2, 1, 1, [], WorldState.FutureTime(20)));
    }

    public override void OnTethered(Actor source, ActorTetherInfo tether)
    {
        if (tether.ID == (uint)tid)
            tetherByActor.Add((source, WorldState.Actors.Find(tether.Target)!));
    }

    public override void OnUntethered(Actor source, ActorTetherInfo tether)
    {
        if (tether.ID == (uint)tid)
            tetherByActor.Remove((source, WorldState.Actors.Find(tether.Target)!));
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        var count = tetherByActor.Count;
        if (count == 0)
            return;

        var isActorTarget = false;

        for (var i = 0; i < count; ++i)
        {
            var tether = tetherByActor[i];
            if (tether.target == actor)
            {
                isActorTarget = true;
                break;
            }
        }

        if (isActorTarget)
        {
            var soakedIndex = -1;
            for (var i = 0; i < Towers.Count; ++i)
            {
                var t = Towers[i];
                var allowedSoakers = t.AllowedSoakers ??= Tower.Soakers(Module);
                if (allowedSoakers.Contains(actor) && t.IsInside(actor))
                {
                    soakedIndex = i;
                    break;
                }
            }
            if (soakedIndex == -1)
                hints.Add(Hint);
            else
                hints.Add(Hint, false);
        }
        else
            base.AddHints(slot, actor, hints);
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        base.DrawArenaForeground(pcSlot, pc);
        var count = tetherByActor.Count;
        if (count == 0)
            return;

        Actor? source = null;
        for (var i = 0; i < count; ++i)
        {
            var tether = tetherByActor[i];
            if (tether.target == pc)
            {
                source = tether.source;
                break;
            }
        }
        if (source != null)
        {
            Arena.AddLine(source.Position, pc.Position);
            Arena.AddCircle(source.Position, 2);
            Arena.Actor(source, Colors.Object, true);
        }
    }

    public override void Update()
    {
        var count = Towers.Count;
        if (count == 0)
            return;
        HashSet<Actor> allowed = new(4);
        for (var i = 0; i < tetherByActor.Count; ++i)
            allowed.Add(tetherByActor[i].target);
        for (var i = 0; i < count; ++i)
            Towers[i] = Towers[i] with { AllowedSoakers = allowed };
    }
}

class BrightAuroraTether(BossModule module) : Towers(module, OID.DarkAuroraHelper, TetherID.BrightAurora);
class DarkAuroraTether(BossModule module) : Towers(module, OID.BrightAuroraHelper, TetherID.DarkAurora);
