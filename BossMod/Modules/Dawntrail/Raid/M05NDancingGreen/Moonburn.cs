namespace BossMod.Dawntrail.Raid.M05NDancingGreen;

class Moonburn(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect rect = new(40f, 7.5f);
    public readonly List<AOEInstance> AOEs = new(2);

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => CollectionsMarshal.AsSpan(AOEs);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.Moonburn1 or (uint)AID.Moonburn1)
            AOEs.Add(new(rect, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.Moonburn1 or (uint)AID.Moonburn1)
            AOEs.Clear();
    }
}
