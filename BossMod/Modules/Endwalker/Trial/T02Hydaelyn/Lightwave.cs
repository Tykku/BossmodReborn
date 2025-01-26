namespace BossMod.Endwalker.Trial.T02Hydaelyn;

class Lightwave(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<Actor> waves = new(4);
    private static readonly AOEShapeRect rect = new(16, 8, 12);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = waves.Count;
        if (waves.Count == 0)
            return [];
        var aoes = new AOEInstance[count];
        for (var i = 0; i < count; ++i)
        {
            var w = waves[i];
            aoes[i] = new(rect, w.Position, w.Rotation);
        }
        return aoes;
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.RayOfLight && !waves.Contains(caster))
            waves.Add(caster);
    }
}
