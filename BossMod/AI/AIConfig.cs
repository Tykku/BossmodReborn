namespace BossMod.AI;

[ConfigDisplay(Name = "AI configuration (AI is very experimental, use at your own risk!)", Order = 7)]
sealed class AIConfig : ConfigNode
{
    [PropertyDisplay("Show status in DTR bar")]
    public bool ShowDTR = false;

    [PropertyDisplay("Show AI interface")]
    public bool DrawUI = false;

    [PropertyDisplay("Focus target master")]
    public bool FocusTargetLeader = false;

    [PropertyDisplay("Broadcast keypresses to other windows")]
    public bool BroadcastToSlaves = false;

    [PropertyDisplay("Follow party slot")]
    public int FollowSlot = 0;

    [PropertyDisplay("Forbid actions")]
    public bool ForbidActions = false;

    [PropertyDisplay("Manual targeting")]
    public bool ManualTarget = false;

    [PropertyDisplay("Forbid movement")]
    public bool ForbidMovement = false;

    [PropertyDisplay("Follow during combat")]
    public bool FollowDuringCombat = false;

    [PropertyDisplay("Follow during active boss module")]
    public bool FollowDuringActiveBossModule = false;

    [PropertyDisplay("Follow out of combat")]
    public bool FollowOutOfCombat = false;

    [PropertyDisplay("Follow target")]
    public bool FollowTarget = false;

    [PropertyDisplay("Desired positional when following target")]
    [PropertyCombo(["Any", "Flank", "Rear", "Front"])]
    public Positional DesiredPositional = Positional.Any;

    [PropertyDisplay("Max distance to slot")]
    public float MaxDistanceToSlot = 1;

    [PropertyDisplay("Max distance to target")]
    public float MaxDistanceToTarget = 2.6f;

    [PropertyDisplay("Override autorotation values", tooltip: "Uses your custom positional and distance settings instead of autorotation specific values")]
    public bool OverrideAutorotation = true;

    [PropertyDisplay("Enable auto AFK", tooltip: "Enables auto AFK if out of combat. While AFK AI will not use autorotation or target anything")]
    public bool AutoAFK = false;

    [PropertyDisplay("Auto AFK timer", tooltip: "Time in seconds out of combat until AFK mode enables. Any movement will reset timer or disable AFK mode if already active.")]
    public float AFKModeTimer = 10;

    [PropertyDisplay("Allow AI to be out of pathfinding map bounds")]
    public bool AllowAIToBeOutsideBounds = false;

    [PropertyDisplay("Movement decision delay", tooltip: "Only change this at your own risk and keep this value low! Too high and it won't move in time for some mechanics. Make sure to readjust the value for different content.")]
    public float MoveDelay = 0;

    [PropertyDisplay("Forbid AI movement while mounted")]
    public bool ForbidAIMovementMounted = false;

    public string? AIAutorotPresetName;
}
