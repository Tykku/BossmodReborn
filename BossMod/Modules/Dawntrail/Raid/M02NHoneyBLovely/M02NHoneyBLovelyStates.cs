namespace BossMod.Dawntrail.Raid.M02NHoneyBLovely;

class M02NHoneyBLovelyStates : StateMachineBuilder
{
    public M02NHoneyBLovelyStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<CallMeHoney>()
            .ActivateOnEnter<Sweethearts>()
            .ActivateOnEnter<TemptingTwist1>()
            .ActivateOnEnter<TemptingTwist2>()
            .ActivateOnEnter<HoneyBeeline1>()
            .ActivateOnEnter<HoneyBeeline2>()
            .ActivateOnEnter<HoneyedBreeze>()
            .ActivateOnEnter<HoneyBLive>()
            .ActivateOnEnter<Heartsore>()
            .ActivateOnEnter<Heartsick>()
            .ActivateOnEnter<Fracture>()
            .ActivateOnEnter<Loveseeker>()
            .ActivateOnEnter<BlowKiss>()
            .ActivateOnEnter<HoneyBFinale>()
            .ActivateOnEnter<DropOfVenom>()
            .ActivateOnEnter<SplashOfVenom>()
            .ActivateOnEnter<Splinter>()
            .ActivateOnEnter<BlindingLove1>()
            .ActivateOnEnter<BlindingLove2>()
            .ActivateOnEnter<HeartStruck1>()
            .ActivateOnEnter<HeartStruck2>()
            .ActivateOnEnter<HeartStruck3>();
    }
}
