namespace BossMod.Dawntrail.Alliance.A12Fafnir;

class A12FafnirStates : StateMachineBuilder
{
    public A12FafnirStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<DarkMatterBlast>()
            .ActivateOnEnter<HorridRoar2>()
            .ActivateOnEnter<HorridRoar3>()
            .ActivateOnEnter<SpikeFlail>()
            .ActivateOnEnter<Touchdown>()
            .ActivateOnEnter<HurricaneWing1>();
    }
}
