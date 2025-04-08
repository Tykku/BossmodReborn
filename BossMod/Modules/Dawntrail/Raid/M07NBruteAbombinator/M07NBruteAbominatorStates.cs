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
            .ActivateOnEnter<BrutishSwingCone1>()
            .ActivateOnEnter<BrutishSwingCone2>()
            .ActivateOnEnter<BrutishSwingDonutSegment1>()
            .ActivateOnEnter<BrutishSwingDonutSegment2>()
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
            .ActivateOnEnter<LashingLariat1>()
            .ActivateOnEnter<LashingLariat2>()
            .ActivateOnEnter<Slaminator>()
            .ActivateOnEnter<PulpSmash>()
            .ActivateOnEnter<Sporesplosion>()
            .ActivateOnEnter<AbominableBlink>()
            .ActivateOnEnter<QuarrySwamp>()
            .ActivateOnEnter<BrutalSmashTB>();
    }
}
