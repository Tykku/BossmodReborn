namespace BossMod.Shadowbringers.Alliance.A34RedGirlP1;

class A34RedGirlP1States : StateMachineBuilder
{
    public A34RedGirlP1States(BossModule module) : base(module)
    {
        TrivialPhase()
            //.ActivateOnEnter<Cruelty1>()
            //.ActivateOnEnter<GenerateBarrier2>()
            //.ActivateOnEnter<GenerateBarrier3>()
            //.ActivateOnEnter<DiffuseEnergy1>()
            .ActivateOnEnter<ShockWhite2>()
            .ActivateOnEnter<ShockBlack2>();
    }
}
