namespace BossMod.Endwalker.VariantCriterion.C01ASS.C013Shadowcaster;

class BlazingBenifice(BossModule module, AID aid, OID oid) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];
    private static readonly AOEShapeRect rect = new(100, 5);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_aoes.Count > 0)
            foreach (var aoe in _aoes)
                if ((aoe.Activation - WorldState.CurrentTime).TotalSeconds <= 5 && (aoe.Activation - _aoes[0].Activation).TotalSeconds <= 1)
                    yield return aoe;
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == oid)
            _aoes.Add(new(rect, actor.Position, actor.Rotation, WorldState.FutureTime(21.7f)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (_aoes.Count > 0 && (AID)spell.Action.ID == aid)
            _aoes.RemoveAt(0);
    }
}

class NBlazingBenifice(BossModule module) : BlazingBenifice(module, AID.NBlazingBenifice, OID.NArcaneFont);
class SBlazingBenifice(BossModule module) : BlazingBenifice(module, AID.SBlazingBenifice, OID.SArcaneFont);