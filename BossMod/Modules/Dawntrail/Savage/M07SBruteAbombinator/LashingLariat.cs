namespace BossMod.Dawntrail.Savage.M07SBruteAbombinator;

class LashingLariat(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect rect = new(70f, 16f);
    public AOEInstance? AOE;

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(ref AOE);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch (spell.Action.ID)
        {
            case (uint)AID.LashingLariat1:
            case (uint)AID.LashingLariat2:
                AOE = new(rect, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell));
                break;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        switch (spell.Action.ID)
        {
            case (uint)AID.LashingLariat1:
            case (uint)AID.LashingLariat2:
                AOE = null;
                ++NumCasts;
                break;
        }
    }
}
