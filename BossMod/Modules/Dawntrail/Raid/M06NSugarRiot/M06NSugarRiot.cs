using static BossMod.Dawntrail.Raid.SugarRiotSharedBounds.SugarRiotSharedBounds;

namespace BossMod.Dawntrail.Raid.M06NSugarRiot;

class SprayPain : Components.SimpleAOEs
{
    public SprayPain(BossModule module) : base(module, ActionID.MakeSpell(AID.SprayPain), 10f, 10)
    {
        MaxDangerColor = 5;
    }
}

class LightningBolt(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.LightningBolt), 4f);

abstract class ColorRiot(BossModule module, AID aid, bool showhint) : Components.BaitAwayCast(module, ActionID.MakeSpell(aid), new AOEShapeCircle(4f), true, tankbuster: showhint);
class WarmBomb(BossModule module) : ColorRiot(module, AID.WarmBomb, true);
class CoolBomb(BossModule module) : ColorRiot(module, AID.CoolBomb, false);

class MousseTouchUp(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.MousseTouchUp), 6f);
class TasteOfThunder(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.TasteOfThunder), 6f);
class TasteOfFire(BossModule module) : Components.StackWithCastTargets(module, ActionID.MakeSpell(AID.TasteOfFire), 6f, 4, 4);

class MousseMural(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.MousseMural));

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1021, NameID = 13822)]
public class M06NSugarRiot(WorldState ws, Actor primary) : BossModule(ws, primary, ArenaCenter, DefaultArena);
