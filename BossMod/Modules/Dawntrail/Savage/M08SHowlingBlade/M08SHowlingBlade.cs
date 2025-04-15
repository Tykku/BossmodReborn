namespace BossMod.Dawntrail.Savage.M08SHowlingBlade;

class ExtraplanarPursuit(BossModule module) : Components.CastCounter(module, ActionID.MakeSpell(AID.ExtraplanarPursuit));
class TitanicPursuit(BossModule module) : Components.CastCounter(module, ActionID.MakeSpell(AID.TitanicPursuit));
class HowlingHavoc(BossModule module) : Components.CastCounter(module, ActionID.MakeSpell(AID.HowlingHavoc));
class GreatDivide(BossModule module) : Components.CastSharedTankbuster(module, ActionID.MakeSpell(AID.GreatDivide), new AOEShapeRect(60f, 3f));
class RavenousSaber(BossModule module) : Components.CastCounterMulti(module, [ActionID.MakeSpell(AID.RavenousSaber1),
ActionID.MakeSpell(AID.RavenousSaber2), ActionID.MakeSpell(AID.RavenousSaber3), ActionID.MakeSpell(AID.RavenousSaber4),
ActionID.MakeSpell(AID.RavenousSaber5)]);
class Mooncleaver1(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Mooncleaver1), 8f);
class ProwlingGaleP2(BossModule module) : Components.CastTowers(module, ActionID.MakeSpell(AID.ProwlingGaleP2), 2f, 2, 2);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1026, NameID = 13843, PlanLevel = 100)]
public class M08SHowlingBlade(WorldState ws, Actor primary) : BossModule(ws, primary, ArenaCenter, StartingArena)
{
    private Actor? _bossP2;

    public Actor? BossP1() => PrimaryActor;
    public Actor? BossP2() => _bossP2;

    protected override void UpdateModule()
    {
        // TODO: this is an ugly hack, think how multi-actor fights can be implemented without it...
        // the problem is that on wipe, any actor can be deleted and recreated in the same frame
        if (_bossP2 == null)
        {
            if (StateMachine.ActivePhaseIndex == 1)
            {
                var b = Enemies((uint)OID.BossP2);
                _bossP2 = b.Count != 0 ? b[0] : null;
            }
        }
    }

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actor(_bossP2);
    }

    public static readonly WPos ArenaCenter = new(100f, 100f);
    public static readonly Polygon[] StartingArenaPolygon = [new(ArenaCenter, 12f, 40)];
    public static readonly ArenaBoundsComplex StartingArena = new(StartingArenaPolygon);
    public static readonly ArenaBoundsComplex DonutArena = new(StartingArenaPolygon, [new Polygon(ArenaCenter, 8f, 40)]);
}