namespace BossMod.Dawntrail.Savage.M07SBruteAbombinator;

class AbominableBlink(BossModule module) : Components.BaitAwayIcon(module, new AOEShapeCircle(24f), (uint)IconID.AbominableBlink, ActionID.MakeSpell(AID.AbominableBlink), 6.4f, true)
{
    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        base.AddAIHints(slot, actor, assignment, hints);
        var baits = ActiveBaitsOn(actor);
        if (baits.Count != 0)
            hints.AddForbiddenZone(ShapeDistance.Rect(Arena.Center, new WDir(default, 1f), 24f, 24f, 25f), baits[0].Activation.AddSeconds(1d));
    }
}
