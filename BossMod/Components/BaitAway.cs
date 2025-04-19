﻿namespace BossMod.Components;

// generic component for mechanics that require baiting some aoe (by proximity, by tether, etc) away from raid
// some players can be marked as 'forbidden' - if any of them is baiting, they are warned
// otherwise we show own bait as as outline (and warn if player is clipping someone) and other baits as filled (and warn if player is being clipped)
public class GenericBaitAway(BossModule module, uint aid = default, bool alwaysDrawOtherBaits = true, bool centerAtTarget = false, bool tankbuster = false, bool onlyShowOutlines = false) : CastCounter(module, aid)
{
    public struct Bait(Actor source, Actor target, AOEShape shape, DateTime activation = default, BitMask forbidden = default)
    {
        public Angle? CustomRotation;
        public AOEShape Shape = shape;
        public Actor Source = source;
        public Actor Target = target;
        public DateTime Activation = activation;
        public BitMask Forbidden = forbidden;

        public readonly Angle Rotation => CustomRotation ?? (Source != Target ? Angle.FromDirection(Target.Position - Source.Position) : Source.Rotation);

        public Bait(Actor source, Actor target, AOEShape shape, DateTime activation, Angle customRotation, BitMask forbidden = default)
            : this(source, target, shape, activation, forbidden)
        {
            CustomRotation = customRotation;
        }
    }

    public readonly bool AlwaysDrawOtherBaits = alwaysDrawOtherBaits; // if false, other baits are drawn only if they are clipping a player
    public readonly bool CenterAtTarget = centerAtTarget; // if true, aoe source is at target
    public readonly bool OnlyShowOutlines = onlyShowOutlines; // if true only show outlines
    public bool AllowDeadTargets = true; // if false, baits with dead targets are ignored
    public bool EnableHints = true;
    public bool IgnoreOtherBaits; // if true, don't show hints/aoes for baits by others
    public PlayerPriority BaiterPriority = PlayerPriority.Interesting;
    public BitMask ForbiddenPlayers; // these players should avoid baiting
    public List<Bait> CurrentBaits = [];
    public const string BaitAwayHint = "Bait away from raid!";

    public List<Bait> ActiveBaits
    {
        get
        {
            var count = CurrentBaits.Count;
            if (count == 0)
                return [];
            List<Bait> activeBaits = new(count);
            for (var i = 0; i < count; ++i)
            {
                var bait = CurrentBaits[i];
                if (!bait.Source.IsDead)
                {
                    if (AllowDeadTargets || !bait.Target.IsDead)
                        activeBaits.Add(bait);
                }
            }
            return activeBaits;
        }
    }

    public List<Bait> ActiveBaitsOn(Actor target)
    {
        var count = CurrentBaits.Count;
        if (count == 0)
            return [];
        List<Bait> activeBaitsOnTarget = new(count);
        for (var i = 0; i < count; ++i)
        {
            var bait = CurrentBaits[i];
            if (!bait.Source.IsDead && bait.Target == target)
                activeBaitsOnTarget.Add(bait);
        }
        return activeBaitsOnTarget;
    }

    public List<Bait> ActiveBaitsNotOn(Actor target)
    {
        var count = CurrentBaits.Count;
        if (count == 0)
            return [];
        List<Bait> activeBaitsNotOnTarget = new(count);
        for (var i = 0; i < count; ++i)
        {
            var bait = CurrentBaits[i];
            if (!bait.Source.IsDead && bait.Target != target)
                activeBaitsNotOnTarget.Add(bait);
        }
        return activeBaitsNotOnTarget;
    }

    public WPos BaitOrigin(Bait bait) => (CenterAtTarget ? bait.Target : bait.Source).Position;
    public bool IsClippedBy(Actor actor, Bait bait) => bait.Shape.Check(actor.Position, BaitOrigin(bait), bait.Rotation);
    public List<Actor> PlayersClippedBy(Bait bait)
    {
        var actors = Raid.WithoutSlot();
        var len = actors.Length;
        List<Actor> result = new(len);
        for (var i = 0; i < len; ++i)
        {
            ref readonly var actor = ref actors[i];
            if (actor != bait.Target && bait.Shape.Check(actor.Position, BaitOrigin(bait), bait.Rotation))
                result.Add(actor);
        }

        return result;
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (!EnableHints)
            return;
        var count = ActiveBaits.Count;
        if (count == 0)
            return;
        if (ForbiddenPlayers[slot])
        {
            var activeBaits = ActiveBaitsOn(actor);
            if (activeBaits.Count != 0)
                hints.Add("Avoid baiting!");
        }
        else
        {
            var activeBaits = ActiveBaitsOn(actor);
            for (var i = 0; i < activeBaits.Count; ++i)
            {
                var clippedPlayers = PlayersClippedBy(activeBaits[i]);
                if (clippedPlayers.Count != 0)
                {
                    hints.Add(BaitAwayHint);
                    break;
                }
            }
        }

        if (!IgnoreOtherBaits)
        {
            var otherActiveBaits = ActiveBaitsNotOn(actor);
            for (var i = 0; i < otherActiveBaits.Count; ++i)
            {
                if (IsClippedBy(actor, otherActiveBaits[i]))
                {
                    hints.Add("GTFO from baited aoe!");
                    break;
                }
            }
        }
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (ActiveBaits.Count == 0)
            return;
        var activeBaitsNotOnActor = ActiveBaitsNotOn(actor);
        var activeBaitsOnActor = ActiveBaitsOn(actor);
        var countActiveBaitsNotOnActor = activeBaitsNotOnActor.Count;
        var countActiveBaitsOnActor = activeBaitsOnActor.Count;

        for (var i = 0; i < countActiveBaitsNotOnActor; ++i)
        {
            var bait = activeBaitsNotOnActor[i];
            hints.AddForbiddenZone(bait.Shape, BaitOrigin(bait), bait.Rotation, bait.Activation);
        }
        for (var i = 0; i < countActiveBaitsOnActor; ++i)
        {
            var bait = activeBaitsOnActor[i];
            AddTargetSpecificHints(ref actor, ref bait, ref hints);
        }
    }

    private void AddTargetSpecificHints(ref Actor actor, ref Bait bait, ref AIHints hints)
    {
        if (bait.Source == bait.Target) // TODO: think about how to handle source == target baits, eg. vomitting mechanics
            return;
        var raid = Raid.WithoutSlot();
        var len = raid.Length;
        for (var i = 0; i < len; ++i)
        {
            var a = raid[i];
            if (a == actor)
                continue;
            switch (bait.Shape)
            {
                case AOEShapeDonut:
                case AOEShapeCircle:
                    hints.AddForbiddenZone(bait.Shape, a.Position, default, bait.Activation);
                    break;
                case AOEShapeCone cone:
                    hints.AddForbiddenZone(ShapeDistance.Cone(bait.Source.Position, 100f, bait.Source.AngleTo(a), cone.HalfAngle), bait.Activation);
                    break;
                case AOEShapeRect rect:
                    hints.AddForbiddenZone(ShapeDistance.Cone(bait.Source.Position, 100f, bait.Source.AngleTo(a), Angle.Asin(rect.HalfWidth / (a.Position - bait.Source.Position).Length())), bait.Activation);
                    break;
                case AOEShapeCross cross:
                    hints.AddForbiddenZone(cross, a.Position, bait.Rotation, bait.Activation);
                    break;
            }
        }
    }

    public override PlayerPriority CalcPriority(int pcSlot, Actor pc, int playerSlot, Actor player, ref uint customColor) => ActiveBaitsOn(player).Count != 0 ? BaiterPriority : PlayerPriority.Irrelevant;

    public override void DrawArenaBackground(int pcSlot, Actor pc)
    {
        if (OnlyShowOutlines || IgnoreOtherBaits)
            return;

        var baits = CollectionsMarshal.AsSpan(CurrentBaits);
        var len = baits.Length;
        for (var i = 0; i < len; ++i)
        {
            ref readonly var b = ref baits[i];
            if (!b.Source.IsDead && b.Target != pc && (AlwaysDrawOtherBaits || IsClippedBy(pc, b)))
                b.Shape.Draw(Arena, BaitOrigin(b), b.Rotation);
        }
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        var baits = CollectionsMarshal.AsSpan(CurrentBaits);
        var len = baits.Length;
        for (var i = 0; i < len; ++i)
        {
            ref readonly var b = ref baits[i];
            if (!b.Source.IsDead && (OnlyShowOutlines || !OnlyShowOutlines && b.Target == pc))
                b.Shape.Outline(Arena, BaitOrigin(b), b.Rotation);
        }
    }

    public override void AddGlobalHints(GlobalHints hints)
    {
        if (tankbuster && CurrentBaits.Count != 0)
            hints.Add("Tankbuster cleave");
    }
}

// bait on all players, requiring everyone to spread out
public class BaitAwayEveryone : GenericBaitAway
{
    public BaitAwayEveryone(BossModule module, Actor? source, AOEShape shape, uint aid = default) : base(module, aid)
    {
        AllowDeadTargets = false;
        if (source != null)
            CurrentBaits.AddRange(Raid.WithoutSlot(true).Select(p => new Bait(source, p, shape)));
    }
}

// component for mechanics requiring tether targets to bait their aoe away from raid
public class BaitAwayTethers(BossModule module, AOEShape shape, uint tetherID, uint aid = default, uint enemyOID = default, float activationDelay = default, bool centerAtTarget = false) : GenericBaitAway(module, aid, centerAtTarget: centerAtTarget)
{
    public AOEShape Shape = shape;
    public uint TID = tetherID;
    public bool DrawTethers = true;
    public readonly List<Actor> _enemies = module.Enemies(enemyOID);
    public float ActivationDelay = activationDelay;

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        base.DrawArenaForeground(pcSlot, pc);
        if (DrawTethers)
        {
            foreach (var b in ActiveBaits)
                Arena.AddLine(b.Source.Position, b.Target.Position);
        }
    }

    public override void OnTethered(Actor source, ActorTetherInfo tether)
    {
        var (player, enemy) = DetermineTetherSides(source, tether);
        if (player != null && enemy != null && (enemyOID == default || _enemies.Contains(source)))
            CurrentBaits.Add(new(enemy, player, Shape, WorldState.FutureTime(ActivationDelay)));
    }

    public override void OnUntethered(Actor source, ActorTetherInfo tether)
    {
        var (player, enemy) = DetermineTetherSides(source, tether);
        if (player != null && enemy != null)
        {
            CurrentBaits.RemoveAll(b => b.Source == enemy && b.Target == player);
        }
    }

    // we support both player->enemy and enemy->player tethers
    public (Actor? player, Actor? enemy) DetermineTetherSides(Actor source, ActorTetherInfo tether)
    {
        if (tether.ID != TID)
            return (null, null);

        var target = WorldState.Actors.Find(tether.Target);
        if (target == null)
            return (null, null);

        var (player, enemy) = source.Type is ActorType.Player or ActorType.Buddy ? (source, target) : (target, source);
        if (player.Type is not ActorType.Player and not ActorType.Buddy || enemy.Type is ActorType.Player or ActorType.Buddy)
        {
            ReportError($"Unexpected tether pair: {source.InstanceID:X} -> {target.InstanceID:X}");
            return (null, null);
        }

        return (player, enemy);
    }
}

// component for mechanics requiring icon targets to bait their aoe away from raid
public class BaitAwayIcon(BossModule module, AOEShape shape, uint iconID, uint aid = default, float activationDelay = 5.1f, bool centerAtTarget = false, Actor? source = null, bool tankbuster = false) : GenericBaitAway(module, aid, centerAtTarget: centerAtTarget, tankbuster: tankbuster)
{
    public AOEShape Shape = shape;
    public uint IID = iconID;
    public float ActivationDelay = activationDelay;

    public virtual Actor? BaitSource(Actor target) => source ?? Module.PrimaryActor;

    public override void OnEventIcon(Actor actor, uint iconID, ulong targetID)
    {
        if (iconID == IID && BaitSource(actor) is var source && source != null)
        {
            CurrentBaits.Add(new(source, WorldState.Actors.Find(targetID) ?? actor, Shape, WorldState.FutureTime(ActivationDelay)));
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        base.OnEventCast(caster, spell);
        if (spell.Action.ID == WatchedAction)
            CurrentBaits.Clear();
    }
}

// component for mechanics requiring cast targets to gtfo from raid (aoe tankbusters etc)
public class BaitAwayCast(BossModule module, uint aid, AOEShape shape, bool centerAtTarget = false, bool endsOnCastEvent = false, bool tankbuster = false) : GenericBaitAway(module, aid, centerAtTarget: centerAtTarget, tankbuster: tankbuster)
{
    public AOEShape Shape = shape;
    public bool EndsOnCastEvent = endsOnCastEvent;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == WatchedAction && WorldState.Actors.Find(spell.TargetID) is var target && target != null)
            CurrentBaits.Add(new(caster, target, Shape, Module.CastFinishAt(spell)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == WatchedAction && !EndsOnCastEvent)
            CurrentBaits.RemoveAll(b => b.Source == caster);
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        base.OnEventCast(caster, spell);
        if (spell.Action.ID == WatchedAction && EndsOnCastEvent)
            CurrentBaits.RemoveAll(b => b.Source == caster);
    }
}

// a variation of BaitAwayCast for charges that end at target
public class BaitAwayChargeCast(BossModule module, uint aid, float halfWidth) : GenericBaitAway(module, aid)
{
    private readonly AOEShapeRect rect = new(default, halfWidth);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == WatchedAction && WorldState.Actors.Find(spell.TargetID) is var target && target != null)
            CurrentBaits.Add(new(caster, target, rect, Module.CastFinishAt(spell)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == WatchedAction)
            CurrentBaits.RemoveAll(b => b.Source == caster);
    }

    public override void Update()
    {
        var count = CurrentBaits.Count;
        if (count == 0)
            return;
        var baits = CollectionsMarshal.AsSpan(CurrentBaits);
        for (var i = 0; i < count; ++i)
        {
            ref var b = ref baits[i];
            b.Shape = rect with { LengthFront = (b.Target.Position - b.Source.Position).Length() };
        }
    }
}

// a variation of baits with tethers for charges that end at target
public class BaitAwayChargeTether(BossModule module, float halfWidth, float activationDelay, uint aidGood, uint aidBad = default, uint tetherIDBad = 57u, uint tetherIDGood = 1u, uint enemyOID = default, float minimumDistance = default)
: StretchTetherDuo(module, minimumDistance, activationDelay, tetherIDBad, tetherIDGood, new AOEShapeRect(default, halfWidth), default, enemyOID)
{
    public uint AidGood = aidGood;
    public uint AidBad = aidBad; // supports 2nd AID incase the AID changes between good and bad tethers
    public uint TetherIDBad = tetherIDBad;
    public uint TetherIDGood = tetherIDGood;
    public float HalfWidth = halfWidth;

    public override void Update()
    {
        base.Update();
        var count = CurrentBaits.Count;
        if (count == 0)
            return;
        var baits = CollectionsMarshal.AsSpan(CurrentBaits);
        for (var i = 0; i < count; ++i)
        {
            ref var b = ref baits[i];
            if (b.Shape is AOEShapeRect shape)
            {
                var length = (b.Target.Position - b.Source.Position).Length();
                if (shape.LengthFront != length)
                {
                    b.Shape = shape with { LengthFront = length };
                }
            }
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action.ID == AidGood || spell.Action.ID == AidBad)
        {
            ++NumCasts;
            CurrentBaits.RemoveAll(x => x.Target == WorldState.Actors.Find(spell.MainTargetID));
        }
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (ActiveBaits.Count == 0)
            return;
        base.AddHints(slot, actor, hints);
        if (ActiveBaitsOn(actor).Any(b => PlayersClippedBy(b).Count != 0))
            hints.Add(BaitAwayHint);
    }
}
