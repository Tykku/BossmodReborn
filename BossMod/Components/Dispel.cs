﻿namespace BossMod.Components;

public class Dispel(BossModule module, uint statusID, uint action = default) : CastHint(module, action, "Prepare to dispel!")
{
    private readonly List<Actor> Targets = [];

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var count = Targets.Count;
        if (count == 0)
            return;
        for (var i = 0; i < count; ++i)
        {
            var enemy = hints.FindEnemy(Targets[i]);
            if (enemy != null)
                enemy.ShouldBeDispelled = true;
        }
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (Targets.Count != 0)
            hints.Add("Dispel!");
    }

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if (status.ID == statusID)
            Targets.Add(actor);
    }

    public override void OnStatusLose(Actor actor, ActorStatus status)
    {
        if (status.ID == statusID)
            Targets.Remove(actor);
    }
}
