namespace BossMod.Dawntrail.Savage.M05SDancingGreen;

class DoTheHustle(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCone cone = new(50f, 90f.Degrees());
    public readonly List<AOEInstance> AOEs = new(4);

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => CollectionsMarshal.AsSpan(AOEs);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch (spell.Action.ID)
        {
            case (uint)AID.DoTheHustle1:
            case (uint)AID.DoTheHustle2:
            case (uint)AID.DoTheHustle3:
            case (uint)AID.DoTheHustle4:
                AOEs.Add(new(cone, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell)));
                break;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        var count = AOEs.Count;
        if (count != 0)
            switch (spell.Action.ID)
            {
                case (uint)AID.DoTheHustle1:
                case (uint)AID.DoTheHustle2:
                case (uint)AID.DoTheHustle3:
                case (uint)AID.DoTheHustle4:
                    AOEs.RemoveAt(0);
                    ++NumCasts;
                    break;
            }
    }
}
