namespace BossMod.Endwalker.Variant.V02MR.V022Moko;

class ArenaChange(BossModule module) : Components.GenericAOEs(module)
{
    public static readonly WPos ArenaCenter = new(-700, 540);
    public static readonly ArenaBoundsSquare StartingBounds = new(24.6f);
    public static readonly ArenaBoundsSquare DefaultBounds = new(20);
    private static readonly AOEShapeCustom square = new([new Square(ArenaCenter, 25)], [new Square(ArenaCenter, 20)]);

    private AOEInstance? _aoe;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.KenkiRelease && Arena.Bounds == StartingBounds)
            _aoe = new(square, Module.Center, default, Module.CastFinishAt(spell, 2.1f));
    }

    public override void OnEventEnvControl(byte index, uint state)
    {
        if (state == 0x00020001 && index == 0x5B)
        {
            Module.Arena.Bounds = DefaultBounds;
            _aoe = null;
        }
    }
}
