namespace BossMod.Endwalker.VariantCriterion.C02AMR.C021Shishio;

class ArenaChange(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCustom square = new([new Square(C021Shishio.ArenaCenter, 23)], [new Square(C021Shishio.ArenaCenter, 20)]);
    private static readonly AOEShapeDonut donut = new(20, 30);

    private AOEInstance? _aoe;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.NEnkyo or AID.SEnkyo && Arena.Bounds == C021Shishio.StartingBounds)
            _aoe = new(square, Arena.Center, default, Module.CastFinishAt(spell, 0.8f));
        else if ((AID)spell.Action.ID is AID.NStormcloudSummons or AID.SStormcloudSummons)
            _aoe = new(donut, Arena.Center, default, Module.CastFinishAt(spell, 0.8f));
    }

    public override void OnEventEnvControl(byte index, uint state)
    {
        if (state == 0x00020001)
        {
            if (index == 0x34)
            {
                Arena.Bounds = C021Shishio.CircleBounds;
                _aoe = null;
            }
            else if (index == 0x35)
            {
                Arena.Bounds = C021Shishio.DefaultBounds;
                _aoe = null;
            }
        }
    }
}
