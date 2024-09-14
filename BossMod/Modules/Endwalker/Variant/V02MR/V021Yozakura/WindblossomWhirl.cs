namespace BossMod.Endwalker.VariantCriterion.V02MR.V021Yozakura;

class WindblossomWhirl(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeDonut donut = new(5, 60);
    private AOEInstance? _aoe;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.WindblossomWhirlVisual)
            _aoe = new(donut, Arena.Center, default, Module.CastFinishAt(spell, 6.3f));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.WindblossomWhirl1 or AID.WindblossomWhirl2)
        {
            if (++NumCasts == 5 && _aoe != null)
            {
                _aoe = null;
                NumCasts = 0;
            }
        }
    }
}
