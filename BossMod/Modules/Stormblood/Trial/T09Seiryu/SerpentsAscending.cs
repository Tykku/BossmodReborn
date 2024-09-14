namespace BossMod.Stormblood.Trial.T09Seiryu;

class SerpentAscending(BossModule module) : Components.GenericTowers(module)
{
    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.Towers)
            Towers.Add(new(actor.Position, 3, activation: WorldState.FutureTime(7.8f)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.SerpentsFang or AID.SerpentsJaws)
            Towers.Clear();
    }
}
