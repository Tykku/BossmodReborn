namespace BossMod.Dawntrail.Dungeon.D09YuweyawataFieldStation.D092OverseerKanilokka;

[ConfigDisplay(Order = 0x100, Parent = typeof(DawntrailConfig))]
public class D092OverseerKanilokkaConfig() : ConfigNode()
{
    public enum NecrohazardMechanic
    {
        [PropertyDisplay("Tank Invuln")]
        TankInvuln,

        [PropertyDisplay("Teleport hack (Always)\nWARNING: This should ONLY be used when multi-boxing!")]
        TeleportHackAlways,

        [PropertyDisplay("Teleport hack (NPC)")]
        TeleportHackNPC,
    }

    [PropertyDisplay("Necrohazard mechanic solver",
        tooltip: "Select how to solve mechanic:" +
        "\nTank Invuln - Will solve the mechanic by using the Tank's invuln ability." +
        "\nTeleport hack (Always) - Always uses teleport hacks to solve mechanic, even with other players present." +
        "\nTeleport hack (NPC) - Only Uses teleport hacks when NPC are in the party.")]
    public NecrohazardMechanic NecroHazardMechanicHints = NecrohazardMechanic.TankInvuln;
}
