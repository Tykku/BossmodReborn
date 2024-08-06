namespace BossMod.Dawntrail.Raid.M01NBlackCat;

public class BlackCatCrossing(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];
    private static readonly Angle[] anglesIntercardinals = [-45.003f.Degrees(), 44.998f.Degrees(), 134.999f.Degrees(), -135.005f.Degrees()];
    private static readonly Angle[] anglesCardinals = [-90.004f.Degrees(), -0.003f.Degrees(), 180.Degrees(), 89.999f.Degrees()];
    private static readonly AOEShapeCone cone = new(60, 22.5f.Degrees());
    private enum Pattern { None, Cardinals, Intercardinals }
    private Pattern _currentPattern;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_aoes.Count > 3)
            for (var i = 0; i < 4; i++)
                yield return _aoes[i] with { Color = Colors.Danger };
        if (_aoes.Count > 7)
            for (var i = 4; i < 8; i++)
                yield return _aoes[i] with { Risky = false };
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.BlackCatCrossingFirst:
            case AID.BlackCatCrossingRest:
                _aoes.Add(new(cone, caster.Position, spell.Rotation, Module.CastFinishAt(spell)));
                _aoes.SortBy(x => x.Activation);
                break;
        }
    }

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if (status.Extra != 0x307 && _currentPattern == Pattern.None)
            switch ((SID)status.ID)
            {
                case SID.BlackCatCrossing1:
                    _currentPattern = Pattern.Cardinals;
                    break;
                case SID.BlackCatCrossing2:
                    _currentPattern = Pattern.Intercardinals;
                    break;
            }
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.LeapingAttacks && _currentPattern != Pattern.None)
        {
            AddAOEs(actor, _currentPattern == Pattern.Cardinals ? anglesCardinals : anglesIntercardinals, 9);
            AddAOEs(actor, _currentPattern == Pattern.Cardinals ? anglesIntercardinals : anglesCardinals, 11);
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (_aoes.Count > 0)
            switch ((AID)spell.Action.ID)
            {
                case AID.BlackCatCrossingFirst:
                case AID.BlackCatCrossingRest:
                case AID.LeapingBlackCatCrossingFirst:
                case AID.LeapingBlackCatCrossingRest:
                    _aoes.RemoveAt(0);
                    _currentPattern = Pattern.None;
                    break;
            }
    }

    private void AddAOEs(Actor actor, Angle[] angles, int futureTime)
    {
        foreach (var angle in angles)
            _aoes.Add(new(cone, actor.Position, angle, Module.WorldState.FutureTime(futureTime)));
    }
}
