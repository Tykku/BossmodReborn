namespace BossMod.Dawntrail.Alliance.A12Fafnir;

abstract class HurricaneWing(BossModule module, AID aid1, AID aid2, AID aid3, AID aid4, AOEShape shape1, AOEShape shape2, AOEShape shape3, AOEShape shape4)
: Components.ConcentricAOEs(module, [shape1, shape2, shape3, shape4])
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
            else if ((AID)spell.Action.ID == aid3)
                order = 2;
            else if ((AID)spell.Action.ID == aid4)
                order = 3;
            AdvanceSequence(order, spell.LocXZ, WorldState.FutureTime(2));
        }
    }
}
abstract class HurricaneWingOutIn(BossModule module, AID aid1, AID aid2, AID aid3, AID aid4) : HurricaneWing(module, aid1, aid2, aid3, aid4,
new AOEShapeCircle(9), new AOEShapeDonut(9, 16), new AOEShapeDonut(16, 23), new AOEShapeDonut(23, 30));
abstract class HurricaneWingInOut(BossModule module, AID aid1, AID aid2, AID aid3, AID aid4) : HurricaneWing(module, aid1, aid2, aid3, aid4,
new AOEShapeDonut(23, 30), new AOEShapeDonut(16, 23), new AOEShapeDonut(9, 16), new AOEShapeCircle(9));
class HurricaneWingOutInLong(BossModule module) : HurricaneWingOutIn(module, AID.HurricaneWingConcentricA1, AID.HurricaneWingConcentricA2,
AID.HurricaneWingConcentricA3, AID.HurricaneWingConcentricA4);
class HurricaneWingOutInShort(BossModule module) : HurricaneWingOutIn(module, AID.HurricaneWingConcentricB1, AID.HurricaneWingConcentricB2,
AID.HurricaneWingConcentricB3, AID.HurricaneWingConcentricB4);
class HurricaneWingInOutLong(BossModule module) : HurricaneWingInOut(module, AID.HurricaneWingConcentricD1, AID.HurricaneWingConcentricD2,
AID.HurricaneWingConcentricD3, AID.HurricaneWingConcentricD4);
class HurricaneWingInOutShort(BossModule module) : HurricaneWingInOut(module, AID.HurricaneWingConcentricC1, AID.HurricaneWingConcentricC2,
AID.HurricaneWingConcentricC3, AID.HurricaneWingConcentricC4);
