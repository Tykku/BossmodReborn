namespace BossMod.Dawntrail.Alliance.A14ShadowLord;

class CthonicFury(BossModule module) : Components.GenericAOEs(module)
{
    private AOEInstance? _aoe;
    public bool Active => _aoe != null || Arena.Bounds != A14ShadowLord.DefaultBounds;
    private static readonly Square[] def = [new Square(A14ShadowLord.ArenaCenter, 30)]; // using a square for the difference instead of a circle since less vertices will result in slightly better performance
    public static readonly AOEShapeCustom AOEBurningBattlements = new(def, [new Square(A14ShadowLord.ArenaCenter, 11.5f, 45.Degrees())]);
    private static readonly AOEShapeCustom aoeCthonicFury = new(def, A14ShadowLord.Combined);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.CthonicFuryStart)
            _aoe = new(aoeCthonicFury, Arena.Center, default, Module.CastFinishAt(spell));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.CthonicFuryStart:
                _aoe = null;
                Arena.Bounds = A14ShadowLord.ComplexBounds;
                break;
            case AID.CthonicFuryEnd:
                Arena.Bounds = A14ShadowLord.DefaultBounds;
                break;
        }
    }
}

class BurningCourtMoatKeepBattlements(BossModule module) : Components.GenericAOEs(module)
{
    public readonly List<AOEInstance> AOEs = [];

    private static readonly AOEShape _shapeC = new AOEShapeCircle(8);
    private static readonly AOEShape _shapeM = new AOEShapeDonut(5, 15);
    private static readonly AOEShape _shapeK = new AOEShapeRect(11.5f, 11.5f, 11.5f);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => AOEs;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        var shape = ShapeForAction(spell.Action);
        if (shape != null)
            AOEs.Add(new(shape, caster.Position, spell.Rotation, Module.CastFinishAt(spell)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        var shape = ShapeForAction(spell.Action);
        if (shape != null)
            AOEs.RemoveAll(aoe => aoe.Shape == shape && aoe.Origin == caster.Position);
    }

    private static AOEShape? ShapeForAction(ActionID aid) => (AID)aid.ID switch
    {
        AID.BurningCourt => _shapeC,
        AID.BurningMoat => _shapeM,
        AID.BurningKeep => _shapeK,
        AID.BurningBattlements => CthonicFury.AOEBurningBattlements,
        _ => null
    };
}

class EchoesOfAgony(BossModule module) : Components.StackWithIcon(module, (uint)IconID.EchoesOfAgony, ActionID.MakeSpell(AID.EchoesOfAgonyAOE), 5, 9.2f, PartyState.MaxAllianceSize, PartyState.MaxAllianceSize)
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.EchoesOfAgony)
            NumFinishedStacks = 0;
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action == StackAction)
        {
            if (++NumFinishedStacks >= 5)
            {
                Stacks.Clear();
            }
        }
    }
}
