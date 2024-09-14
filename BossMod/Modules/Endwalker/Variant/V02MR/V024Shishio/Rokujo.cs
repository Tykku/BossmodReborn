namespace BossMod.Endwalker.VariantCriterion.V02MR.V024Shishio;

class Rokujos(BossModule module, AID aid) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(aid), new AOEShapeRect(30, 7, 30))
{
    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var aoes = ActiveCasters.Select((c, index) =>
            new AOEInstance(Shape, c.Position, c.CastInfo!.Rotation, Module.CastFinishAt(c.CastInfo),
            index < 1 ? Colors.Danger : Colors.AOE));

        return aoes;
    }
}

class OnceOnRokujo(BossModule module) : Rokujos(module, AID.OnceOnRokujo);
class TwiceOnRokujo(BossModule module) : Rokujos(module, AID.TwiceOnRokujo);
class ThriceOnRokujo(BossModule module) : Rokujos(module, AID.ThriceOnRokujo);

class Rokujo(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCircle[] circles = [new(8), new(12), new(23)];
    private readonly List<AOEInstance> _aoes = [];
    private readonly List<Actor> _clouds = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    => _aoes.Skip(2).Take(4).Concat(_aoes.Take(2).Select(a => a with { Color = _aoes.Count > 2 ? Colors.Danger : Colors.AOE }));

    public override void OnActorDestroyed(Actor actor)
    {
        if ((OID)actor.OID == OID.Raiun)
            _clouds.Remove(actor);
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.Raiun)
            _clouds.Add(actor);
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.SmokeaterAbsorb)
            _clouds.Remove(caster);
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.OnceOnRokujo:
                RemoveCloudsAndAddAOEs(caster, spell, circles[0], circles[0].Radius, 1, aoeCount => 1 + MathF.Ceiling(aoeCount / 2));
                break;
            case AID.TwiceOnRokujo:
                RemoveCloudsAndAddAOEs(caster, spell, circles[1], circles[1].Radius, 1.6f, aoeCount => 3.1f);
                break;
            case AID.ThriceOnRokujoVisual1:
                foreach (var cloud in _clouds)
                    _aoes.Add(new(circles[2], cloud.Position, default, Module.CastFinishAt(spell, 7.1f)));
                break;
        }
    }

    private void RemoveCloudsAndAddAOEs(Actor caster, ActorCastInfo spell, AOEShapeCircle circle, float radius, float initialActivation, Func<int, float> activation)
    {
        var cloudsToRemove = _clouds.Where(c => c.Position.InRect(caster.Position, spell.Rotation, 30, 30, 7));
        foreach (var c in cloudsToRemove)
            _aoes.Add(new(circle, c.Position, default, Module.CastFinishAt(spell, initialActivation)));
        _clouds.RemoveAll(cloudsToRemove.Contains);

        while (_clouds.Count > 0)
        {
            List<AOEInstance> newAOEs = [];
            List<Actor> cloudsToRemoveInThisPass = [];

            foreach (var c in _clouds)
                if (_aoes.Skip(_aoes.Count - 2).Any(a => a.Origin.InCircle(c.Position, radius)))
                {
                    newAOEs.Add(new(circle, c.Position, default, Module.CastFinishAt(spell, activation(_aoes.Count))));
                    cloudsToRemoveInThisPass.Add(c);
                }
            if (cloudsToRemoveInThisPass.Count == 0)
                break;  // avoid infinite loop if no clouds are found for removal for whatever reason

            _aoes.AddRange(newAOEs);
            _clouds.RemoveAll(cloudsToRemoveInThisPass.Contains);
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (_aoes.Count > 0)
            switch ((AID)spell.Action.ID)
            {
                case AID.LeapingLevin1:
                case AID.LeapingLevin2:
                case AID.LeapingLevin3:
                    _aoes.RemoveAt(0);
                    break;
            }
    }
}
