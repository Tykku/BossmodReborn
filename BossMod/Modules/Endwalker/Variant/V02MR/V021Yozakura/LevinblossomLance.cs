namespace BossMod.Endwalker.Variant.V02MR.V021Yozakura;

class LevinblossomLance(BossModule module) : Components.GenericRotatingAOE(module)
{
    private Angle _increment;
    private Angle _rotation;
    private DateTime _activation;

    private static readonly AOEShapeRect _shape = new(60, 3.5f, 60);

    public override void OnEventIcon(Actor actor, uint iconID)
    {
        var increment = (IconID)iconID switch
        {
            IconID.RotateCW => -22.5f.Degrees(),
            IconID.RotateCCW => 22.5f.Degrees(),
            _ => default
        };
        if (increment != default)
        {
            _increment = increment;
            InitIfReady(actor);
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.LevinblossomLanceFirst)
        {
            _rotation = spell.Rotation;
            _activation = Module.CastFinishAt(spell);
        }
        if (_rotation != default)
            InitIfReady(caster);
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.LevinblossomLanceFirst or AID.LevinblossomLanceRest)
            AdvanceSequence(0, WorldState.CurrentTime);
    }

    private void InitIfReady(Actor source)
    {
        if (_rotation != default && _increment != default)
        {
            Sequences.Add(new(_shape, source.Position, _rotation, _increment, _activation, 1, 5));
            _rotation = default;
            _increment = default;
        }
    }
}
