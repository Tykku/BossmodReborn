namespace BossMod.Endwalker.Variant.V02MR.V021Yozakura;

class GloryNeverlasting(BossModule module) : Components.SingleTargetDelayableCast(module, ActionID.MakeSpell(AID.GloryNeverlasting));
class ArtOfTheFireblossom(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.ArtOfTheFireblossom), new AOEShapeCircle(9));
class ArtOfTheWindblossom(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.ArtOfTheWindblossom), new AOEShapeDonut(5, 60));
class KugeRantsui(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.KugeRantsui));
class OkaRanman(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.OkaRanman));

//Left Windy
class LevinblossomStrike(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.LevinblossomStrike), 3);
class DriftingPetals(BossModule module) : Components.KnockbackFromCastTarget(module, ActionID.MakeSpell(AID.DriftingPetals), 15, ignoreImmunes: true);

//Left Rainy
class Mudrain(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.Mudrain), 5); //persistent AOE, needs to be handled differently
class Icebloom(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.Icebloom), 6);
class ShadowflightAOE(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.ShadowflightAOE), new AOEShapeRect(10, 3));
class MudPieAOE(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.MudPieAOE), new AOEShapeRect(60, 3));

//Middle Rope Pulled
class FireblossomFlare(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.FireblossomFlare), 6);
class ArtOfTheFluff1(BossModule module) : Components.CastGaze(module, ActionID.MakeSpell(AID.ArtOfTheFluff1));
class ArtOfTheFluff2(BossModule module) : Components.CastGaze(module, ActionID.MakeSpell(AID.ArtOfTheFluff2));

//Middle Rope Unpulled
class TatamiGaeshiAOE(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.RockRootArrangementVisual), new AOEShapeCircle(4));

//Right No Dogu
class RootArrangement : Components.StandardChasingAOEs
{
    public RootArrangement(BossModule module) : base(module, new AOEShapeCircle(4), ActionID.MakeSpell(AID.RockRootArrangementFirst), ActionID.MakeSpell(AID.RockRootArrangementRest), 4, 1, 4)
    {
        ExcludedTargets = Raid.WithSlot(true).Mask();
    }

    public override void OnEventIcon(Actor actor, uint iconID)
    {
        if (iconID == (uint)IconID.RootArrangement)
            ExcludedTargets.Clear(Raid.FindSlot(actor.InstanceID));
    }
}

//Right Dogu

class Witherwind(BossModule module) : Components.GenericAOEs(module)
{
    private readonly IReadOnlyList<Actor> _spirits = module.Enemies(OID.AutumnalTempest);

    private static readonly AOEShapeCircle _shape = new(3);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _spirits.Where(actor => !actor.IsDead).Select(b => new AOEInstance(_shape, b.Position));
}

[ModuleInfo(BossModuleInfo.Maturity.Contributed, Contributors = "The Combat Reborn Team", GroupType = BossModuleInfo.GroupType.CFC, Category = BossModuleInfo.Category.Criterion, GroupID = 945, NameID = 12325, SortOrder = 1)]
public class V021Yozakura(WorldState ws, Actor primary) : BossModule(ws, primary, primary.Position.X < -700 ? ArenaCenter1 : primary.Position.X > 700 ? ArenaCenter2 : ArenaCenter3, StartingBounds)
{
    public static readonly WPos ArenaCenter1 = new(-775, 16);
    public static readonly WPos ArenaCenter2 = new(737, 220);
    public static readonly WPos ArenaCenter3 = new(47, 93);
    public static readonly ArenaBoundsSquare StartingBounds = new(22.5f);
    public static readonly ArenaBoundsSquare DefaultBounds = new(20);
}
