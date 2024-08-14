namespace BossMod;

[ConfigDisplay(Name = "Color scheme", Order = -1)]
public sealed class ColorConfig : ConfigNode
{
    [PropertyDisplay("Arena: background")]
    public Color ArenaBackground = new(0xc00f0f0f);

    [PropertyDisplay("Arena: border")]
    public Color ArenaBorder = new(0xffffffff);

    [PropertyDisplay("Arena: AOE")]
    public Color ArenaAOE = new(0x80008080);

    [PropertyDisplay("Arena: safe from AOE")]
    public Color ArenaSafeFromAOE = new(0x80008000);

    [PropertyDisplay("Arena: danger")]
    public Color ArenaDanger = new(0xff00ffff);

    [PropertyDisplay("Arena: safe")]
    public Color ArenaSafe = new(0xff00ff00);

    [PropertyDisplay("Arena: trap")]
    public Color ArenaTrap = new(0x80000080);

    [PropertyDisplay("Arena: player character")]
    public Color ArenaPC = new(0xff00ff00);

    [PropertyDisplay("Arena: enemy")]
    public Color ArenaEnemy = new(0xff0000ff);

    [PropertyDisplay("Arena: object")]
    public Color ArenaObject = new(0xff0080ff);

    [PropertyDisplay("Arena: player interesting")]
    public Color ArenaPlayerInteresting = new(0xffc0c0c0);

    [PropertyDisplay("Arena: player generic")]
    public Color ArenaPlayerGeneric = new(0xff808080);

    [PropertyDisplay("Arena: vulnerable")]
    public Color ArenaVulnerable = new(0xffff00ff);

    [PropertyDisplay("Arena: future vulnerable")]
    public Color ArenaFutureVulnerable = new(0x80ff00ff);
    [PropertyDisplay("Arena: melee range indicator")]
    public Color ArenaMeleeRangeIndicator = new(0xffff0000);
    [PropertyDisplay("Arena: other 1")]
    public Color ArenaOther1 = new(0xffff0080);
    [PropertyDisplay("Arena: other 2")]
    public Color ArenaOther2 = new(0xff8080ff);

    [PropertyDisplay("Outlines and shadows")]
    public Color Shadows = new(0xFF000000);

    [PropertyDisplay("Waymark: A")]
    public Color WaymarkA = new(0xff964ee5);

    [PropertyDisplay("Waymark: B")]
    public Color WaymarkB = new(0xff11a2c6);

    [PropertyDisplay("Waymark: C")]
    public Color WaymarkC = new(0xffe29f30);

    [PropertyDisplay("Waymark: D")]
    public Color WaymarkD = new(0xffbc567a);

    [PropertyDisplay("Waymark: 1")]
    public Color Waymark1 = new(0xff964ee5);

    [PropertyDisplay("Waymark: 2")]
    public Color Waymark2 = new(0xff11a2c6);

    [PropertyDisplay("Waymark: 3")]
    public Color Waymark3 = new(0xffe29f30);

    [PropertyDisplay("Waymark: 4")]
    public Color Waymark4 = new(0xffbc567a);
    [PropertyDisplay("Positional colors")]
    public Color[] PositionalColors = [new(0xff00ff00), new(0xff0000ff), new(0xffffffff),
     new(0xff00ffff)];

    [PropertyDisplay("Planner: background")]
    public Color PlannerBackground = new(0x80362b00);

    [PropertyDisplay("Planner: background highlight")]
    public Color PlannerBackgroundHighlight = new(0x80423607);

    [PropertyDisplay("Planner: cooldown")]
    public Color PlannerCooldown = new(0x80756e58);

    [PropertyDisplay("Planner: fallback color for options")]
    public Color PlannerFallback = new(0x80969483);

    [PropertyDisplay("Planner: effect")]
    public Color PlannerEffect = new(0x8000ff00);

    [PropertyDisplay("Planner: window")]
    public Color[] PlannerWindow = [new(0x800089b5), new(0x80164bcb), new(0x802f32dc), new(0x808236d3), new(0x80c4716c), new(0x80d28b26), new(0x8098a12a), new(0x80009985)];

    [PropertyDisplay("Button push color 1")]
    public Color ButtonPushColor1 = new(0xff000080);

    [PropertyDisplay("Button push color 2")]
    public Color ButtonPushColor2 = new(0xff008080);

    [PropertyDisplay("Text colors")]
    public Color[] TextColors = [new(0xffffffff), new(0xff00ffff), new(0xff0000ff),
     new(0xff00ff00), new(0xff0080ff), new(0xffff00ff), new(0x80808080), new(0x80800080),
     new(0x80ffffff), new(0x8000ff00), new(0xffffff00), new(0x800000ff), new(0xff404040),
     new(0xffff0000), new(0xff000000)];

    public static ColorConfig DefaultConfig => new();
}
