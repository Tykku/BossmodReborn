namespace BossMod.Dawntrail.Raid.M07NBruteAbombinator;

class M07NBruteAbombinatorStates : StateMachineBuilder
{
    public M07NBruteAbombinatorStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<ArenaChanges>()
            .ActivateOnEnter<BrutalImpact>()
            .ActivateOnEnter<RevengeOfTheVines1>()
            .ActivateOnEnter<BrutishSwingCircle2>()
            .ActivateOnEnter<BrutishSwingDonut>()
            .ActivateOnEnter<BrutishSwingCone>()
            .ActivateOnEnter<BrutishSwingDonutSegment>()
            .ActivateOnEnter<NeoBombarianSpecial>()
            .ActivateOnEnter<NeoBombarianSpecialKB>()
            .ActivateOnEnter<SporeSac>()
            .ActivateOnEnter<Pollen>()
            .ActivateOnEnter<Powerslam>()
            .ActivateOnEnter<ItCameFromTheDirt>()
            .ActivateOnEnter<TheUnpotted>()
            .ActivateOnEnter<CrossingCrosswinds>()
            .ActivateOnEnter<CrossingCrosswindsHint>()
            .ActivateOnEnter<WindingWildwinds>()
            .ActivateOnEnter<WindingWildwindsHint>()
            .ActivateOnEnter<Explosion>()
            .ActivateOnEnter<GlowerPower>()
            .ActivateOnEnter<ElectrogeneticForce>()
            .ActivateOnEnter<LashingLariat>()
            .ActivateOnEnter<Slaminator>()
            .ActivateOnEnter<PulpSmash>()
            .ActivateOnEnter<Sporesplosion>()
            .ActivateOnEnter<AbominableBlink>()
            .ActivateOnEnter<QuarrySwamp>()
            .ActivateOnEnter<BrutalSmashTB>();
    }
}
