namespace BossMod.Endwalker.Trial.T08Asura;

class SixBladedKhadga(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<ActorCastInfo> _spell = [];
    private DateTime _start;
    private static readonly AOEShapeCone cone = new(20, 90.Degrees());
    private static readonly HashSet<AID> castEnd = [AID.Khadga1, AID.Khadga2, AID.Khadga3, AID.Khadga4, AID.Khadga5, AID.Khadga6];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_spell.Count > 0)
            yield return new(cone, Module.PrimaryActor.Position, _spell[0].Rotation, _start.AddSeconds(NumCasts * 2), Colors.Danger);
        if (_spell.Count > 1)
            yield return new(cone, Module.PrimaryActor.Position, _spell[1].Rotation, _start.AddSeconds(2 + NumCasts * 2), Risky: !_spell[1].Rotation.AlmostEqual(_spell[0].Rotation + 180.Degrees(), Angle.DegToRad));
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.KhadgaTelegraph1 or AID.KhadgaTelegraph2 or AID.KhadgaTelegraph3)
        {
            _spell.Add(spell);
            if (_start == default)
                _start = WorldState.FutureTime(12.9f);
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (castEnd.Contains((AID)spell.Action.ID))
        {
            _spell.RemoveAt(0);
            _start.AddSeconds(2);
            if (++NumCasts == 6)
            {
                NumCasts = 0;
                _start = default;
            }
        }
    }
}
