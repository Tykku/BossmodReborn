namespace BossMod.Dawntrail.Extreme.Ex1Valigarmanda;

class SkyruinFire(BossModule module) : Components.CastCounter(module, (uint)AID.SkyruinFireAOE);
class SkyruinIce(BossModule module) : Components.CastCounter(module, (uint)AID.SkyruinIceAOE);
class SkyruinThunder(BossModule module) : Components.CastCounter(module, (uint)AID.SkyruinThunderAOE);
class DisasterZoneFire(BossModule module) : Components.CastCounter(module, (uint)AID.DisasterZoneFireAOE);
class DisasterZoneIce(BossModule module) : Components.CastCounter(module, (uint)AID.DisasterZoneIceAOE);
class DisasterZoneThunder(BossModule module) : Components.CastCounter(module, (uint)AID.DisasterZoneThunderAOE);
class Tulidisaster1(BossModule module) : Components.CastCounter(module, (uint)AID.TulidisasterAOE1);
class Tulidisaster2(BossModule module) : Components.CastCounter(module, (uint)AID.TulidisasterAOE2);
class Tulidisaster3(BossModule module) : Components.CastCounter(module, (uint)AID.TulidisasterAOE3);
class IceTalon(BossModule module) : Components.BaitAwayIcon(module, 6f, (uint)IconID.IceTalon, (uint)AID.IceTalonAOE, 5.1f, tankbuster: true);
class WrathUnfurled(BossModule module) : Components.CastCounter(module, (uint)AID.WrathUnfurledAOE);
class TulidisasterEnrage1(BossModule module) : Components.CastCounter(module, (uint)AID.TulidisasterEnrageAOE1);
class TulidisasterEnrage2(BossModule module) : Components.CastCounter(module, (uint)AID.TulidisasterEnrageAOE2);
class TulidisasterEnrage3(BossModule module) : Components.CastCounter(module, (uint)AID.TulidisasterEnrageAOE3);

// TODO: investigate how exactly are omens drawn for northern cross & susurrant breath
[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 833, NameID = 12854, PlanLevel = 100)]
public class Ex1Valigarmanda(WorldState ws, Actor primary) : BossModule(ws, primary, new(100, 100), new ArenaBoundsRect(20, 15))
{
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.IceBoulderJail));
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        foreach (var e in hints.PotentialTargets)
        {
            e.Priority = (OID)e.Actor.OID switch
            {
                OID.IceBoulderJail => 2,
                OID.Boss => 1,
                _ => 0
            };
        }
    }
}

