namespace BossMod.Stormblood.Foray.BaldesionArsenal.BA2Raiden;

class SpiritsOfTheFallen(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.SpiritsOfTheFallen))
{
    public override bool KeepOnPhaseChange => true;
}
class Levinwhorl(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.Levinwhorl));
class Shingan(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.Shingan))
{
    public override bool KeepOnPhaseChange => true;
}
class AmeNoSakahoko(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.AmeNoSakahoko), new AOEShapeCircle(25))
{
    public override bool KeepOnPhaseChange => true;
}
class WhirlingZantetsuken(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.WhirlingZantetsuken), new AOEShapeDonut(5, 60))
{
    public override bool KeepOnPhaseChange => true;
}
class Shock(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Shock), new AOEShapeCircle(8));
class ForHonor(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.ForHonor), new AOEShapeCircle(11.4f));

abstract class LateralZantetsuken(BossModule module, AID aid) : Components.SimpleAOEs(module, ActionID.MakeSpell(aid), new AOEShapeRect(75.4f, 19.5f));
class LateralZantetsuken1(BossModule module) : LateralZantetsuken(module, AID.LateralZantetsuken1);
class LateralZantetsuken2(BossModule module) : LateralZantetsuken(module, AID.LateralZantetsuken2);

class BitterBarbs(BossModule module) : Components.Chains(module, (uint)TetherID.Chains, ActionID.MakeSpell(AID.BitterBarbs));
class BoomingLament(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.BoomingLament), 10);
class SilentLevin(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.SilentLevin), 5);

class UltimateZantetsuken(BossModule module) : Components.CastHint(module, ActionID.MakeSpell(AID.UltimateZantetsuken), "Enrage, kill the adds!", true);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.BaldesionArsenal, GroupID = 639, NameID = 7973, PlanLevel = 70, SortOrder = 3)]
public class BA2Raiden(WorldState ws, Actor primary) : BossModule(ws, primary, startingArena.Center, startingArena)
{
    private static readonly WPos ArenaCenter = new(0, 458);
    private static readonly ArenaBoundsComplex startingArena = new([new Polygon(ArenaCenter, 34.6f, 80)], [new Rectangle(new(35.3f, 458), 0.99f, 20), new Rectangle(new(-35.4f, 458), 1.65f, 20),
    new Rectangle(new(0, 493), 20, 0.75f)]);
    public static readonly ArenaBoundsComplex DefaultArena = new([new Polygon(ArenaCenter, 29.93f, 64)]);

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.StreakLightning));
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        for (var i = 0; i < hints.PotentialTargets.Count; ++i)
        {
            var e = hints.PotentialTargets[i];
            e.Priority = (OID)e.Actor.OID switch
            {
                OID.StreakLightning => 1,
                _ => 0
            };
        }
    }
}
