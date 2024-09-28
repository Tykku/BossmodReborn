namespace BossMod.Shadowbringers.Alliance.A37HerInfloresence;

class A37HerInfloresenceStates : StateMachineBuilder
{
    public A37HerInfloresenceStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<UnevenFooting>()
            .ActivateOnEnter<Crash>()
            .ActivateOnEnter<ScreamingScore>()
            .ActivateOnEnter<DarkerNote1>()
            .ActivateOnEnter<HeavyArms1>()
            .ActivateOnEnter<HeavyArms3>()
            .ActivateOnEnter<PlaceOfPower>()
            .ActivateOnEnter<Shockwave1>()
            .ActivateOnEnter<Shockwave2>()
            .ActivateOnEnter<Towerfall2>();
    }
}
