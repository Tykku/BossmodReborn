namespace BossMod.Dawntrail.Alliance.A11Prishe;

class A11PrisheStates : StateMachineBuilder
{
    public A11PrisheStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<Banishga>()
            .ActivateOnEnter<KnuckleSandwichAOE1>()
            .ActivateOnEnter<KnuckleSandwichAOE2>()
            .ActivateOnEnter<KnuckleSandwichAOE3>()
            .ActivateOnEnter<BrittleImpact1>()
            .ActivateOnEnter<BrittleImpact2>()
            .ActivateOnEnter<BrittleImpact3>()
            .ActivateOnEnter<NullifyingDropkick1>()
            .ActivateOnEnter<Holy2>()
            .ActivateOnEnter<BanishgaIV>()
            .ActivateOnEnter<Explosion>();
    }
}
