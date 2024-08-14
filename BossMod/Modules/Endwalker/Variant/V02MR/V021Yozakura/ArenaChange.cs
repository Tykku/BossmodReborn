namespace BossMod.Endwalker.Variant.V02MR.V021Yozakura;

class ArenaChange(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCustom square1 = new([new Square(V021Yozakura.ArenaCenter1, 22.5f)], [new Square(V021Yozakura.ArenaCenter1, 20)]);
    private AOEInstance? _aoe;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.GloryNeverlasting && Arena.Bounds == V021Yozakura.StartingBounds && Arena.Center == V021Yozakura.ArenaCenter1)
            _aoe = new(square1, Module.Center, default, Module.CastFinishAt(spell, 3.7f));
    }

    public override void OnEventEnvControl(byte index, uint state)
    {
        if (state == 0x00020001 && index == 0x35)
        {
            Module.Arena.Bounds = V021Yozakura.DefaultBounds;
            _aoe = null;
        }
    }
}
