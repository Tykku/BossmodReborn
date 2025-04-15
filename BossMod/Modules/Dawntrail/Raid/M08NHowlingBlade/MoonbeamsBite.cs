namespace BossMod.Dawntrail.Raid.M08NHowlingBlade;

class MoonbeamsBite(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = new(4);
    private static readonly AOEShapeRect rect = new(40f, 10f);

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = _aoes.Count;
        if (count == 0)
            return [];
        var aoes = CollectionsMarshal.AsSpan(_aoes);
        var max = count > 2 ? 2 : count;
        if (max > 1)
            aoes[0].Color = Colors.Danger;
        return aoes[..max];
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch (spell.Action.ID)
        {
            case (uint)AID.MoonbeamsBite1:
            case (uint)AID.MoonbeamsBite2:
            case (uint)AID.MoonbeamsBite3:
            case (uint)AID.MoonbeamsBite4:
                _aoes.Add(new(rect, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell)));
                break;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (_aoes.Count != 0)
            switch (spell.Action.ID)
            {
                case (uint)AID.MoonbeamsBite1:
                case (uint)AID.MoonbeamsBite2:
                case (uint)AID.MoonbeamsBite3:
                case (uint)AID.MoonbeamsBite4:
                    _aoes.RemoveAt(0);
                    break;
            }
    }
}
