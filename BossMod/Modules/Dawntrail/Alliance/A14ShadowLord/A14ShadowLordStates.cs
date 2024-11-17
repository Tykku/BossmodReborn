namespace BossMod.Dawntrail.Alliance.A14ShadowLord;

class A14ShadowLordStates : StateMachineBuilder
{
    public A14ShadowLordStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<GigaSlash3>()
            .ActivateOnEnter<GigaSlash4>()
            .ActivateOnEnter<GigaSlash5>()
            .ActivateOnEnter<GigaSlash6>()
            .ActivateOnEnter<UmbraSmash2>()
            .ActivateOnEnter<UmbraSmash5>()
            .ActivateOnEnter<UmbraSmash6>()
            .ActivateOnEnter<UmbraSmash7>()
            .ActivateOnEnter<UmbraSmash8>()
            .ActivateOnEnter<UmbraWave>()
            .ActivateOnEnter<FlamesOfHatred>()
            .ActivateOnEnter<Implosion1>()
            .ActivateOnEnter<Implosion2>()
            .ActivateOnEnter<Implosion3>()
            .ActivateOnEnter<Implosion4>()
            .ActivateOnEnter<CthonicFury1>()
            .ActivateOnEnter<CthonicFury2>()
            .ActivateOnEnter<BurningCourt>()
            .ActivateOnEnter<BurningKeep>()
            .ActivateOnEnter<TeraSlash>()
            .ActivateOnEnter<GigaSlashNightfall1>()
            .ActivateOnEnter<GigaSlashNightfall2>()
            .ActivateOnEnter<GigaSlashNightfall3>()
            .ActivateOnEnter<GigaSlashNightfall4>()
            .ActivateOnEnter<GigaSlashNightfall5>()
            .ActivateOnEnter<GigaSlashNightfall6>()
            .ActivateOnEnter<DarkNova>()
            .ActivateOnEnter<SoulBinding>()
            .ActivateOnEnter<Impact1>()
            .ActivateOnEnter<Impact2>()
            .ActivateOnEnter<Impact3>()
            .ActivateOnEnter<DoomArc>();
    }
}
