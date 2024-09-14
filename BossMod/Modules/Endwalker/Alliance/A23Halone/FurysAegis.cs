namespace BossMod.Endwalker.Alliance.A23Halone;

class FurysAegis(BossModule module) : Components.CastCounter(module, default)
{
    private static readonly HashSet<AID> castEnd = [AID.Shockwave, AID.FurysAegisAOE1, AID.FurysAegisAOE2, AID.FurysAegisAOE3, AID.FurysAegisAOE4, AID.FurysAegisAOE5, AID.FurysAegisAOE6];

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (castEnd.Contains((AID)spell.Action.ID))
            ++NumCasts;
    }
}
