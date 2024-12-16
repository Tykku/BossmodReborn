﻿namespace BossMod.Dawntrail.Ultimate.FRU;

class P2MirrorMirrorReflectedScytheKickBlue(BossModule module) : Components.GenericAOEs(module, ActionID.MakeSpell(AID.ReflectedScytheKickBlue))
{
    private WDir _blueMirror;
    private AOEInstance? _aoe;

    private static readonly AOEShapeDonut _shape = new(4, 20);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        if (_blueMirror != default)
        {
            Arena.Actor(Arena.Center + 20 * _blueMirror, Angle.FromDirection(-_blueMirror), Colors.Object);
            if (_aoe == null)
            {
                // draw hint for melees
                Arena.AddCircle(Arena.Center - 11 * _blueMirror, 1, Colors.Safe);
            }
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.ScytheKick && _blueMirror != default)
            _aoe = new(_shape, Arena.Center + 20 * _blueMirror, default, Module.CastFinishAt(spell));
    }

    public override void OnEventEnvControl(byte index, uint state)
    {
        if (index is >= 1 and <= 8 && state == 0x00020001)
            _blueMirror = (225 - index * 45).Degrees().ToDirection();
    }
}

class P2MirrorMirrorReflectedScytheKickRed(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.ReflectedScytheKickRed), new AOEShapeDonut(4, 20))
{
    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        Arena.Actors(Casters, Colors.Object, true);
    }
}

class P2MirrorMirrorHouseOfLight(BossModule module) : Components.GenericBaitAway(module, ActionID.MakeSpell(AID.HouseOfLight))
{
    private WPos _mirror;
    private readonly List<(Actor source, DateTime activation)> _sources = [];

    private static readonly AOEShapeCone _shape = new(60, 15.Degrees());

    public override void Update()
    {
        CurrentBaits.Clear();
        foreach (var s in _sources.Take(2))
            foreach (var p in Raid.WithoutSlot().SortedByRange(s.source.Position).Take(4))
                CurrentBaits.Add(new(s.source, p, _shape, s.activation));
    }

    public override void OnEventEnvControl(byte index, uint state)
    {
        if (index is >= 1 and <= 8 && state == 0x00020001)
        {
            _mirror = Arena.Center + 20 * (225 - index * 45).Degrees().ToDirection();
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.ScytheKick:
                var activation = Module.CastFinishAt(spell, 0.7f);
                _sources.Add((caster, activation));
                var mirror = Module.Enemies(OID.FrozenMirror).Closest(_mirror);
                if (mirror != null)
                    _sources.Add((mirror, activation));
                break;
            case AID.ReflectedScytheKickRed:
                _sources.Add((caster, Module.CastFinishAt(spell, 0.6f)));
                break;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action == WatchedAction)
        {
            var deadline = WorldState.FutureTime(2);
            var firstInBunch = _sources.RemoveAll(s => s.activation < deadline) > 0;
            if (firstInBunch)
                ++NumCasts;
        }
    }
}
