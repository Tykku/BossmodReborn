namespace BossMod.Shadowbringers.Alliance.A36FalseIdol;

class A36FalseIdolStates : StateMachineBuilder
{
    public A36FalseIdolStates(BossModule module) : base(module)
    {
        TrivialPhase()
            //.ActivateOnEnter<MadeMagic1>() not appearing properly
            //.ActivateOnEnter<MadeMagic2>() not appearing properly
            .ActivateOnEnter<ScreamingScore>()
            .ActivateOnEnter<ScatteredMagic>()
            .ActivateOnEnter<DarkerNote2>();
    }
}
