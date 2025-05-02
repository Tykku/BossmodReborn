namespace BossMod.Shadowbringers.Foray.TheDalriada.DAL1Sartauvoir;

class PyrokinesisAOE(BossModule module) : Components.RaidwideCast(module, (uint)AID.PyrokinesisAOE);

class Flamedive(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Flamedive, new AOEShapeRect(55f, 2.5f));
class BurningBlade(BossModule module) : Components.SingleTargetCast(module, (uint)AID.BurningBlade);

class MannatheihwonFlame2(BossModule module) : Components.RaidwideCast(module, (uint)AID.MannatheihwonFlame2);
class MannatheihwonFlame3(BossModule module) : Components.SimpleAOEs(module, (uint)AID.MannatheihwonFlame3, new AOEShapeRect(50f, 4f));
class MannatheihwonFlame4(BossModule module) : Components.SimpleAOEs(module, (uint)AID.MannatheihwonFlame4, 10f);

abstract class Brand(BossModule module, uint aid) : Components.SimpleAOEs(module, aid, new AOEShapeCone(40f, 90f.Degrees()));
class LeftBrand(BossModule module) : Brand(module, (uint)AID.LeftBrand);
class RightBrand(BossModule module) : Brand(module, (uint)AID.RightBrand);

class Pyrocrisis(BossModule module) : Components.SpreadFromCastTargets(module, (uint)AID.Pyrocrisis, 6f);
class Pyrodoxy(BossModule module) : Components.StackWithCastTargets(module, (uint)AID.Pyrodoxy, 6f, 8);

class ThermalGustAOE(BossModule module) : Components.SimpleAOEs(module, (uint)AID.ThermalGustAOE, new AOEShapeRect(44f, 5f));
class GrandCrossflameAOE(BossModule module) : Components.SimpleAOEs(module, (uint)AID.GrandCrossflameAOE, new AOEShapeRect(40f, 9f));
class TimeEruption(BossModule module) : Components.SimpleAOEGroups(module, [(uint)AID.ReverseTimeEruptionAOEFirst, (uint)AID.ReverseTimeEruptionAOESecond,
(uint)AID.TimeEruptionAOEFirst, (uint)AID.TimeEruptionAOESecond], new AOEShapeRect(20f, 10f), 2, 4);

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team", GroupType = BossModuleInfo.GroupType.BozjaCE, GroupID = 778, NameID = 32, SortOrder = 2)] //BossNameID = 9384
public class DAL1Sartauvoir(WorldState ws, Actor primary) : BossModule(ws, primary, new(631f, 157f), new ArenaBoundsSquare(20f))
{
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actors(Enemies((uint)OID.BossP2));
        Arena.Actor(PrimaryActor);
    }
}
