namespace BossMod.Dawntrail.Alliance.A12Fafnir;

class DragonBreath(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeDonut donut = new(16, 30);
    private AOEInstance? _aoe;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.OffensivePostureVisual2)
            _aoe = new(donut, Arena.Center, default, Module.CastFinishAt(spell, 1.2f));
    }

    public override void OnActorEAnim(Actor actor, uint state)
    {
        if (state == 0x00010002 && (OID)actor.OID == OID.FireVoidzone)
            _aoe = null;
    }
}
