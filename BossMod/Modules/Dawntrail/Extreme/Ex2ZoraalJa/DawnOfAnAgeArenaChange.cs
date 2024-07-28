namespace BossMod.Dawntrail.Extreme.Ex2ZoraalJa;

class DawnOfAnAgeArenaChange(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCustom square = new([new Square(Ex2ZoraalJa.NormalCenter, 20, Ex2ZoraalJa.ArenaRotation)],
    [new Square(Ex2ZoraalJa.NormalCenter, 10, Ex2ZoraalJa.ArenaRotation)]);
    private AOEInstance? _aoe;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.DawnOfAnAge)
            _aoe = new(square, Module.Center, default, Module.CastFinishAt(spell, 0.9f));
    }

    public override void OnEventEnvControl(byte index, uint state)
    {
        if (state == 0x00200010 && index == 0x0B)
        {
            Module.Arena.Bounds = Ex2ZoraalJa.SmallBounds;
            _aoe = null;
        }
    }
}
