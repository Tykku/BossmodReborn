namespace BossMod.Shadowbringers.Alliance.A13Engels;

class A13MarxEngelsStates : StateMachineBuilder
{
    public A13MarxEngelsStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<DemolishStructureArenaChange>()
            .ActivateOnEnter<MarxSmash3>()
            .ActivateOnEnter<MarxSmash2>()
            .ActivateOnEnter<PrecisionGuidedMissile2>()
            .ActivateOnEnter<DiffuseLaser>()
            .ActivateOnEnter<LaserSight1>()
            .ActivateOnEnter<GuidedMissile2>()
            .ActivateOnEnter<IncendiaryBombing2>()
            //.ActivateOnEnter<IncendiaryBombing1>()
            .ActivateOnEnter<GuidedMissile>()
            .ActivateOnEnter<DiffuseLaser>()
            .ActivateOnEnter<MarxSmash6>()
            //.ActivateOnEnter<MarxSmash8>()
            //.ActivateOnEnter<MarxSmash10>() needs placement adjustment
            .ActivateOnEnter<MarxSmash12>()
            .ActivateOnEnter<MarxSmash13>()
            .ActivateOnEnter<MarxCrush2>()
            .ActivateOnEnter<SurfaceMissile2>();
    }
}
