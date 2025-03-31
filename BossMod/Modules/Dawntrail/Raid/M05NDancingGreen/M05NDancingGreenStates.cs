namespace BossMod.Dawntrail.Raid.M05NDancingGreen;

class M05NDancingGreenStates : StateMachineBuilder
{
    public M05NDancingGreenStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<DoTheHustle1>()
            .ActivateOnEnter<DoTheHustle2>()
            .ActivateOnEnter<Spotlight>()
            .ActivateOnEnter<Moonburn>()
            .ActivateOnEnter<DeepCut>()
            .ActivateOnEnter<FullBeat>()
            .ActivateOnEnter<CelebrateGoodTimes>()
            .ActivateOnEnter<LetsPose1>()
            .ActivateOnEnter<LetsPose2>()
            .ActivateOnEnter<EighthBeats>()
            .ActivateOnEnter<FunkyFloor>()
            .ActivateOnEnter<LetsDance>()
            .ActivateOnEnter<DiscoInfernal>()
            .ActivateOnEnter<RideTheWaves>()
            .ActivateOnEnter<TwoFourSnapTwist>();
    }
}
