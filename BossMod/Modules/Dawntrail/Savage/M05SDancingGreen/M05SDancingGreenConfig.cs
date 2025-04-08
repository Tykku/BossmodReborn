namespace BossMod.Dawntrail.Savage.M05SDancingGreen;

[ConfigDisplay(Order = 0x100, Parent = typeof(DawntrailConfig))]
public class M05SDancingGreenConfig() : ConfigNode()
{
    [PropertyDisplay("Draw moving exaflare pattern", tooltip: "If disabled the full Ride the Waves exaflare pattern is drawn from the beginning. Otherwise it will move like in normal mode.")]
    public bool MovingExaflares = true;
}
