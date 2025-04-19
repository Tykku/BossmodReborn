using static BossMod.Dawntrail.Raid.SugarRiotSharedBounds.SugarRiotSharedBounds;

namespace BossMod.Dawntrail.Raid.M06NSugarRiot;

class SprayPain : Components.SimpleAOEs
{
    public SprayPain(BossModule module) : base(module, (uint)AID.SprayPain, 10f, 10)
    {
        MaxDangerColor = 5;
    }
}

class LightningBolt(BossModule module) : Components.SimpleAOEs(module, (uint)AID.LightningBolt, 4f);

abstract class ColorRiot(BossModule module, uint aid, bool showhint) : Components.BaitAwayCast(module, aid, new AOEShapeCircle(4f), true, tankbuster: showhint);
class WarmBomb(BossModule module) : ColorRiot(module, (uint)AID.WarmBomb, true);
class CoolBomb(BossModule module) : ColorRiot(module, (uint)AID.CoolBomb, false);

class MousseTouchUp(BossModule module) : Components.SpreadFromCastTargets(module, (uint)AID.MousseTouchUp, 6f);
class TasteOfThunder(BossModule module) : Components.SpreadFromCastTargets(module, (uint)AID.TasteOfThunder, 6f);
class TasteOfFire(BossModule module) : Components.StackWithCastTargets(module, (uint)AID.TasteOfFire, 6f, 4, 4);

class MousseMural(BossModule module) : Components.RaidwideCast(module, (uint)AID.MousseMural);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1021, NameID = 13822)]
public class M06NSugarRiot(WorldState ws, Actor primary) : BossModule(ws, primary, ArenaCenter, DefaultArena);
