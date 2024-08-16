namespace BossMod.Endwalker.Variant.V02MR.V022Moko;

class AzureAuspice(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.AzureAuspice), new AOEShapeDonut(6, 60));
class BoundlessAzure(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BoundlessAzureAOE), new AOEShapeRect(60, 5, 60));
class KenkiRelease(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.KenkiRelease));
class IronRain(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.IronRain), 10);

//Route 1
class Unsheathing(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.Unsheathing), 3);
class VeilSever(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.VeilSever), new AOEShapeRect(40, 2.5f));

//Route 2
class ScarletAuspice(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.ScarletAuspice), new AOEShapeCircle(6));
class MoonlessNight(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.MoonlessNight));
class Clearout(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Clearout), new AOEShapeCone(27, 90.Degrees())); // origin is detected incorrectly, need to add 5 range to correct it
class BoundlessScarlet(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BoundlessScarlet), new AOEShapeRect(60, 5, 60));
class Explosion(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Explosion), new AOEShapeRect(60, 15, 60), 2)
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

//Route 3
class GhastlyGrasp(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.GhastlyGrasp), 5);

class YamaKagura(BossModule module) : Components.Knockback(module)
{
    private readonly List<Actor> _casters = [];
    private DateTime _activation;
    private static readonly AOEShapeRect rect = new(40, 5, 40);

    public override IEnumerable<Source> Sources(int slot, Actor actor)
    {
        foreach (var c in _casters)
            yield return new(c.Position, 33, _activation, rect, c.Rotation - 90.Degrees(), Kind.DirLeft);
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.YamaKagura)
        {
            _activation = Module.CastFinishAt(spell);
            _casters.Add(caster);
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.YamaKagura)
            _casters.Remove(caster);
    }

    public override bool DestinationUnsafe(int slot, Actor actor, WPos pos) => (Module.FindComponent<GhastlyGrasp>()?.ActiveAOEs(slot, actor).Any(z => z.Shape.Check(pos, z.Origin, z.Rotation)) ?? false) || !Module.InBounds(pos);
}

//Route 4
class Spiritflame(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.Spiritflame), 6);
class SpiritMobs(BossModule module) : Components.GenericAOEs(module)
{
    private readonly IReadOnlyList<Actor> _spirits = module.Enemies(OID.Spiritflame);

    private static readonly AOEShapeCircle _shape = new(2);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _spirits.Where(actor => !actor.IsDead).Select(b => new AOEInstance(_shape, b.Position));
}

[ModuleInfo(BossModuleInfo.Maturity.Contributed, Contributors = "The Combat Reborn Team", PrimaryActorOID = (uint)OID.Boss, GroupType = BossModuleInfo.GroupType.CFC, Category = BossModuleInfo.Category.Criterion, GroupID = 945, NameID = 12357, SortOrder = 2)]
public class V022MokoOtherPaths(WorldState ws, Actor primary) : BossModule(ws, primary, ArenaChange.ArenaCenter, ArenaChange.StartingBounds);

[ModuleInfo(BossModuleInfo.Maturity.Contributed, Contributors = "The Combat Reborn Team", PrimaryActorOID = (uint)OID.BossP2, GroupType = BossModuleInfo.GroupType.CFC, Category = BossModuleInfo.Category.Criterion, GroupID = 945, NameID = 12357, SortOrder = 3)]
public class V022MokoPath2(WorldState ws, Actor primary) : BossModule(ws, primary, ArenaChange.ArenaCenter, ArenaChange.StartingBounds);
