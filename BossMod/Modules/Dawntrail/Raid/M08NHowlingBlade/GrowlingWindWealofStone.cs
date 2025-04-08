namespace BossMod.Dawntrail.Raid.M08NHowlingBlade;

class GrowlingWindWealofStone(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = new(4);
    private static readonly AOEShapeRect rect = new(40f, 3f);

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => CollectionsMarshal.AsSpan(_aoes);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch (spell.Action.ID)
        {
            case (uint)AID.GrowlingWind:
            case (uint)AID.WealOfStone1:
            case (uint)AID.WealOfStone2:
            case (uint)AID.WealOfStone3:
                _aoes.Add(new(rect, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell)));
                break;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        switch (spell.Action.ID)
        {
            case (uint)AID.GrowlingWind:
            case (uint)AID.WealOfStone1:
            case (uint)AID.WealOfStone2:
            case (uint)AID.WealOfStone3:
                _aoes.Clear();
                break;
        }
    }
}
