using static BossMod.Dawntrail.Raid.BruteAmbombinatorSharedBounds.BruteAmbombinatorSharedBounds;

namespace BossMod.Dawntrail.Savage.M07SBruteAbombinator;

class BrutalImpact(BossModule module) : Components.CastCounter(module, (uint)AID.BrutalImpact);
class RevengeOfTheVines2(BossModule module) : Components.CastCounter(module, (uint)AID.RevengeOfTheVines2);
class Slaminator(BossModule module) : Components.CastTowers(module, (uint)AID.Slaminator, 8f, 8, 8);

class Explosion : Components.SimpleAOEs
{
    public Explosion(BossModule module) : base(module, (uint)AID.Explosion, 20f, 2)
    {
        MaxDangerColor = 1;
    }
}

class Sporesplosion : Components.SimpleAOEs
{
    public Sporesplosion(BossModule module) : base(module, (uint)AID.Sporesplosion, 8f, 12)
    {
        MaxDangerColor = 6;
    }
}

class NeoBombarianSpecialKB(BossModule module) : Components.SimpleKnockbacks(module, (uint)AID.NeoBombarianSpecial, 58f, true)
{
    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (Casters.Count != 0)
        {
            hints.GoalZones.Add(hints.GoalSingleTarget(new WPos(100f, 80f), 5f, 5f));
        }
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1024, NameID = 13756, PlanLevel = 100)]
public class M07SBruteAbombinator(WorldState ws, Actor primary) : BossModule(ws, primary, FirstCenter, DefaultArena)
{
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies((uint)OID.BloomingAbomination));
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var count = hints.PotentialTargets.Count;
        for (var i = 0; i < count; ++i)
        {
            var e = hints.PotentialTargets[i];
            e.Priority = e.Actor.OID switch
            {
                (uint)OID.BloomingAbomination => 1,
                _ => 0
            };
        }
    }
}
