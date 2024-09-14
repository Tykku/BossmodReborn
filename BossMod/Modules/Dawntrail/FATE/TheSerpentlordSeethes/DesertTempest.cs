namespace BossMod.Dawntrail.FATE.Ttokrrone;

class DesertTempest(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly Angle aM90 = -90.004f.Degrees();
    private static readonly Angle a90 = 89.999f.Degrees();
    private static readonly AOEShapeCone cone = new(19, 90.Degrees());
    private static readonly AOEShapeCircle circle = new(19);
    private static readonly AOEShapeDonut donut = new(14, 60);
    private static readonly AOEShapeDonutSector donutSector = new(14, 60, 90.Degrees());
    private static readonly HashSet<AID> castEnd = [AID.DesertTempestCircle, AID.DesertTempestDonut,
    AID.DesertTempestDonutSegment1, AID.DesertTempestDonutSegment2, AID.DesertTempestCone1, AID.DesertTempestCone2];

    private readonly List<AOEInstance> _aoes = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.DesertTempestVisualDonut:
                AddAOEs(donut, null, spell);
                break;
            case AID.DesertTempestVisualCircle:
                AddAOEs(circle, null, spell);
                break;
            case AID.DesertTempestVisualConeDonutSegment:
                AddAOEs(cone, donutSector, spell);
                break;
            case AID.DesertTempestVisualDonutSegmentCone:
                AddAOEs(donutSector, cone, spell);
                break;
        }
    }

    private void AddAOEs(AOEShape first, AOEShape? second, ActorCastInfo spell)
    {
        var position = Module.PrimaryActor.Position;
        _aoes.Add(new(first, position, spell.Rotation + a90, Module.CastFinishAt(spell, 1)));
        if (second != null)
            _aoes.Add(new(second, position, spell.Rotation + aM90, Module.CastFinishAt(spell, 1)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (castEnd.Contains((AID)spell.Action.ID))
            _aoes.Clear();
    }
}