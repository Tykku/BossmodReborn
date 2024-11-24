namespace BossMod.Dawntrail.Alliance.A11Prishe;

class AsuranFists(BossModule module) : Components.GenericTowers(module)
{
    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.Tower)
            Towers.Add(new(actor.Position, 6, PartyState.MaxAllianceSize, PartyState.MaxAllianceSize, activation: WorldState.FutureTime(6.5f)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.AsuranFists1 or AID.AsuranFists2 or AID.AsuranFists3)
        {
            if (++NumCasts == 8)
            {
                NumCasts = 0;
                Towers.Clear();
            }
        }
    }
}
