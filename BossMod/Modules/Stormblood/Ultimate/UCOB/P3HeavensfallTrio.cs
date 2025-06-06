﻿namespace BossMod.Stormblood.Ultimate.UCOB;

class P3HeavensfallTrio(BossModule module) : BossComponent(module)
{
    private Actor? _nael;
    private Actor? _twin;
    private Actor? _baha;
    private readonly WPos[] _safeSpots = new WPos[PartyState.MaxPartySize];
    private readonly UCOBConfig _config = Service.Config.Get<UCOBConfig>();

    public bool Active => _nael != null;

    private static readonly Angle[] _offsetsNaelCenter = [10.Degrees(), 80.Degrees(), 100.Degrees(), 170.Degrees()];
    private static readonly Angle[] _offsetsNaelSide = [60.Degrees(), 80.Degrees(), 100.Degrees(), 120.Degrees()];

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        Arena.Actor(_nael, Colors.Object, true);
        var safespot = _safeSpots[pcSlot];
        if (safespot != default)
            Arena.AddCircle(safespot, 1, Colors.Safe);
    }

    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if (actor.OID == (uint)OID.NaelDeusDarnus && id == 0x1E43)
        {
            _nael = actor;
            InitIfReady();
        }
        else if (actor.OID == (uint)OID.Twintania && id == 0x1E44)
        {
            _twin = actor;
            InitIfReady();
        }
        else if (actor.OID == (uint)OID.BahamutPrime && id == 0x1E43)
        {
            _baha = actor;
            InitIfReady();
        }
    }

    private void InitIfReady()
    {
        if (_nael == null || _twin == null || _baha == null)
            return;

        var dirToNael = Angle.FromDirection(_nael.Position - Arena.Center);
        var dirToTwin = Angle.FromDirection(_twin.Position - Arena.Center);
        var dirToBaha = Angle.FromDirection(_baha.Position - Arena.Center);

        var twinRel = (dirToTwin - dirToNael).Normalized();
        var bahaRel = (dirToBaha - dirToNael).Normalized();
        var (offsetSymmetry, offsets) = twinRel.Rad * bahaRel.Rad < 0 // twintania & bahamut are on different sides => nael is in center
            ? (0.Degrees(), _offsetsNaelCenter)
            : ((twinRel + bahaRel) * 0.5f, _offsetsNaelSide);
        var dirSymmetry = dirToNael + offsetSymmetry;
        foreach (var p in _config.P3QuickmarchTrioAssignments.Resolve(Raid))
        {
            var left = p.group < 4;
            var order = p.group & 3;
            var offset = offsets[order];
            var dir = dirSymmetry + (left ? offset : -offset);
            _safeSpots[p.slot] = Arena.Center + 20 * dir.ToDirection();
        }
    }
}

class P3HeavensfallTowers(BossModule module) : Components.CastTowers(module, (uint)AID.MegaflareTower, 3)
{
    private readonly UCOBConfig _config = Service.Config.Get<UCOBConfig>();

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        base.OnCastStarted(caster, spell);

        if (spell.Action.ID == WatchedAction && Towers.Count == 8)
        {
            var nael = Module.Enemies(OID.NaelDeusDarnus).FirstOrDefault();
            if (nael != null)
            {
                var dirToNael = Angle.FromDirection(nael.Position - Arena.Center);
                var orders = Towers.Select(t => TowerSortKey(Angle.FromDirection(t.Position - Arena.Center), dirToNael)).ToList();
                MemoryExtensions.Sort(orders.AsSpan(), Towers.AsSpan());
                foreach (var p in _config.P3HeavensfallTrioTowers.Resolve(Raid))
                {
                    Towers.Ref(p.group).ForbiddenSoakers = new(~(1ul << p.slot));
                }
            }
        }
    }

    // order towers from nael's position CW
    private float TowerSortKey(Angle tower, Angle reference)
    {
        var cwDist = (reference - tower).Normalized().Deg;
        if (cwDist < -5f) // towers are ~22.5 degrees apart
            cwDist += 360;
        return cwDist;
    }
}

class P3HeavensfallFireball(BossModule module) : Components.StackWithIcon(module, (uint)IconID.Fireball, (uint)AID.Fireball, 4f, 5.3f, 8, 8);
