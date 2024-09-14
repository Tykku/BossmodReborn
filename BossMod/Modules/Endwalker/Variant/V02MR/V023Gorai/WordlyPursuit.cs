namespace BossMod.Endwalker.VariantCriterion.V02MR.V023Gorai;

class WorldlyPursuit(BossModule module) : Components.GenericRotatingAOE(module)
{
    private static readonly AOEShapeCross cross = new(60, 10);
    public override uint ImminentColor => Colors.AOE;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.WorldlyPursuitFirstCCW:
                Sequences.Add(new(cross, Module.PrimaryActor.Position, spell.Rotation, 22.5f.Degrees(), Module.CastFinishAt(spell), 3.7f, 5, 1));
                break;
            case AID.WorldlyPursuitFirstCW:
                Sequences.Add(new(cross, Module.PrimaryActor.Position, spell.Rotation, -22.5f.Degrees(), Module.CastFinishAt(spell), 3.7f, 5, 1));
                break;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.WorldlyPursuitFirstCCW or AID.WorldlyPursuitFirstCW or AID.WorldlyPursuitRest)
            AdvanceSequence(0, WorldState.CurrentTime);
    }
}
