namespace BossMod.Endwalker.Variant.V02MR.V021Yozakura;

class V021YozakuraStates : StateMachineBuilder
{
    public V021YozakuraStates(BossModule module) : base(module)
    {
        TrivialPhase()
            //Right No Dogu
            .ActivateOnEnter<RootArrangement>()
            //Right Dogu
            .ActivateOnEnter<Witherwind>()
            //Left Windy
            .ActivateOnEnter<WindblossomWhirl>()
            .ActivateOnEnter<LevinblossomStrike>()
            .ActivateOnEnter<DriftingPetals>()
            //Left Rainy
            .ActivateOnEnter<Mudrain>()
            .ActivateOnEnter<Icebloom>()
            .ActivateOnEnter<ShadowflightAOE>()
            .ActivateOnEnter<MudPieAOE>()
            //Middle Rope Pulled
            .ActivateOnEnter<FireblossomFlare>()
            .ActivateOnEnter<ArtOfTheFluff1>()
            .ActivateOnEnter<ArtOfTheFluff2>()
            //Middle Rope Unpulled
            .ActivateOnEnter<LevinblossomLance>()
            .ActivateOnEnter<TatamiGaeshiAOE>()
            //Standard
            .ActivateOnEnter<ArenaChange>()
            .ActivateOnEnter<Components.StayInBounds>()
            .ActivateOnEnter<GloryNeverlasting>()
            .ActivateOnEnter<KugeRantsui>()
            .ActivateOnEnter<OkaRanman>()
            .ActivateOnEnter<SealOfRiotousBloom>()
            .ActivateOnEnter<SeasonsOfTheFleeting>()
            .ActivateOnEnter<ArtOfTheFireblossom>()
            .ActivateOnEnter<ArtOfTheWindblossom>();
    }
}
