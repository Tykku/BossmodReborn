namespace BossMod.Dawntrail.Alliance.A11Prishe;

abstract class KnuckleSandwich(BossModule module, AID aid1, AID aid2, AOEShape shape1, AOEShape shape2) : Components.ConcentricAOEs(module, [shape1, shape2])
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == aid1)
            AddSequence(spell.LocXZ, Module.CastFinishAt(spell));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (Sequences.Count != 0)
        {
            var order = -1;
            if ((AID)spell.Action.ID == aid1)
                order = 0;
            else if ((AID)spell.Action.ID == aid2)
                order = 1;
            AdvanceSequence(order, spell.LocXZ, WorldState.FutureTime(1.4f));
        }
    }
}
class KnuckleSandwich1(BossModule module) : KnuckleSandwich(module, AID.KnuckleSandwich1, AID.BrittleImpact1, new AOEShapeCircle(9), new AOEShapeDonut(9, 60));
class KnuckleSandwich2(BossModule module) : KnuckleSandwich(module, AID.KnuckleSandwich2, AID.BrittleImpact2, new AOEShapeCircle(18), new AOEShapeDonut(18, 60));
class KnuckleSandwich3(BossModule module) : KnuckleSandwich(module, AID.KnuckleSandwich3, AID.BrittleImpact3, new AOEShapeCircle(27), new AOEShapeDonut(27, 60));
