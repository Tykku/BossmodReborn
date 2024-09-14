namespace BossMod.Endwalker.VariantCriterion.V02MR.V023Gorai;

class ImpurePurgation(BossModule module) : Components.GenericAOEs(module, ActionID.MakeSpell(AID.ImpurePurgation))
{
    private readonly List<Actor> _castersPurgationFirst = [];
    private readonly List<Actor> _castersPurgationNext = [];

    private static readonly AOEShapeCone cone = new(60, 22.5f.Degrees());

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        return _castersPurgationFirst.Count > 0
            ? _castersPurgationFirst.Select(c => new AOEInstance(cone, c.Position, c.CastInfo!.Rotation, Module.CastFinishAt(c.CastInfo)))
            : _castersPurgationNext.Select(c => new AOEInstance(cone, c.Position, c.CastInfo!.Rotation, Module.CastFinishAt(c.CastInfo)));
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        CastersForSpell(spell.Action)?.Add(caster);
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        CastersForSpell(spell.Action)?.Remove(caster);
    }

    private List<Actor>? CastersForSpell(ActionID spell) => (AID)spell.ID switch
    {
        AID.ImpurePurgationFirst => _castersPurgationFirst,
        AID.ImpurePurgationSecond => _castersPurgationNext,
        _ => null
    };
}
