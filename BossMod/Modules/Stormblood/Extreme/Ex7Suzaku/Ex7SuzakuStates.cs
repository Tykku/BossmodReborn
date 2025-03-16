namespace BossMod.Stormblood.Extreme.Ex7Suzaku;

class Ex7SuzakuStates : StateMachineBuilder
{
    public Ex7SuzakuStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<ArenaChange>()
            .ActivateOnEnter<Cremate>()
            .ActivateOnEnter<ScreamsOfTheDamned>()
            .ActivateOnEnter<AshesToAshes>()
            .ActivateOnEnter<ScarletFever>()
            .ActivateOnEnter<SouthronStar>()
            .ActivateOnEnter<Rout>()
            .ActivateOnEnter<RekindleSpread>()
            .ActivateOnEnter<FleetingSummer>()
            .ActivateOnEnter<RapturousEchoTowers>()
            .ActivateOnEnter<ScarletMelody>()
            .ActivateOnEnter<ScarletPlumeTailFeather>()
            .ActivateOnEnter<MesmerizingMelody>()
            .ActivateOnEnter<RuthlessRefrain>()
            .ActivateOnEnter<PayThePiper>()
            .ActivateOnEnter<WellOfFlame>()
            .ActivateOnEnter<ScathingNetStack>()
            .ActivateOnEnter<PhantomFlurryAOE>()
            .ActivateOnEnter<Hotspot>();
    }
}