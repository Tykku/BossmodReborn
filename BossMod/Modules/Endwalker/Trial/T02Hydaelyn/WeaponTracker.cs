namespace BossMod.Endwalker.Trial.T02Hydaelyn;

class WeaponTracker(BossModule module) : Components.GenericAOEs(module)
{
    private AOEInstance? _aoe;
    private static readonly AOEShapeDonut donut = new(5, 40);
    private static readonly AOEShapeCircle circle = new(10);
    private static readonly AOEShapeCross cross = new(40, 5);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if ((SID)status.ID == SID.HydaelynsWeapon)
        {
            var activation = WorldState.FutureTime(6);
            if (status.Extra == 0x1B4)
                _aoe = new(circle, Module.PrimaryActor.Position, default, activation);
            else if (status.Extra == 0x1B5)
                _aoe = new(donut, Module.PrimaryActor.Position, default, activation);
        }
    }

    public override void OnStatusLose(Actor actor, ActorStatus status)
    {
        if ((SID)status.ID == SID.HydaelynsWeapon)
            _aoe = new(cross, Module.PrimaryActor.Position, Module.PrimaryActor.Rotation, WorldState.FutureTime(6.9f));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.Equinox2 or AID.HighestHoly or AID.Anthelion)
            _aoe = null;
    }
}
