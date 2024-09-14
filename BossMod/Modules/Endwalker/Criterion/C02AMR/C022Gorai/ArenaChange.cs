namespace BossMod.Endwalker.VariantCriterion.C02AMR.C022Gorai;

class ArenaChange(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCustom square = new([new Square(C022Gorai.ArenaCenter, 23)], [new Square(C022Gorai.ArenaCenter, 20)]);

    private AOEInstance? _aoe;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.Unenlightenment && Arena.Bounds == C022Gorai.StartingBounds)
            _aoe = new(square, Arena.Center, default, Module.CastFinishAt(spell, 0.5f));
    }

    public override void OnEventEnvControl(byte index, uint state)
    {
        if (state == 0x00020001 && index == 0x02)
        {
            Arena.Bounds = C022Gorai.DefaultBounds;
            _aoe = null;
        }
    }
}
