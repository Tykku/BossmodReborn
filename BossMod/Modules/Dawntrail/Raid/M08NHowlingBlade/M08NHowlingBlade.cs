namespace BossMod.Dawntrail.Raid.M08NHowlingBlade;

class ExtraplanarPursuit(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.ExtraplanarPursuit));
class TitanicPursuit(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.TitanicPursuit));
class RavenousSaber(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.RavenousSaber5), "Raidwide x5");
class GreatDivide(BossModule module) : Components.CastSharedTankbuster(module, ActionID.MakeSpell(AID.GreatDivide), new AOEShapeRect(60f, 3f));
class Heavensearth1(BossModule module) : Components.StackWithCastTargets(module, ActionID.MakeSpell(AID.Heavensearth1), 6, 8, 8);
class Heavensearth2(BossModule module) : Components.StackWithCastTargets(module, ActionID.MakeSpell(AID.Heavensearth2), 6, 8, 8);
class Gust(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.Gust), 5f);
class TargetedQuake(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.TargetedQuake), 4f);
class FangedCharge : Components.SimpleAOEs
{
    public FangedCharge(BossModule module) : base(module, ActionID.MakeSpell(AID.FangedCharge), new AOEShapeRect(46f, 3f))
    {
        MaxDangerColor = 2;
        MaxRisky = 2;
    }
}

class TerrestrialTitans(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.TerrestrialTitans), 3f);
class RoaringWind(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.RoaringWind), new AOEShapeRect(40f, 4f));

abstract class Shadowchase(BossModule module, AID aid) : Components.SimpleAOEs(module, ActionID.MakeSpell(aid), new AOEShapeRect(40f, 4f));
class Shadowchase1(BossModule module) : Shadowchase(module, AID.Shadowchase1);
class Shadowchase2(BossModule module) : Shadowchase(module, AID.Shadowchase2);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1025, NameID = 13843)]
public class M08NHowlingBlade(WorldState ws, Actor primary) : BossModule(ws, primary, ArenaCenter, startingArena)
{
    public static readonly WPos ArenaCenter = new(100f, 100f);
    private static readonly ArenaBoundsComplex startingArena = new([new Polygon(ArenaCenter, 15f, 40)]);
    public static readonly Polygon[] EndArenaPolygon = [new Polygon(ArenaCenter, 12f, 40)]; // 11.2s after 0x200010 then 0x00 20001
    public static readonly ArenaBoundsComplex EndArena = new(EndArenaPolygon);
}
