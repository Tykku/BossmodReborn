namespace BossMod.Shadowbringers.Foray.CastrumLacusLitore.CLL3Adrammelech;

class BlizzardIVFireIV(BossModule module) : Components.StayMove(module, 3f)
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        var req = spell.Action.ID switch
        {
            (uint)AID.BlizzardIV1 or (uint)AID.BlizzardIV3 => Requirement.Move,
            (uint)AID.FireIV1 or (uint)AID.FireIV3 => Requirement.Stay,
            _ => Requirement.None
        };
        if (req != Requirement.None)
        {
            Array.Fill(PlayerStates, new(req, Module.CastFinishAt(spell)));
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.BlizzardIV1 or (uint)AID.BlizzardIV3)
        {
            Array.Fill(PlayerStates, default);
        }
    }

    public override void OnStatusLose(Actor actor, ActorStatus status)
    {
        if (status.ID == (uint)SID.Pyretic && Raid.FindSlot(actor.InstanceID) is var slot && slot >= 0)
            PlayerStates[slot] = default;
    }
}
