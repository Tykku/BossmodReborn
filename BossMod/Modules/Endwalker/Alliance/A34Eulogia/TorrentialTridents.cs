namespace BossMod.Endwalker.Alliance.A34Eulogia;

class TorrentialTrident(BossModule module) : Components.GenericAOEs(module)
{
    public readonly List<AOEInstance> AOEs = new(6);
    private static readonly AOEShapeCircle _shape = new(18);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = AOEs.Count;
        if (count == 0)
            return [];
        var max = count > 5 ? 5 : count;
        List<AOEInstance> aoes = new(max);
        for (var i = 0; i < max; ++i)
        {
            var aoe = AOEs[i];
            if (i == 0)
                aoes.Add(count > 1 ? aoe with { Color = Colors.Danger } : aoe);
            else
                aoes.Add(aoe);
        }
        return aoes;
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.TorrentialTridentLanding:
                AOEs.Add(new(_shape, caster.Position, default, WorldState.FutureTime(13.6f)));
                break;
            case AID.LightningBolt:
                ++NumCasts;
                if (AOEs.Count > 0)
                    AOEs.RemoveAt(0);
                break;
        }
    }
}
