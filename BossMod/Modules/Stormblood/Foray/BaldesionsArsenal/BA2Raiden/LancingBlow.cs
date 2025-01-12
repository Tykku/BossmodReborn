namespace BossMod.Stormblood.Foray.BaldesionArsenal.BA2Raiden;

class LancingBlowSpread(BossModule module) : Components.SpreadFromIcon(module, (uint)IconID.Spreadmarker, ActionID.MakeSpell(AID.LancingBlow), 10, 6)
{
    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.StreakLightning)
            Spreads.Clear();
    }
}

class LancingBlowAOE(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCircle circle = new(10);
    public readonly List<AOEInstance> AOEs = new(6);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => AOEs;

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.StreakLightning)
            AOEs.Add(new(circle, actor.Position, default, WorldState.FutureTime(1)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.LancingBlow)
        {
            ++NumCasts;
            AOEs.Clear();
        }
    }
}
