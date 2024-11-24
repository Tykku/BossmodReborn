namespace BossMod.Dawntrail.Alliance.A13ArkAngels;

class Cloudsplitter2(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.Cloudsplitter2), 6);

class TachiGekko(BossModule module) : Components.CastGaze(module, ActionID.MakeSpell(AID.TachiGekko));
class TachiKasha(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.TachiKasha), new AOEShapeCircle(4));
class TachiYukikaze(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.TachiYukikaze), new AOEShapeRect(70, 2.5f, 70));
class ConcertedDissolution(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.ConcertedDissolution), new AOEShapeCone(40, 15.Degrees()));
class LightsChain(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.LightsChain), new AOEShapeDonut(6, 60));
class Guillotine1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Guillotine1), new AOEShapeCone(80, 135.Degrees()));
class DominionSlash(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.DominionSlash));
class DivineDominion(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.DivineDominion1), new AOEShapeCircle(6));
class CrossReaver2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.CrossReaver2), new AOEShapeCross(50, 6));
class Holy(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.Holy));
class SpiralFinish2(BossModule module) : Components.KnockbackFromCastTarget(module, ActionID.MakeSpell(AID.SpiralFinish2), 16, stopAtWall: true, kind: Kind.AwayFromOrigin);

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team (LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1015, NameID = 13640)]
public class A13ArkAngels(WorldState ws, Actor primary) : BossModule(ws, primary, new(865, -820), new ArenaBoundsCircle(30))
{
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.ArkAngelGK));
        Arena.Actors(Enemies(OID.ArkAngelHM));
        Arena.Actors(Enemies(OID.ArkAngelEV));
        Arena.Actors(Enemies(OID.ArkAngelTT));
        Arena.Actors(Enemies(OID.ArkShield));
    }
}
