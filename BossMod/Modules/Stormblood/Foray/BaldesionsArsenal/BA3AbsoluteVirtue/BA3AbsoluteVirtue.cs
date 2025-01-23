namespace BossMod.Stormblood.Foray.BaldesionArsenal.BA3AbsoluteVirtue;

class Meteor(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.Meteor));
class MedusaJavelin(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.MedusaJavelin), new AOEShapeCone(65.4f, 45.Degrees()));
class AuroralWind(BossModule module) : Components.BaitAwayCast(module, ActionID.MakeSpell(AID.AuroralWind), new AOEShapeCircle(5), true)
{
    public override void AddGlobalHints(GlobalHints hints)
    {
        if (CurrentBaits.Count != 0)
            hints.Add("Tankbuster cleave");
    }
}

abstract class ExplosiveImpulse(BossModule module, AID aid) : Components.SimpleAOEs(module, ActionID.MakeSpell(aid), 18);
class ExplosiveImpulse1(BossModule module) : ExplosiveImpulse(module, AID.ExplosiveImpulse1);
class ExplosiveImpulse2(BossModule module) : ExplosiveImpulse(module, AID.ExplosiveImpulse2);

class AernsWynavExplosion(BossModule module) : Components.CastHint(module, ActionID.MakeSpell(AID.ExplosionWyvern), "Aerns Wyvnav is enraging!", true);
class MeteorEnrageCounter(BossModule module) : Components.CastCounter(module, ActionID.MakeSpell(AID.MeteorEnrageRepeat));

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.BaldesionArsenal, GroupID = 639, NameID = 7976, PlanLevel = 70, SortOrder = 4)]
public class BA3AbsoluteVirtue(WorldState ws, Actor primary) : BossModule(ws, primary, arena.Center, arena)
{
    private static readonly ArenaBoundsComplex arena = new([new Polygon(new(-175, 314), 29.95f, 96), new Rectangle(new(-146, 314), 0.8f, 5.8f), new Rectangle(new(-175, 285), 6, 1.05f)],
    [new Rectangle(new(-144.4f, 314), 0.8f, 5.8f), new Polygon(new(-144.85f, 306.75f), 1.5f, 8, 22.5f.Degrees()), new Polygon(new(-144.85f, 321.25f), 1.5f, 8, 22.5f.Degrees()),
    new Rectangle(new(-206, 314), 1.525f, 20)]);

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.AernsWynav));
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        for (var i = 0; i < hints.PotentialTargets.Count; ++i)
        {
            var e = hints.PotentialTargets[i];
            e.Priority = (OID)e.Actor.OID switch
            {
                OID.AernsWynav => 1,
                _ => 0
            };
        }
    }
}
