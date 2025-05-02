using static BossMod.Dawntrail.Raid.BruteAmbombinatorSharedBounds.BruteAmbombinatorSharedBounds;

namespace BossMod.Dawntrail.Raid.M07NBruteAbombinator;

class BrutalImpact(BossModule module) : Components.RaidwideCast(module, (uint)AID.BrutalImpact);
class RevengeOfTheVines1(BossModule module) : Components.RaidwideCast(module, (uint)AID.RevengeOfTheVines1);
class NeoBombarianSpecial(BossModule module) : Components.RaidwideCast(module, (uint)AID.NeoBombarianSpecial);
class Powerslam(BossModule module) : Components.RaidwideCast(module, (uint)AID.Powerslam);
class Slaminator(BossModule module) : Components.CastTowers(module, (uint)AID.Slaminator, 8f, 8, 8);

class ElectrogeneticForce(BossModule module) : Components.SpreadFromCastTargets(module, (uint)AID.ElectrogeneticForce, 6f);
class SporeSac(BossModule module) : Components.SimpleAOEs(module, (uint)AID.SporeSac, 8f);
class Pollen(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Pollen, 8f);
class Sporesplosion : Components.SimpleAOEs
{
    public Sporesplosion(BossModule module) : base(module, (uint)AID.Sporesplosion, 8f, 12)
    {
        MaxDangerColor = 6;
    }
}

class Explosion : Components.SimpleAOEs
{
    public Explosion(BossModule module) : base(module, (uint)AID.Explosion, 25f, 2)
    {
        MaxDangerColor = 1;
    }
}

class ItCameFromTheDirt(BossModule module) : Components.SimpleAOEs(module, (uint)AID.ItCameFromTheDirt, 6f);
class CrossingCrosswinds(BossModule module) : Components.SimpleAOEs(module, (uint)AID.CrossingCrosswinds, new AOEShapeCross(50f, 5f));
class CrossingCrosswindsHint(BossModule module) : Components.CastInterruptHint(module, (uint)AID.CrossingCrosswinds, showNameInHint: true);
class TheUnpotted(BossModule module) : Components.SimpleAOEs(module, (uint)AID.TheUnpotted, new AOEShapeCone(60f, 15f.Degrees()));
class WindingWildwinds(BossModule module) : Components.SimpleAOEs(module, (uint)AID.WindingWildwinds, new AOEShapeDonut(5f, 60f));
class WindingWildwindsHint(BossModule module) : Components.CastInterruptHint(module, (uint)AID.WindingWildwinds, showNameInHint: true);
class GlowerPower(BossModule module) : Components.SimpleAOEs(module, (uint)AID.GlowerPower, new AOEShapeRect(65f, 7f));

class BrutishSwingCircle2(BossModule module) : Components.SimpleAOEs(module, (uint)AID.BrutishSwingCircle2, 12f);
class BrutishSwingDonut(BossModule module) : Components.SimpleAOEs(module, (uint)AID.BrutishSwingDonut, new AOEShapeDonut(9f, 60f));

class BrutishSwingCone(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.BrutishSwingCone1, (uint)AID.BrutishSwingCone2], new AOEShapeCone(25f, 90f.Degrees()));
class BrutishSwingDonutSegment(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.BrutishSwingDonutSegment1, (uint)AID.BrutishSwingDonutSegment2], new AOEShapeDonutSector(22f, 88f, 90f.Degrees()));
class LashingLariat(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.LashingLariat1, (uint)AID.LashingLariat2], new AOEShapeRect(70f, 16f));

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

class PulpSmash(BossModule module) : Components.StackWithIcon(module, (uint)IconID.PulpSmash, (uint)AID.PulpSmash, 6f, 5.2f, 8, 8);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1023, NameID = 13756)]
public class M07NBruteAbombinator(WorldState ws, Actor primary) : BossModule(ws, primary, FirstCenter, DefaultArena)
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
