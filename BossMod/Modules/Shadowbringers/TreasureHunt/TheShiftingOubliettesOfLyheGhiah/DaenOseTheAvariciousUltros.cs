namespace BossMod.Shadowbringers.TreasureHunt.ShiftingOubliettesOfLyheGhiah.DaenOseTheAvariciousUltros;

public enum OID : uint
{
    Boss = 0x3030, // R0.75-5.1
    StylishTentacle = 0x3031, // R7.2
    SecretQueen = 0x3021, // R0.84, icon 5, needs to be killed in order from 1 to 5 for maximum rewards
    SecretGarlic = 0x301F, // R0.84, icon 3, needs to be killed in order from 1 to 5 for maximum rewards
    SecretTomato = 0x3020, // R0.84, icon 4, needs to be killed in order from 1 to 5 for maximum rewards
    SecretOnion = 0x301D, // R0.84, icon 1, needs to be killed in order from 1 to 5 for maximum rewards
    SecretEgg = 0x301E, // R0.84, icon 2, needs to be killed in order from 1 to 5 for maximum rewards
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 872, // Boss/SecretTomato/SecretQueen->player, no cast, single-target
    Change = 21741, // Boss->self, 6.0s cast, single-target, boss morphs into Ultros

    TentacleVisual = 21753, // Boss->self, no cast, single-target
    Tentacle = 21754, // StylishTentacle->self, 3.0s cast, range 8 circle
    Megavolt = 21752, // Boss->self, 3.5s cast, range 11 circle
    Wallop = 21755, // StylishTentacle->self, 5.0s cast, range 20 width 10 rect
    ThunderIII = 21743, // Boss->player, 4.0s cast, single-target, tankbuster

    WaveOfTurmoilVisual = 21748, // Boss->self, 5.0s cast, single-target
    WaveOfTurmoil = 21749, // Helper->self, 5.0s cast, range 40 circle, knockback 20, away from source

    SoakingSplatter = 21750, // Helper->location, 6.5s cast, range 10 circle
    AquaBreath = 21751, // Boss->self, 3.0s cast, range 13 90-degree cone

    FallingWaterVisual = 21746, // Boss->self, 5.0s cast, single-target
    FallingWater = 21747, // Helper->player, 5.0s cast, range 8 circle, spread

    WaterspoutVisual = 21744, // Boss->self, 3.0s cast, single-target
    Waterspout = 21745, // Helper->location, 3.0s cast, range 4 circle

    Pollen = 6452, // SecretQueen->self, 3.5s cast, range 6+R circle
    TearyTwirl = 6448, // SecretOnion->self, 3.5s cast, range 6+R circle
    HeirloomScream = 6451, // SecretTomato->self, 3.5s cast, range 6+R circle
    PluckAndPrune = 6449, // SecretEgg->self, 3.5s cast, range 6+R circle
    PungentPirouette = 6450, // SecretGarlic->self, 3.5s cast, range 6+R circle
    Telega = 9630 // Mandragoras->self, no cast, single-target, bonus adds disappear
}

class AquaBreath(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.AquaBreath), new AOEShapeCone(13, 45.Degrees()));
class Tentacle(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Tentacle), new AOEShapeCircle(8));
class Wallop(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Wallop), new AOEShapeRect(20, 5));
class Megavolt(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Megavolt), new AOEShapeCircle(11));
class Waterspout(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Waterspout), 4);
class SoakingSplatter(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.SoakingSplatter), 10);
class FallingWater(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.FallingWater), 8);
class ThunderIII(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.ThunderIII));

class WaveOfTurmoil(BossModule module) : Components.KnockbackFromCastTarget(module, ActionID.MakeSpell(AID.WaveOfTurmoil), 20, stopAtWall: true)
{
    private readonly SoakingSplatter _aoe = module.FindComponent<SoakingSplatter>()!;

    public override bool DestinationUnsafe(int slot, Actor actor, WPos pos) => _aoe?.ActiveAOEs(slot, actor).Any(z => z.Shape.Check(pos, z.Origin, z.Rotation)) ?? false;

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var forbidden = new List<Func<WPos, float>>();
        var source = Sources(slot, actor).FirstOrDefault();
        if (source != default)
        {
            foreach (var c in _aoe.ActiveAOEs(slot, actor))
            {
                forbidden.Add(ShapeDistance.Cone(Arena.Center, 20, Angle.FromDirection(c.Origin - Module.Center), 30.Degrees()));
            }
            if (forbidden.Count > 0)
                hints.AddForbiddenZone(p => forbidden.Min(f => f(p)), source.Activation);
        }
    }
}

abstract class Mandragoras(BossModule module, AID aid) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(aid), new AOEShapeCircle(6.84f));
class PluckAndPrune(BossModule module) : Mandragoras(module, AID.PluckAndPrune);
class TearyTwirl(BossModule module) : Mandragoras(module, AID.TearyTwirl);
class HeirloomScream(BossModule module) : Mandragoras(module, AID.HeirloomScream);
class PungentPirouette(BossModule module) : Mandragoras(module, AID.PungentPirouette);
class Pollen(BossModule module) : Mandragoras(module, AID.Pollen);

class DaenOseTheAvariciousUltrosStates : StateMachineBuilder
{
    public DaenOseTheAvariciousUltrosStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<AquaBreath>()
            .ActivateOnEnter<Tentacle>()
            .ActivateOnEnter<Wallop>()
            .ActivateOnEnter<Megavolt>()
            .ActivateOnEnter<Waterspout>()
            .ActivateOnEnter<SoakingSplatter>()
            .ActivateOnEnter<FallingWater>()
            .ActivateOnEnter<ThunderIII>()
            .ActivateOnEnter<WaveOfTurmoil>()
            .ActivateOnEnter<PluckAndPrune>()
            .ActivateOnEnter<TearyTwirl>()
            .ActivateOnEnter<HeirloomScream>()
            .ActivateOnEnter<PungentPirouette>()
            .ActivateOnEnter<Pollen>()
            .Raw.Update = () => module.Enemies(DaenOseTheAvariciousUltros.All).All(x => x.IsDeadOrDestroyed);
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 745, NameID = 9808, SortOrder = 2)]
public class DaenOseTheAvariciousUltros(WorldState ws, Actor primary) : THTemplate(ws, primary)
{
    private static readonly uint[] bonusAdds = [(uint)OID.SecretEgg, (uint)OID.SecretGarlic, (uint)OID.SecretOnion, (uint)OID.SecretTomato,
    (uint)OID.SecretQueen];
    public static readonly uint[] All = [(uint)OID.Boss, .. bonusAdds];

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(bonusAdds), Colors.Vulnerable);
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        for (var i = 0; i < hints.PotentialTargets.Count; ++i)
        {
            var e = hints.PotentialTargets[i];
            e.Priority = (OID)e.Actor.OID switch
            {
                OID.SecretOnion => 5,
                OID.SecretEgg => 4,
                OID.SecretGarlic => 3,
                OID.SecretTomato => 2,
                OID.SecretQueen => 1,
                _ => 0
            };
        }
    }
}
