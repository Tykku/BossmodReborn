﻿namespace BossMod.Components;

// generic component that counts specified casts
public class CastCounter(BossModule module, uint aid) : BossComponent(module)
{
    public readonly uint WatchedAction = aid;
    public int NumCasts;

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action.ID == WatchedAction)
            ++NumCasts;
    }
}

public class CastCounterMulti(BossModule module, uint[] aids) : BossComponent(module)
{
    public readonly uint[] WatchedActions = aids;
    public int NumCasts;

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (WatchedActions.Contains(spell.Action.ID))
            ++NumCasts;
    }
}
