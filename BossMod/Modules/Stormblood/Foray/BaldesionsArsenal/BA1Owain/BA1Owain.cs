namespace BossMod.Stormblood.Foray.BaldesionArsenal.BA1Owain;

class Thricecull(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.Thricecull));
class AcallamNaSenorach(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.AcallamNaSenorach));
class LegendaryImbas(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.LegendaryImbas)); // applies dorito stacks, seems to get skipped if less than 4 people alive?
class Pitfall(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Pitfall), 20);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.BaldesionArsenal, GroupID = 639, NameID = 7970, PlanLevel = 70, SortOrder = 2)]
public class BA1Owain(WorldState ws, Actor primary) : BossModule(ws, primary, arena.Center, arena)
{
    private static readonly ArenaBoundsComplex arena = new([new Polygon(new(128.98f, 748), 29.5f, 64)], [new Rectangle(new(129, 718), 20, 0.8f), new Rectangle(new(129, 778), 20, 0.825f),
    new Polygon(new(123.5f, 778), 1.5f, 8), new Polygon(new(134.5f, 778), 1.5f, 8), new Polygon(new(123.5f, 718), 1.5f, 8), new Polygon(new(134.5f, 718), 1.5f, 8)]);

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.IvoryPalm));
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        for (var i = 0; i < hints.PotentialTargets.Count; ++i)
        {
            var e = hints.PotentialTargets[i];
            e.Priority = (OID)e.Actor.OID switch
            {
                OID.IvoryPalm => 1,
                _ => 0
            };
        }
    }
}
