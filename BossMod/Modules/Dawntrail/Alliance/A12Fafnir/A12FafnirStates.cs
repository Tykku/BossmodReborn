namespace BossMod.Dawntrail.Alliance.A12Fafnir;

class A12FafnirStates : StateMachineBuilder
{
    public A12FafnirStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<ArenaChange>()
            .ActivateOnEnter<GreatWhirlwindFirst1>()
            .ActivateOnEnter<GreatWhirlwindFirst2>()
            .ActivateOnEnter<Whirlwinds>()
            .ActivateOnEnter<BalefulBreath>()
            .ActivateOnEnter<DragonBreath>()
            .ActivateOnEnter<DragonBreathArenaChange>()
            .ActivateOnEnter<DarkMatterBlast>()
            .ActivateOnEnter<HorridRoarAOE>()
            .ActivateOnEnter<HorridRoarSpread>()
            .ActivateOnEnter<SpikeFlail>()
            .ActivateOnEnter<Touchdown>()
            .ActivateOnEnter<SharpSpike>()
            .ActivateOnEnter<WingedTerror>()
            .ActivateOnEnter<AbsoluteTerror>()
            .ActivateOnEnter<Venom>()
            .ActivateOnEnter<PestilentSphere>()
            .ActivateOnEnter<HurricaneWingRaidwide>()
            .ActivateOnEnter<HurricaneWingInOutShort>()
            .ActivateOnEnter<HurricaneWingInOutLong>()
            .ActivateOnEnter<HurricaneWingOutInShort>()
            .ActivateOnEnter<HurricaneWingOutInLong>();
    }
}
