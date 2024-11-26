namespace BossMod.Dawntrail.Alliance.A13ArkAngels;

class DecisiveBattle(BossModule module) : BossComponent(module)
{
    private readonly Actor?[] _assignedBoss = new Actor?[PartyState.MaxAllianceSize];

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (slot < _assignedBoss.Length && _assignedBoss[slot] != null)
        {
            var target = WorldState.Actors.Find(actor.TargetID);
            if (target != null && target != _assignedBoss[slot] && (OID)target.OID is OID.BossMR or OID.BossTT or OID.BossGK)
                hints.Add($"Target {_assignedBoss[slot]?.Name}!");
        }
    }

    public override void OnTethered(Actor source, ActorTetherInfo tether)
    {
        if (tether.ID == (uint)TetherID.DecisiveBattle && Raid.FindSlot(source.InstanceID) is var slot && slot >= 0)
        {
            _assignedBoss[slot] = WorldState.Actors.Find(tether.Target);
        }
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (slot < _assignedBoss.Length && _assignedBoss[slot] != null)
            for (var i = 0; i < hints.PotentialTargets.Count; ++i)
            {
                var enemy = hints.PotentialTargets[i];
                if (enemy.Actor != _assignedBoss[slot])
                    enemy.Priority = AIHints.Enemy.PriorityForbidFully;
            }
    }
}
