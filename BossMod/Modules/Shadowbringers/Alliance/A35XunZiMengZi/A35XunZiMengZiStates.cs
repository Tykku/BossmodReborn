namespace BossMod.Shadowbringers.Alliance.A35XunZiMengZi;

class A35XunZiMengZiStates : StateMachineBuilder
{
    public A35XunZiMengZiStates(BossModule module) : base(module)
    {
        TrivialPhase()
            //.ActivateOnEnter<DeployArmaments1>() unneeded
            //.ActivateOnEnter<DeployArmaments2>() unneeded
            .ActivateOnEnter<DeployArmaments3>()
            //.ActivateOnEnter<DeployArmaments4>() unneeded
            //.ActivateOnEnter<DeployArmaments5>() unneeded
            .ActivateOnEnter<DeployArmaments6>()
            .ActivateOnEnter<DeployArmaments7>()
            .ActivateOnEnter<DeployArmaments8>()
            .ActivateOnEnter<UniversalAssault>();
    }
}
