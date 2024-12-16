namespace BossMod.Dawntrail.Alliance.A11Prishe;

class AsuranFists(BossModule module) : Components.GenericTowers(module)
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.AsuranFistsVisual)
        {
            Towers.Add(new(spell.LocXZ, 6, PartyState.MaxAllianceSize, PartyState.MaxAllianceSize, activation: Module.CastFinishAt(spell, 0.5f)));
        }
    }

    public override void OnActorEAnim(Actor actor, uint state)
    {
        if (Towers.Count != 0 && (OID)actor.OID == OID.Tower && state == 0x00100020)
        {
            Towers[0] = Towers[0] with { Position = actor.Position }; // spell position can be about one or two pixels off the real tower position...
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.AsuranFists1 or AID.AsuranFists2 or AID.AsuranFists3)
        {
            ++NumCasts;
            if ((AID)spell.Action.ID == AID.AsuranFists3)
                Towers.Clear();
        }
    }
}
