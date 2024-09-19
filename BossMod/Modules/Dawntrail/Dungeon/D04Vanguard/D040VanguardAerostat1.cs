namespace BossMod.Dawntrail.Dungeon.D04Vanguard.D040VanguardAerostat1;

public enum OID : uint
{
    Boss = 0x41DA, //R=2.3
    Aerostat2 = 0x447B //R=2.3
}

public enum AID : uint
{
    AutoAttack = 871, // Boss/Aerostat2->player, no cast, single-target

    IncendiaryRing = 38452 // Aerostat2->self, 4.8s cast, range 3-12 donut
}

class IncendiaryRing(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.IncendiaryRing), new AOEShapeDonut(3, 12));

class D040VanguardAerostat1States : StateMachineBuilder
{
    public D040VanguardAerostat1States(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<IncendiaryRing>()
            .Raw.Update = () => module.Enemies(OID.Aerostat2).Concat([module.PrimaryActor]).All(e => e.IsDeadOrDestroyed);
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 831, NameID = 12780, SortOrder = 3)]
public class D040VanguardAerostat1(WorldState ws, Actor primary) : BossModule(ws, primary, new(-50, -15), new ArenaBoundsRect(7.7f, 25))
{
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.Aerostat2));
    }
}
