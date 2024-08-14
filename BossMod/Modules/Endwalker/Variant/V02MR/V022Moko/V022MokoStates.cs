﻿namespace BossMod.Endwalker.Variant.V02MR.V022Moko;

class V022MokoOtherPathsStates : StateMachineBuilder
{
    public V022MokoOtherPathsStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<ArenaChange>()
            //Route 1
            .ActivateOnEnter<Unsheathing>()
            .ActivateOnEnter<VeilSever>()
            //Route 2
            .ActivateOnEnter<ScarletAuspice>()
            .ActivateOnEnter<MoonlessNight>()
            .ActivateOnEnter<Clearout>()
            .ActivateOnEnter<BoundlessScarlet>()
            .ActivateOnEnter<Explosion>()
            // Route 3
            .ActivateOnEnter<YamaKagura>()
            .ActivateOnEnter<GhastlyGrasp>()
            // Route 4
            .ActivateOnEnter<Spiritflame>()
            .ActivateOnEnter<SpiritMobs>()
            //Standard
            .ActivateOnEnter<KenkiRelease>()
            .ActivateOnEnter<IronRain>()
            .ActivateOnEnter<IaiKasumiGiri1>()
            .ActivateOnEnter<IaiKasumiGiri2>()
            .ActivateOnEnter<IaiKasumiGiri3>()
            .ActivateOnEnter<IaiKasumiGiri4>()
            .ActivateOnEnter<DoubleKasumiGiri>()
            .ActivateOnEnter<AzureAuspice>()
            .ActivateOnEnter<BoundlessAzure>()
            .ActivateOnEnter<Upwell>();
    }
}

class V022MokoPath2States(BossModule module) : V022MokoOtherPathsStates(module) { }
