namespace BossMod.Dawntrail.Savage.M05SDancingGreen;

class DoTheHustle(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCone cone = new(50f, 90f.Degrees());
    private readonly List<AOEInstance> _aoes = new(4);

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => CollectionsMarshal.AsSpan(_aoes);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch (spell.Action.ID)
        {
            case (uint)AID.DoTheHustle1:
            case (uint)AID.DoTheHustle2:
            case (uint)AID.DoTheHustle3:
            case (uint)AID.DoTheHustle4:
                _aoes.Add(new(cone, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell)));
                break;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        var count = _aoes.Count;
        if (count != 0)
            switch (spell.Action.ID)
            {
                case (uint)AID.DoTheHustle1:
                case (uint)AID.DoTheHustle2:
                case (uint)AID.DoTheHustle3:
                case (uint)AID.DoTheHustle4:
                    _aoes.RemoveAt(0);
                    ++NumCasts;
                    break;
            }
    }
}
