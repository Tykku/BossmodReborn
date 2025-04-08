namespace BossMod.Endwalker.Alliance.A22AlthykNymeia;

class Petrai(BossModule module) : Components.GenericSharedTankbuster(module, ActionID.MakeSpell(AID.PetraiAOE), 6f)
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == (uint)AID.Petrai)
        {
            Source = caster;
            Target = WorldState.Actors.Find(spell.TargetID);
            Activation = Module.CastFinishAt(spell, 1f);
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action == WatchedAction)
        {
            ++NumCasts;
            Source = Target = null;
        }
    }
}
