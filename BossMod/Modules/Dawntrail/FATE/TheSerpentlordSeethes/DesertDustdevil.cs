namespace BossMod.Dawntrail.FATE.Ttokrrone;

class DesertDustdevil(BossModule module) : Components.GenericRotatingAOE(module)
{
    private static readonly AOEShapeCone cone = new(60, 45.Degrees());
    private static readonly Angle offset = 180.Degrees();
    private static readonly Angle a90 = 90.Degrees();

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.FangwardDustdevilVisualCW:
            case AID.FangwardDustdevilVisualCCW:
            case AID.TailwardDustdevilVisualCW:
            case AID.TailwardDustdevilVisualCCW:
                AddSequence(spell, Sequences.Count == 0 ? 7 : 4);
                break;
        }
    }

    private void AddSequence(ActorCastInfo spell, int repeats)
    {
        var rotation = spell.Rotation;
        var direction = a90;

        if ((AID)spell.Action.ID is AID.FangwardDustdevilVisualCW or AID.TailwardDustdevilVisualCW)
            direction = -a90;
        if ((AID)spell.Action.ID is AID.TailwardDustdevilVisualCW or AID.TailwardDustdevilVisualCCW)
            rotation += offset;
        if (Sequences.Count != 0)
            Sequences.Clear();
        Sequences.Add(new(cone, Module.PrimaryActor.Position, rotation, direction, Module.CastFinishAt(spell, 1), 2.6f, repeats));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.TailwardDustdevilFirst:
            case AID.FangwardDustdevilFirst:
            case AID.RightwardSandspoutDDRest:
            case AID.LeftwardSandspoutDDRest:
                AdvanceSequence(0, WorldState.CurrentTime);
                break;
        }
    }
}

class DustcloakDustdevil(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCircle circle = new(13);
    private AOEInstance? _aoe;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);

    public override void Update()
    {
        var component = Module.FindComponent<DesertDustdevil>()!.Sequences.Count == 0;
        if (_aoe == null && !component)
            _aoe = new(circle, Module.PrimaryActor.Position, default, WorldState.FutureTime(8.1f));
        else if (_aoe != null && component)
            _aoe = null;
    }
}
