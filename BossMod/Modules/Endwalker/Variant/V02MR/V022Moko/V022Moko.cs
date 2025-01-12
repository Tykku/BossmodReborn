namespace BossMod.Endwalker.VariantCriterion.V02MR.V022Moko;

class AzureAuspice(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.AzureAuspice), new AOEShapeDonut(6, 60));
class BoundlessAzure(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BoundlessAzure), new AOEShapeRect(30, 5, 30));
class KenkiRelease(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.KenkiRelease));
class IronRain(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.IronRain), 10);
class Unsheathing(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Unsheathing), 3);
class VeilSever(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.VeilSever), new AOEShapeRect(40, 2.5f));
class ScarletAuspice(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.ScarletAuspice), new AOEShapeCircle(6));
class MoonlessNight(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.MoonlessNight));
class Clearout(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Clearout), new AOEShapeCone(27, 90.Degrees())); // origin is detected incorrectly, need to add 5 range to correct it
class BoundlessScarlet(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BoundlessScarlet), new AOEShapeRect(30, 5, 30));
class Explosion(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Explosion), new AOEShapeRect(30, 15, 30), 2)
{
    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var casters = ActiveCasters.ToList();
        var hasDifferentRotations = casters.Count > 1 && casters[0].Rotation != casters[1].Rotation;
        var aoes = casters.Select((c, index) =>
            new AOEInstance(Shape, c.Position, c.CastInfo!.Rotation, Module.CastFinishAt(c.CastInfo),
            index < 1 ? Colors.Danger : Colors.AOE, c.Position == casters[0].Position || hasDifferentRotations));
        return aoes;
    }
}

class GhastlyGrasp(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.GhastlyGrasp), 5);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 945, NameID = 12357, SortOrder = 2)]
public class V022MokoOtherPaths(WorldState ws, Actor primary) : BossModule(ws, primary, ArenaChange.ArenaCenter, ArenaChange.StartingBounds);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", PrimaryActorOID = (uint)OID.BossP2, GroupType = BossModuleInfo.GroupType.CFC, GroupID = 945, NameID = 12357, SortOrder = 3)]
public class V022MokoPath2(WorldState ws, Actor primary) : V022MokoOtherPaths(ws, primary);
