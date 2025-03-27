namespace BossMod.Dawntrail.Unreal.UnSuzaku;

class IncandescentInterlude(BossModule module) : Components.GenericTowers(module)
{
    private BitMask _forbidden;
    public readonly List<Tower> TowerCache = new(4);
    private readonly RuthlessRefrain _kb = module.FindComponent<RuthlessRefrain>()!;

    public override void OnActorCreated(Actor actor)
    {
        if (actor.OID == (uint)OID.Towers)
            TowerCache.Add(new(WPos.ClampToGrid(actor.Position), 4f, activation: WorldState.FutureTime(9.7d))); // no use to draw towers before spread markers are out
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action.ID is (uint)AID.Burn or (uint)AID.IncandescentInterlude)
            Towers.Clear();
    }

    public override void OnEventIcon(Actor actor, uint iconID, ulong targetID)
    {
        if (TowerCache.Count != 0 && iconID == (uint)IconID.Spreadmarker && Raid.FindSlot(actor.InstanceID) is var slot)
        {
            _forbidden[slot] = true;
            if (Towers.Count == 0)
                Towers = TowerCache;
            var count = Towers.Count;
            var towers = CollectionsMarshal.AsSpan(Towers);
            for (var i = 0; i < count; ++i)
            {
                ref var t = ref towers[i];
                t.ForbiddenSoakers = _forbidden;
            }
        }
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        base.AddAIHints(slot, actor, assignment, hints);
        if (_kb.Casters.Count != 0)
        {
            var towers = Module.Enemies((uint)OID.Towers);
            var forbidden = new Func<WPos, float>[4];
            for (var i = 0; i < 4; ++i)
                forbidden[i] = ShapeDistance.Cone(UnSuzaku.ArenaCenter, 20f, _forbidden[slot] ? Angle.AnglesCardinals[i] : Angle.AnglesIntercardinals[i], 35f.Degrees());
            hints.AddForbiddenZone(ShapeDistance.Union(forbidden), Module.CastFinishAt(_kb.Casters[0].CastInfo));
        }
    }
}
