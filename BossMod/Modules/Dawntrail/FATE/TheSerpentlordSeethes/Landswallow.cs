namespace BossMod.Dawntrail.FATE.Ttokrrone;

class Landswallow(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect rect1 = new(38, 13.5f);
    private static readonly AOEShapeRect rect2 = new(50, 13.5f);
    private static readonly AOEShapeRect rect3 = new(68, 13.5f);
    private static readonly AOEShapeRect rect4 = new(63, 13.5f);
    private readonly List<AOEInstance> _aoes = [];
    private static readonly Dictionary<(Angle, Angle), List<(WPos, Angle, AOEShape)>> chargeConfigs = new()
    {
        { (0.Degrees(), 135.Degrees()), new List<(WPos, Angle, AOEShape)> {
            (new(52.995f, -820.005f), -0.003f.Degrees(), rect1),
            (new(52.995f, -790.524f), 157.5f.Degrees(), rect4),
            (new(73.747f, -840.75f), -90.004f.Degrees(), rect2),
            (new(32.242f, -840.757f), 67.498f.Degrees(), rect4),
            (new(82.475f, -820.005f), -67.504f.Degrees(), rect4),
            (new(32.242f, -799.252f), 134.999f.Degrees(), rect3)
        }},
        { (0.Degrees(), -135.Degrees()), new List<(WPos, Angle, AOEShape)> {
            (new(52.995f, -820.005f), -0.003f.Degrees(), rect1),
            (new(52.995f, -790.524f), -180.Degrees(), rect3),
            (new(52.995f, -849.516f), -45.003f.Degrees(), rect2),
            (new(23.484f, -820.005f), 112.449f.Degrees(), rect4),
            (new(73.747f, -840.757f), -0.003f.Degrees(), rect2),
            (new(73.747f, -799.252f), -135.005f.Degrees(), rect3)
        }},
        { (-90.Degrees(), -135.Degrees()), new List<(WPos, Angle, AOEShape)> {
            (new(52.995f, -820.005f), -90.004f.Degrees(), rect1),
            (new(23.484f, -820.005f), 134.999f.Degrees(), rect2),
            (new(52.995f, -849.516f), -0.003f.Degrees(), rect3),
            (new(52.995f, -790.524f), 157.5f.Degrees(), rect4),
            (new(73.747f, -840.757f), -0.003f.Degrees(), rect2),
            (new(73.747f, -799.252f), -135.005f.Degrees(), rect3)
        }},
        { (-90.Degrees(), 135.Degrees()), new List<(WPos, Angle, AOEShape)> {
            (new(52.995f, -820.005f), -90.004f.Degrees(), rect1),
            (new(23.484f, -820.005f), 112.499f.Degrees(), rect4),
            (new(73.747f, -840.757f), -90.004f.Degrees(), rect2),
            (new(32.242f, -840.757f), 67.498f.Degrees(), rect4),
            (new(82.475f, -820.005f), -67.504f.Degrees(), rect4),
            (new(32.242f, -799.252f), 134.999f.Degrees(), rect3)
        }},
        { (90.Degrees(), -45.Degrees()), new List<(WPos, Angle, AOEShape)> {
            (new(52.995f, -820.005f), 89.999f.Degrees(), rect1),
            (new(82.475f, -820.005f), -89.982f.Degrees(), rect3),
            (new(23.484f, -820.005f), 44.998f.Degrees(), rect2),
            (new(52.995f, -790.524f), -157.505f.Degrees(), rect4),
            (new(32.242f, -840.757f), 89.999f.Degrees(), rect2),
            (new(73.747f, -840.757f), -45.003f.Degrees(), rect3)
        }},
        { (90.Degrees(), 45.Degrees()), new List<(WPos, Angle, AOEShape)> {
            (new(52.995f, -820.005f), 89.999f.Degrees(), rect1),
            (new(82.475f, -820.005f), -45.003f.Degrees(), rect2),
            (new(52.995f, -790.524f), -180.Degrees(), rect3),
            (new(52.995f, -849.516f), -22.503f.Degrees(), rect4),
            (new(32.242f, -799.252f), -180.Degrees(), rect2),
            (new(32.242f, -840.757f), 44.998f.Degrees(), rect3)
        }},
        { (180.Degrees(), -45.Degrees()), new List<(WPos, Angle, AOEShape)> {
            (new(52.995f, -820.005f), 180.Degrees(), rect1),
            (new(52.995f, -849.516f), -22.503f.Degrees(), rect4),
            (new(32.242f, -799.252f), 89.999f.Degrees(), rect2),
            (new(73.747f, -799.252f), -112.504f.Degrees(), rect4),
            (new(23.484f, -820.005f), 112.499f.Degrees(), rect4),
            (new(73.747f, -840.757f), -45.003f.Degrees(), rect3)
        }},
        { (180.Degrees(), 45.Degrees()), new List<(WPos, Angle, AOEShape)> {
            (new(52.995f, -820.005f), 179.995f.Degrees(), rect1),
            (new(52.995f, -849.516f), 22.498f.Degrees(), rect4),
            (new(73.747f, -799.252f), 180.Degrees(), rect2),
            (new(73.747f, -840.757f), -22.503f.Degrees(), rect4),
            (new(52.995f, -790.524f), -157.505f.Degrees(), rect4),
            (new(32.242f, -840.757f), 44.998f.Degrees(), rect3)
        }}
    };

    private Angle startRotation;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_aoes.Count > 0)
        {
            yield return _aoes[0] with { Color = Colors.Danger };
            foreach (var a in _aoes.Skip(1).Take(_aoes.Count - 1))
                yield return a;
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.Landswallow1)
            startRotation = spell.Rotation;
        else if ((AID)spell.Action.ID == AID.LandSwallowTelegraph6)
        {
            var maxError = Helpers.RadianConversion;
            foreach (var config in chargeConfigs)
            {
                if (startRotation.AlmostEqual(config.Key.Item1, maxError) && spell.Rotation.AlmostEqual(config.Key.Item2, maxError))
                {
                    AddAOEs(config.Value, spell);
                    break;
                }
            }
        }
    }

    private void AddAOEs(List<(WPos, Angle, AOEShape)> list, ActorCastInfo spell)
    {
        foreach (var c in list)
            _aoes.Add(new(c.Item3, c.Item1, c.Item2, Module.CastFinishAt(spell, 1.3f * _aoes.Count)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (_aoes.Count == 0)
            return;
        switch ((AID)spell.Action.ID)
        {
            case AID.Landswallow1:
            case AID.Landswallow2:
            case AID.Landswallow3:
            case AID.Landswallow4:
                _aoes.RemoveAt(0);
                break;
        }
    }
}
