namespace BossMod.Dawntrail.Raid.M04NWickedThunder;

public class WickedCannon(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = [];
    private static readonly AOEShapeRect rect = new(40, 5);
    private static readonly float[] delays3fold = [9.5f, 8.3f, 7.7f];
    private static readonly float[] delays4fold = [12, 10.9f, 10.2f, 9.5f];
    private static readonly float[] delays5fold = [14.5f, 13.4f, 12.8f, 12.2f, 11.6f];
    private float[] currentDelays = [];
    private static readonly HashSet<AID> castEnd = [AID.WickedCannon1, AID.WickedCannon2, AID.WickedCannon3, AID.WickedCannon4, AID.WickedCannon5];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_aoes.Count > 0)
            yield return _aoes[0] with { Color = Colors.Danger };
        if (_aoes.Count > 1)
            yield return _aoes[1] with { Risky = false };
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        currentDelays = (AID)spell.Action.ID switch
        {
            AID.ThreefoldBlast1 or AID.ThreefoldBlast2 => delays3fold,
            AID.FourfoldBlast1 or AID.FourfoldBlast2 => delays4fold,
            AID.FivefoldBlast1 or AID.FivefoldBlast2 => delays5fold,
            _ => []
        };
    }

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if (status.ID == (uint)SID.WickedCannon && currentDelays.Length > _aoes.Count)
            AddAOE(status);
    }

    private void AddAOE(ActorStatus status)
    {
        var rotation = status.Extra switch
        {
            0x2D3 => 180.Degrees(),
            0x2D4 => default,
            _ => default
        };
        _aoes.Add(new(rect, Module.PrimaryActor.Position, rotation, WorldState.FutureTime(currentDelays[_aoes.Count])));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (_aoes.Count > 0 && castEnd.Contains((AID)spell.Action.ID))
            _aoes.RemoveAt(0);
    }
}
