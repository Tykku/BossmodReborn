namespace BossMod.Global.MaskedCarnivale.Stage15;

public enum OID : uint
{
    Boss = 0x26F9, // R=2.3
    Shabti = 0x26FA, //R=1.1
    Serpent = 0x26FB, //R=1.2
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 6497, // Shabti/Serpent->player, no cast, single-target

    HighVoltage = 14890, // Boss->self, 7.0s cast, range 50+R circle, paralysis + summon add
    Summon = 14897, // Boss->self, no cast, range 50 circle

    BallastVisual1 = 14893, // Boss->self, 1.0s cast, single-target
    BallastVisual2 = 14955, // Boss->self, no cast, single-target
    Ballast1 = 14894, // Helper->self, 3.0s cast, range 5+R 270-degree cone, knockback dist 15
    Ballast2 = 14895, // Helper->self, 3.0s cast, range 10+R 270-degree cone, knockback dist 15
    Ballast3 = 14896, // Helper->self, 3.0s cast, range 15+R 270-degree cone, knockback dist 15

    PiercingLaser = 14891, // Boss->self, 3.0s cast, range 30+R width 8 rect
    RepellingCannons = 14892, // Boss->self, 3.0s cast, range 10+R circle
    Spellsword = 14968, // Shabti->self, 3.5s cast, range 6+R 120-degree cone
    Superstorm = 14971, // Boss->self, 3.5s cast, single-target
    Superstorm2 = 14970, // Helper->self, 3.5s cast, range 8-20 donut
    Disseminate = 14899 // Serpent->self, 2.0s cast, range 6+R circle, casts on death of serpents
}

class HighVoltage(BossModule module) : Components.CastInterruptHint(module, ActionID.MakeSpell(AID.HighVoltage));

class Ballast(BossModule module) : Components.ConcentricAOEs(module, _shapes, true)
{
    private static readonly AOEShape[] _shapes = [new AOEShapeCone(5.5f, 135.Degrees()), new AOEShapeDonutSector(5.5f, 10.5f, 135.Degrees()), new AOEShapeDonutSector(10.5f, 15.5f, 135.Degrees())];

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.BallastVisual1)
            AddSequence(caster.Position, Module.CastFinishAt(spell, 3.6f), spell.Rotation);
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (Sequences.Count != 0)
        {
            var order = (AID)spell.Action.ID switch
            {
                AID.Ballast1 => 0,
                AID.Ballast2 => 1,
                AID.Ballast3 => 2,
                _ => -1
            };
            AdvanceSequence(order, spell.LocXZ, WorldState.FutureTime(0.6f), spell.Rotation);
        }
    }
}

class PiercingLaser(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.PiercingLaser), new AOEShapeRect(32.3f, 4));
class RepellingCannons(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.RepellingCannons), 12.3f);
class Superstorm(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Superstorm2), new AOEShapeDonut(8, 20));
class Spellsword(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Spellsword), new AOEShapeCone(7.1f, 60.Degrees()));
class Disseminate(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Disseminate), 7.2f);

class Hints(BossModule module) : BossComponent(module)
{
    public override void AddGlobalHints(GlobalHints hints)
    {
        hints.Add("For this stage Flying Sardine and Acorn Bomb are highly recommended.\nUse Flying Sardine to interrupt High Voltage.\nUse Acorn Bomb to put Shabtis to sleep until their buff runs out.");
    }
}

class Stage15States : StateMachineBuilder
{
    public Stage15States(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<HighVoltage>()
            .ActivateOnEnter<Ballast>()
            .ActivateOnEnter<PiercingLaser>()
            .ActivateOnEnter<RepellingCannons>()
            .ActivateOnEnter<Superstorm>()
            .ActivateOnEnter<Spellsword>()
            .ActivateOnEnter<Disseminate>()
            .DeactivateOnEnter<Hints>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.MaskedCarnivale, GroupID = 625, NameID = 8109)]
public class Stage15 : BossModule
{
    public Stage15(WorldState ws, Actor primary) : base(ws, primary, Layouts.ArenaCenter, Layouts.CircleSmall)
    {
        ActivateComponent<Hints>();
    }

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.Shabti), Colors.Object);
        Arena.Actors(Enemies(OID.Serpent), Colors.Object);
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        for (var i = 0; i < hints.PotentialTargets.Count; ++i)
        {
            var e = hints.PotentialTargets[i];
            e.Priority = (OID)e.Actor.OID switch
            {
                OID.Shabti => 2, //TODO: ideally AI would use Acorn Bomb to put it to sleep until buff runs out instead of attacking them directly
                OID.Serpent => 1,
                _ => 0
            };
        }
    }
}
