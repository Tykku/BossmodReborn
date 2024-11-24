namespace BossMod.Dawntrail.Alliance.A11Prishe;

class A11PrisheStates : StateMachineBuilder
{
    public A11PrisheStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<ArenaChanges>()
            .ActivateOnEnter<AuroralUppercut>()
            .ActivateOnEnter<AsuranFists>()
            .ActivateOnEnter<Banishga>()
            .ActivateOnEnter<KnuckleSandwich1>()
            .ActivateOnEnter<KnuckleSandwich2>()
            .ActivateOnEnter<KnuckleSandwich3>()
            .ActivateOnEnter<NullifyingDropkick>()
            .ActivateOnEnter<Holy>()
            .ActivateOnEnter<BanishStorm>()
            .ActivateOnEnter<BanishgaIV>()
            .ActivateOnEnter<Explosion>();
    }
}
