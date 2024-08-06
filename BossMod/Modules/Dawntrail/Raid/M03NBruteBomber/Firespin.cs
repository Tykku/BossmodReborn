namespace BossMod.Dawntrail.Raid.M03NBruteBomber;

class FireSpin(BossModule module) : Components.GenericRotatingAOE(module)
{
    private static readonly AOEShapeCone cone = new(40, 30.Degrees());

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.FireSpinCCW:
            case AID.InfernalSpinCCW:
                Sequences.Add(new(cone, Module.PrimaryActor.Position, spell.Rotation, 45.Degrees(), Module.CastFinishAt(spell, 0.5f), 1, 8));
                break;
            case AID.FireSpinCW:
            case AID.InfernalSpinCW:
                Sequences.Add(new(cone, Module.PrimaryActor.Position, spell.Rotation, -45.Degrees(), Module.CastFinishAt(spell, 0.5f), 1, 8));
                break;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.FireSpinFirst or AID.FireSpinRest or AID.InfernalSpinFirst or AID.InfernalSpinRest)
            AdvanceSequence(0, WorldState.CurrentTime);
    }
}
