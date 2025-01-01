namespace BossMod.Components;

// generic component that shows arbitrary shapes representing avoidable aoes
public abstract class GenericAOEs(BossModule module, ActionID aid = default, string warningText = "GTFO from aoe!") : CastCounter(module, aid)
{
    public record struct AOEInstance(AOEShape Shape, WPos Origin, Angle Rotation = default, DateTime Activation = default, uint Color = 0, bool Risky = true)
    {
        public readonly bool Check(WPos pos) => Shape.Check(pos, Origin, Rotation);
    }

    public string WarningText = warningText;

    public abstract IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor);

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (ActiveAOEs(slot, actor).Any(c => c.Risky && c.Check(actor.Position)))
            hints.Add(WarningText);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        foreach (var c in ActiveAOEs(slot, actor))
            if (c.Risky)
                hints.AddForbiddenZone(c.Shape, c.Origin, c.Rotation, c.Activation);
    }

    public override void DrawArenaBackground(int pcSlot, Actor pc)
    {
        foreach (var c in ActiveAOEs(pcSlot, pc))
            c.Shape.Draw(Arena, c.Origin, c.Rotation, c.Color == 0 ? Colors.AOE : c.Color);
    }
}

// self-targeted aoe that happens at the end of the cast
public class SelfTargetedAOEs(BossModule module, ActionID aid, AOEShape shape, int maxCasts = int.MaxValue, float riskyAfterSeconds = 0, uint color = 0) : GenericAOEs(module, aid)
{
    public readonly AOEShape Shape = shape;
    public int MaxCasts = maxCasts; // used for staggered aoes, when showing all active would be pointless
    public uint Color = color;
    public bool Risky = true; // can be customized if needed
    public float RiskyAfterSeconds = riskyAfterSeconds; // can be used to delay risky status of AOEs, so AI waits longer to dodge, if 0 it will just use the bool Risky
    public readonly List<Actor> Casters = [];

    public IEnumerable<Actor> ActiveCasters => Casters.Take(MaxCasts);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = Casters.Count;
        if (count == 0)
            return [];
        var min = count > MaxCasts ? MaxCasts : count;
        var aoes = new List<AOEInstance>(min);
        for (var i = 0; i < min; ++i)
        {
            var caster = Casters[i];
            AOEInstance aoeInstance = new(Shape, caster.Position, caster.CastInfo!.Rotation, Module.CastFinishAt(caster.CastInfo), Color, RiskyAfterSeconds == 0 ? Risky : caster.CastInfo.ElapsedTime > RiskyAfterSeconds);
            aoes.Add(aoeInstance);
        }
        return aoes;
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action == WatchedAction)
            Casters.Add(caster);
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action == WatchedAction)
            Casters.Remove(caster);
    }
}

// self-targeted aoe that uses current caster's rotation instead of rotation from cast-info - used by legacy modules written before i've reversed real cast rotation
public class SelfTargetedLegacyRotationAOEs(BossModule module, ActionID aid, AOEShape shape, int maxCasts = int.MaxValue) : GenericAOEs(module, aid)
{
    public readonly AOEShape Shape = shape;
    public int MaxCasts = maxCasts; // used for staggered aoes, when showing all active would be pointless
    public readonly List<Actor> Casters = [];

    public IEnumerable<Actor> ActiveCasters => Casters.Take(MaxCasts);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => ActiveCasters.Select(c => new AOEInstance(Shape, c.Position, c.Rotation, Module.CastFinishAt(c.CastInfo)));

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action == WatchedAction)
            Casters.Add(caster);
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action == WatchedAction)
            Casters.Remove(caster);
    }
}

// location-targeted circle aoe that happens at the end of the cast
public class LocationTargetedAOEs(BossModule module, ActionID aid, AOEShape shape, int maxCasts = int.MaxValue, float riskyWithSecondsLeft = 0, bool targetIsLocation = false) : GenericAOEs(module, aid)
{
    public LocationTargetedAOEs(BossModule module, ActionID aid, float radius, float riskyWithSecondsLeft = 0, int maxCasts = int.MaxValue, bool targetIsLocation = false) : this(module, aid, new AOEShapeCircle(radius), maxCasts, riskyWithSecondsLeft, targetIsLocation) { }
    public readonly AOEShape Shape = shape;
    public readonly int MaxCasts = maxCasts; // used for staggered aoes, when showing all active would be pointless
    public uint Color; // can be customized if needed
    public bool Risky = true; // can be customized if needed
    public readonly bool TargetIsLocation = targetIsLocation; // can be customized if needed
    public float RiskyWithSecondsLeft = riskyWithSecondsLeft; // can be used to delay risky status of AOEs, so AI waits longer to dodge, if 0 it will just use the bool Risky

    public readonly List<AOEInstance> Casters = [];
    public IEnumerable<AOEInstance> ActiveCasters => Casters.Take(MaxCasts);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = Casters.Count;
        if (count == 0)
            yield break;
        var time = WorldState.CurrentTime;
        for (var i = 0; i < (count > MaxCasts ? MaxCasts : count); ++i)
        {
            var caster = Casters[i];
            yield return RiskyWithSecondsLeft == 0 ? caster : caster with { Risky = caster.Activation.AddSeconds(-RiskyWithSecondsLeft) <= time };
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action == WatchedAction)
            Casters.Add(new(Shape, (TargetIsLocation ? WorldState.Actors.Find(caster.CastInfo!.TargetID)?.Position : spell.LocXZ) ?? default, spell.Rotation, Module.CastFinishAt(spell), Color, Risky));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (Casters.Count != 0 && spell.Action == WatchedAction)
            Casters.RemoveAt(0);
    }
}

// 'charge at location' aoes that happen at the end of the cast
public class ChargeAOEs(BossModule module, ActionID aid, float halfWidth, float riskyAfterSeconds = 0) : GenericAOEs(module, aid)
{
    public readonly float HalfWidth = halfWidth;
    public float RiskyAfterSeconds = riskyAfterSeconds; // can be used to delay risky status of AOEs, so AI waits longer to dodge, if 0 it will just use the bool Risky
    public readonly List<(Actor caster, AOEShape shape, Angle direction)> Casters = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = Casters.Count;
        if (count == 0)
            return [];
        var aoes = new List<AOEInstance>(count);
        for (var i = 0; i < count; ++i)
        {
            var csr = Casters[i];
            AOEInstance aoeInstance = new(csr.shape, csr.caster.Position, csr.direction, Module.CastFinishAt(csr.caster.CastInfo), Risky: csr.caster.CastInfo?.ElapsedTime > RiskyAfterSeconds);
            aoes.Add(aoeInstance);
        }
        return aoes;
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action == WatchedAction)
        {
            var dir = spell.LocXZ - caster.Position;
            Casters.Add((caster, new AOEShapeRect(dir.Length(), HalfWidth), Angle.FromDirection(dir)));
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action == WatchedAction)
            Casters.RemoveAll(e => e.caster == caster);
    }
}
