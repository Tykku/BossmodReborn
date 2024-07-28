namespace BossMod.Dawntrail.FATE.Ttokrrone;

class Sandspout(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCone cone = new(60, 45.Degrees());
    private static readonly AOEShapeCircle circle = new(13);
    private static readonly Angle a180 = 180.Degrees();
    private static readonly Angle a90 = 90.Degrees();
    private static readonly Angle a0 = 0.Degrees();
    private readonly List<AOEInstance> _aoes = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.TailwardSandspoutVisual:
                AddAOEs(a180, spell);
                break;
            case AID.LeftwardSandspoutVisual:
                AddAOEs(a90, spell);
                break;
            case AID.RightwardSandspoutVisual:
                AddAOEs(-a90, spell);
                break;
            case AID.FangwardSandspoutVisual:
                AddAOEs(a0, spell);
                break;
        }
    }

    private void AddAOEs(Angle offset, ActorCastInfo spell)
    {
        var position = Module.PrimaryActor.Position;
        _aoes.Add(new(cone, position, spell.Rotation + offset, Module.CastFinishAt(spell, 6.1f)));
        _aoes.Add(new(circle, position, default, Module.CastFinishAt(spell, 6.1f)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.TailwardSandspout:
            case AID.RightwardSandspout:
            case AID.LeftwardSandspout:
            case AID.FangwardSandspout:
                _aoes.Clear();
                break;
        }
    }
}
