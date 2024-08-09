namespace BossMod.Dawntrail.Raid.M01NBlackCat;

class OneTwoPaw(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];
    private enum Pattern { None, WestEast, EastWest }
    private Pattern _currentPattern;
    private static readonly AOEShapeCone cone = new(60, 90.Degrees());
    private static readonly List<Angle> angles = [89.999f.Degrees(), -90.004f.Degrees()];
    private Actor? helper;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_aoes.Count > 0)
            yield return _aoes[0] with { Color = Colors.Danger };
        if (_aoes.Count > 1)
            yield return _aoes[1] with { Risky = false };
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.LeapingAttacks)
        {
            helper = actor;
            InitIfReady();
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.OneTwoPaw1:
            case AID.OneTwoPaw2:
            case AID.OneTwoPaw3:
            case AID.OneTwoPaw4:
                _aoes.Add(new(cone, caster.Position, spell.Rotation, Module.CastFinishAt(spell)));
                _aoes.SortBy(x => x.Activation);
                break;
            case AID.LeapingOneTwoPawVisual1:
            case AID.LeapingOneTwoPawVisual3:
                _currentPattern = Pattern.EastWest;
                break;
            case AID.LeapingOneTwoPawVisual2:
            case AID.LeapingOneTwoPawVisual4:
                _currentPattern = Pattern.WestEast;
                break;
        }
        InitIfReady();
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.LeapingBlackCatCrossingVisual1:
            case AID.LeapingBlackCatCrossingVisual2:
            case AID.LeapingBlackCatCrossingVisual3:
            case AID.LeapingBlackCatCrossingVisual4:
                helper = null;
                break;
        }
        if (_aoes.Count > 0)
            switch ((AID)spell.Action.ID)
            {
                case AID.OneTwoPaw1:
                case AID.OneTwoPaw2:
                case AID.OneTwoPaw3:
                case AID.OneTwoPaw4:
                case AID.LeapingOneTwoPaw1:
                case AID.LeapingOneTwoPaw2:
                case AID.LeapingOneTwoPaw3:
                case AID.LeapingOneTwoPaw4:
                    _currentPattern = Pattern.None;
                    _aoes.RemoveAt(0);
                    break;
            }
    }

    private void InitIfReady()
    {
        if (helper != null && _currentPattern != Pattern.None)
        {
            var angle = _currentPattern == Pattern.EastWest ? angles : angles.AsEnumerable().Reverse().ToList();
            _aoes.Add(new(cone, helper.Position, angle[0], Module.WorldState.FutureTime(8.7f)));
            _aoes.Add(new(cone, helper.Position, angle[1], Module.WorldState.FutureTime(10.7f)));
            _currentPattern = Pattern.None;
            helper = null;
        }
    }
}
