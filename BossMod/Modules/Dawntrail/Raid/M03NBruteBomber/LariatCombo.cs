namespace BossMod.Dawntrail.Raid.M03NBruteBomber;

class LariatCombo(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];

    private static readonly AOEShapeRect rect1 = new(20, 30, 5, -90.Degrees());
    private static readonly AOEShapeRect rect2 = new(20, 30, 5, 90.Degrees());

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes.Take(1);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        var rotation = spell.Rotation;
        var position = caster.Position;
        switch ((AID)spell.Action.ID)
        {
            case AID.LariatComboVisual1:
                AddAOEs(rect2, rect1, position, rotation, spell);
                break;
            case AID.LariatComboVisual2:
                AddAOEs(rect2, rect2, position, rotation, spell);
                break;
            case AID.LariatComboVisual3:
                AddAOEs(rect1, rect2, position, rotation, spell);
                break;
            case AID.LariatComboVisual4:
                AddAOEs(rect1, rect1, position, rotation, spell);
                break;
        }
    }

    private void AddAOEs(AOEShapeRect shape1, AOEShapeRect shape2, WPos position, Angle rotation, ActorCastInfo spell)
    {
        _aoes.Add(new(shape1, position, rotation, Module.CastFinishAt(spell)));
        _aoes.Add(new(shape2, position, rotation, Module.CastFinishAt(spell, 4.4f)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (_aoes.Count > 0 && (AID)spell.Action.ID is AID.LariatCombo1 or AID.LariatCombo2 or AID.LariatCombo3 or AID.LariatCombo4)
            _aoes.RemoveAt(0);
    }
}
