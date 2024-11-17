namespace BossMod.Dawntrail.Alliance.A13ArkAngels;

class A13ArkAngelsStates : StateMachineBuilder
{
    public A13ArkAngelsStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<Cloudsplitter2>()
            .ActivateOnEnter<TachiGekko>()
            .ActivateOnEnter<TachiKasha>()
            .ActivateOnEnter<TachiYukikaze>()
            .ActivateOnEnter<ConcertedDissolution>()
            .ActivateOnEnter<LightsChain>()
            .ActivateOnEnter<Guillotine1>()
            .ActivateOnEnter<DominionSlash>()
            .ActivateOnEnter<DivineDominion>()
            .ActivateOnEnter<CrossReaver2>()
            .ActivateOnEnter<Holy>()
            .ActivateOnEnter<SpiralFinish2>();
    }
}
