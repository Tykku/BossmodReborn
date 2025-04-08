namespace BossMod.Dawntrail.Raid.M08NHowlingBlade;

class M08NHowlingBladeStates : StateMachineBuilder
{
    public M08NHowlingBladeStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<ArenaChanges>()
            .ActivateOnEnter<ExtraplanarPursuit>()
            .ActivateOnEnter<TitanicPursuit>()
            .ActivateOnEnter<GreatDivide>()
            .ActivateOnEnter<Heavensearth1>()
            .ActivateOnEnter<Heavensearth2>()
            .ActivateOnEnter<WolvesReignRect1>()
            .ActivateOnEnter<WolvesReignRect2>()
            .ActivateOnEnter<WolvesReignCone>()
            .ActivateOnEnter<WolvesReignCircle>()
            .ActivateOnEnter<MoonbeamsBite>()
            .ActivateOnEnter<Shadowchase1>()
            .ActivateOnEnter<Shadowchase2>()
            .ActivateOnEnter<TargetedQuake>()
            .ActivateOnEnter<FangedCharge>()
            .ActivateOnEnter<TerrestrialTitans>()
            .ActivateOnEnter<Towerfall>()
            .ActivateOnEnter<TrackingTremors>()
            .ActivateOnEnter<RavenousSaber>()
            .ActivateOnEnter<RoaringWind>()
            .ActivateOnEnter<Gust>()
            .ActivateOnEnter<GrowlingWindWealofStone>();
    }
}
