namespace BossMod.Endwalker.VariantCriterion.V02MR.V021Yozakura;

class LevinblossomLance(BossModule module) : Components.GenericRotatingAOE(module)
{
    private static readonly AOEShapeRect rect = new(30, 3.5f, 30);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.LevinblossomLanceCCW:
                Sequences.Add(new(rect, spell.LocXZ, spell.Rotation, 28.Degrees(), Module.CastFinishAt(spell, 0.8f), 1, 5, 2));
                break;
            case AID.LevinblossomLanceCW:
                Sequences.Add(new(rect, spell.LocXZ, spell.Rotation, -28.Degrees(), Module.CastFinishAt(spell, 0.8f), 1, 5, 2));
                break;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.LevinblossomLanceFirst or AID.LevinblossomLanceRest)
            AdvanceSequence(0, WorldState.CurrentTime);
    }
}
