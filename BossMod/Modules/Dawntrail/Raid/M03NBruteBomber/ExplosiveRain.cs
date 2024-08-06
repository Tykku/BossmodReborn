namespace BossMod.Dawntrail.Raid.M03NBruteBomber;

class ExplosiveRainConcentric(BossModule module) : Components.ConcentricAOEs(module, _shapes)
{
    private static readonly AOEShape[] _shapes = [new AOEShapeCircle(8), new AOEShapeDonut(8, 16), new AOEShapeDonut(16, 24)];

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.ExplosiveRain1)
            AddSequence(Module.Center, Module.CastFinishAt(spell));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (Sequences.Count > 0)
        {
            var order = (AID)spell.Action.ID switch
            {
                AID.ExplosiveRain1 => 0,
                AID.ExplosiveRain2 => 1,
                AID.ExplosiveRain3 => 2,
                _ => -1
            };
            AdvanceSequence(order, Module.Center, WorldState.FutureTime(2));
        }
    }
}

class ExplosiveRainCircle(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.ExplosiveRain4), new AOEShapeCircle(6));
