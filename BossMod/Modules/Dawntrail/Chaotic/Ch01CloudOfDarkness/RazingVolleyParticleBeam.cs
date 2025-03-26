﻿namespace BossMod.Dawntrail.Chaotic.Ch01CloudOfDarkness;

class RazingVolleyParticleBeam(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.RazingVolleyParticleBeam), new AOEShapeRect(45f, 4f))
{
    private DateTime _nextBundle;

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = Casters.Count;
        if (count == 0)
            return [];
        var aoes = CollectionsMarshal.AsSpan(Casters);
        var deadline = aoes[0].Activation.AddSeconds(1d);

        var index = 0;
        while (index < count && aoes[index].Activation < deadline)
            ++index;

        return aoes[..index];
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        base.OnCastStarted(caster, spell);
        if (spell.Action == WatchedAction)
            NumCasts = 0;
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action == WatchedAction && WorldState.CurrentTime > _nextBundle)
        {
            ++NumCasts;
            _nextBundle = WorldState.FutureTime(1d);
        }
    }
}
