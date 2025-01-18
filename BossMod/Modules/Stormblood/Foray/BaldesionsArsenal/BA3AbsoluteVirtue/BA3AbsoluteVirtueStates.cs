namespace BossMod.Stormblood.Foray.BaldesionArsenal.BA3AbsoluteVirtue;

class BA3AbsoluteVirtueStates : StateMachineBuilder
{
    public BA3AbsoluteVirtueStates(BossModule module) : base(module)
    {
        DeathPhase(0, SinglePhase)
            .ActivateOnEnter<Meteor>()
            .ActivateOnEnter<MedusaJavelin>()
            .ActivateOnEnter<AuroralWind>()
            .ActivateOnEnter<ExplosiveImpulse1>()
            .ActivateOnEnter<ExplosiveImpulse2>()
            .ActivateOnEnter<AernsWynavExplosion>()
            .ActivateOnEnter<BrightDarkAurora>()
            .ActivateOnEnter<AstralUmbralRays>()
        ;
    }

    private void SinglePhase(uint id)
    {
        SimpleState(id + 0xFF0000, 10000, "???");
    }
}
