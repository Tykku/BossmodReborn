namespace BossMod.Dawntrail.Trial.T04Zelenia;

abstract class PowerBreak(BossModule module, AID aid) : Components.SimpleAOEs(module, ActionID.MakeSpell(aid), new AOEShapeRect(24f, 32f));
class PowerBreak1(BossModule module) : PowerBreak(module, AID.PowerBreak1);
class PowerBreak2(BossModule module) : PowerBreak(module, AID.PowerBreak2);

class HolyHazard(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.HolyHazard), new AOEShapeCone(24f, 60f.Degrees()), 2);
class ValorousAscensionRect(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.ValorousAscensionRect), new AOEShapeRect(40, 4f), 2);

class RosebloodBloom(BossModule module) : Components.SimpleKnockbacks(module, ActionID.MakeSpell(AID.RosebloodBloom), 10f, true)
{
    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (Casters.Count != 0)
        {
            var source = Casters[0];
            hints.AddForbiddenZone(ShapeDistance.InvertedCircle(source.Position, 6f), Module.CastFinishAt(source.CastInfo));
        }
    }
}

class PerfumedQuietus(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.RosebloodBloom)); // using the knockback here, since after knockback player is stunned for a cutscene and can't heal up
class ValorousAscension(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.ValorousAscension1), "Raidwide x3");
class ThornedCatharsis(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.ThornedCatharsis));

class SpecterOfTheLost(BossModule module) : Components.BaitAwayCast(module, ActionID.MakeSpell(AID.SpecterOfTheLost), new AOEShapeCone(50f, 25f.Degrees()), tankbuster: true);

[ModuleInfo(BossModuleInfo.Maturity.WIP, GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1030, NameID = 13861)]
public class T04Zelenia(WorldState ws, Actor primary) : BossModule(ws, primary, arenaCenter, DefaultArena)
{
    private static readonly WPos arenaCenter = new(100f, 100f);
    public static readonly ArenaBoundsComplex DefaultArena = new([new Polygon(arenaCenter, 16f, 64)]);
    public static readonly ArenaBoundsComplex DonutArena = new([new DonutV(arenaCenter, 4f, 16f, 64)]);
}
