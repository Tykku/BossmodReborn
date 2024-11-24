namespace BossMod.Dawntrail.Alliance.A12Fafnir;

class DarkMatterBlast(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.DarkMatterBlast));
class HurricaneWingRaidwide(BossModule module) : Components.RaidwideCastDelay(module, ActionID.MakeSpell(AID.HurricaneWingVisual), ActionID.MakeSpell(AID.HurricaneWing1), 2.7f, "Raidwide x9");
class HorridRoarAOE(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.HorridRoarAOE), 4);
class HorridRoarSpread(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.HorridRoarSpread), 8);
class SpikeFlail(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.SpikeFlail), new AOEShapeCone(80, 135.Degrees()));
class Venom(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Venom), new AOEShapeCone(30, 60.Degrees()));
class PestilentSphere(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.PestilentSphere));
class WingedTerror(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.WingedTerror), new AOEShapeRect(70, 12.5f));
class AbsoluteTerror(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.AbsoluteTerror), new AOEShapeRect(70, 10));
class Touchdown(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Touchdown), new AOEShapeCircle(24));
class BalefulBreath(BossModule module) : Components.LineStack(module, (uint)IconID.LineStack, ActionID.MakeSpell(AID.BalefulBreath2), 8.2f, 70, 3, PartyState.MaxAllianceSize, PartyState.MaxAllianceSize, 3, false);
class SharpSpike(BossModule module) : Components.BaitAwayIcon(module, new AOEShapeCircle(4), (uint)IconID.Tankbuster, ActionID.MakeSpell(AID.SharpSpike), 6.2f, true)
{
    public override void AddGlobalHints(GlobalHints hints)
    {
        if (CurrentBaits.Count > 0)
            hints.Add("Tankbuster cleave");
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1015, NameID = 13662)]
public class A12Fafnir(WorldState ws, Actor primary) : BossModule(ws, primary, ArenaCenter, new ArenaBoundsCircle(34.5f))
{
    public static readonly WPos ArenaCenter = new(-500, 600);
    public static readonly ArenaBoundsCircle DefaultBounds = new(30);
    public static readonly ArenaBoundsCircle FireArena = new(16);

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actors(WorldState.Actors.Where(x => !x.IsAlly));
    }
}
