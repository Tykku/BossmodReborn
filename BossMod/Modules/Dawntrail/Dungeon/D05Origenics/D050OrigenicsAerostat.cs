namespace BossMod.Dawntrail.Dungeon.D05Origenics.D050OrigenicsAerostat;

public enum OID : uint
{
    Boss = 0x41E2, //R=2.3
    Aerostat2 = 0x42BA, //R=2.3
    OrigenicsSentryS9 = 0x43D6, // R0.65
    OrigenicsSentryS92 = 0x4189, // R0.65
    OrigenicsSentryG10 = 0x43D7 // R0.8
}

public enum AID : uint
{
    AutoAttack1 = 871, // Boss/Aerostat2->player, no cast, single-target
    AutoAttack2 = 873, // OrigenicsSentryG10->player, no cast, single-target
    AutoAttack3 = 870, // OrigenicsSentryS9/OrigenicsSentryS92->player, no cast, single-target

    IncendiaryCircle = 38328, // Aerostat2->self, 4.0s cast, range 3-12 donut
    SentryfugalSlash = 35425, // OrigenicsSentryS92->player, no cast, single-target
    GrenadoShot = 35428, // OrigenicsSentryG10->location, 3.0s cast, range 5 circle
}

class IncendiaryCircle(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.IncendiaryCircle), new AOEShapeDonut(3, 12));
class GrenadoShot(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.GrenadoShot), 5);

class D050OrigenicsAerostatStates : StateMachineBuilder
{
    public D050OrigenicsAerostatStates(D050OrigenicsAerostat module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<IncendiaryCircle>()
            .ActivateOnEnter<GrenadoShot>()
            .Raw.Update = () => module.Enemies(OID.Aerostat2).Concat([module.PrimaryActor]).Concat(module.Enemies(OID.OrigenicsSentryS9))
            .Concat(module.Enemies(OID.OrigenicsSentryS92)).Concat(module.Enemies(OID.OrigenicsSentryG10)).All(e => e.IsDeadOrDestroyed);
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 825, NameID = 12895, SortOrder = 2)]
public class D050OrigenicsAerostat(WorldState ws, Actor primary) : BossModule(ws, primary, arena.Center, arena)
{
    private static readonly ArenaBoundsComplex arena = new([new Polygon(new(-116, -80), 14.5f, 6, 30.Degrees()), new Rectangle(new(-88, -80), 20, 5.5f),
    new Polygon(new(-60, -80), 14.5f, 6, 30.Degrees()), new Rectangle(new(-144, -80), 20, 5.5f)]);

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.Aerostat2).Concat(Enemies(OID.OrigenicsSentryS9)).Concat(Enemies(OID.OrigenicsSentryS92)).Concat(Enemies(OID.OrigenicsSentryG10)));
    }
}
