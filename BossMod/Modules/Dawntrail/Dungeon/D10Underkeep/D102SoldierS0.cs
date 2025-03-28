namespace BossMod.Dawntrail.Dungeon.D10Underkeep.D102SoldierS0;

public enum OID : uint
{
    Boss = 0x47AD, // R2.76
    SoldierS0Clone = 0x47AE, // R2.76
    AddBlock = 0x47AF, // R3.2
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 870, // Boss->player, no cast, single-target
    Teleport = 42576, // Boss->location, no cast, single-target

    FieldOfScorn = 42579, // Boss->self, 5.0s cast, range 45 circle
    ThunderousSlash = 43136, // Boss->player, 5.0s cast, single-target

    SectorBisectorVisual1 = 42562, // Boss->self, 5.0s cast, single-target, cleave left
    SectorBisectorVisual2 = 42563, // Boss->self, 5.0s cast, single-target, cleave right
    SectorBisectorVisualClone1 = 42568, // SoldierS0Clone->self, no cast, single-target
    SectorBisectorVisualClone2 = 43163, // SoldierS0Clone->self, no cast, single-target
    SectorBisectorVisualClone3 = 42564, // SoldierS0Clone->self, no cast, single-target
    SectorBisectorVisualClone4 = 42569, // SoldierS0Clone->self, no cast, single-target
    SectorBisectorVisualClone5 = 43164, // SoldierS0Clone->self, no cast, single-target
    SectorBisectorVisualClone6 = 42565, // SoldierS0Clone->self, no cast, single-target
    SectorBisector1 = 42566, // Helper->self, 0.5s cast, range 45 180-degree cone, cleave left
    SectorBisector2 = 42567, // Helper->self, 0.5s cast, range 45 180-degree cone, cleave right

    OrderedFireVisual = 42572, // Boss->self, 2.0+2,0s cast, single-target
    OrderedFire = 42573, // AddBlock->self, 5.0s cast, range 55 width 8 rect
    StaticForceVisual = 42574, // Boss->self, 5.0s cast, single-target
    StaticForce = 42575, // Helper->self, no cast, range 60 30-degree cone
    ElectricExcessVisual = 42570, // Boss->self, 4.0+1,0s cast, single-target
    ElectricExcess = 43139 // Helper->players, 5.0s cast, range 6 circle, spread
}

public enum TetherID : uint
{
    BisectorInitial = 313, // SoldierS0Clone->SoldierS0Clone
    BisectorEnd = 327 // SoldierS0Clone->SoldierS0Clone
}

public enum IconID : uint
{
    StaticForce = 591 // Boss->players
}

class FieldOfScorn(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.FieldOfScorn));
class ThunderousSlash(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.ThunderousSlash));
class OrderedFire(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.OrderedFire), new AOEShapeRect(55f, 4f));
class ElectricExcess(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.ElectricExcess), 6f);
class StaticForce(BossModule module) : Components.BaitAwayIcon(module, new AOEShapeCone(60f, 15f.Degrees()), (uint)IconID.StaticForce, ActionID.MakeSpell(AID.StaticForce), 5.1f);

class SectorBisector(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCone cone = new(45f, 90f.Degrees());
    private AOEInstance? _aoe;
    private int cloneCount;
    private int tetherCount;
    private bool direction; // false = left, true = right

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(ref _aoe);

    public override void OnActorModelStateChange(Actor actor, byte modelState, byte animState1, byte animState2)
    {
        if (actor.OID == (uint)OID.SoldierS0Clone)
        {
            if (modelState == 5)
                direction = false;
            else if (modelState == 6)
                direction = true;
        }
    }

    public override void OnTethered(Actor source, ActorTetherInfo tether)
    {
        if (tether.ID == (uint)TetherID.BisectorInitial)
            ++cloneCount;
        else if (tether.ID == (uint)TetherID.BisectorEnd)
        {
            if (++tetherCount == cloneCount)
            {
                _aoe = new(cone, WPos.ClampToGrid(source.Position), source.Rotation + (direction ? -1f : 1f) * 90f.Degrees(), WorldState.FutureTime(5.1d));
                cloneCount = 0;
                tetherCount = 0;
            }
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.SectorBisector1 or (uint)AID.SectorBisector2)
            _aoe = null;
    }
}

class D102SoldierS0States : StateMachineBuilder
{
    public D102SoldierS0States(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<FieldOfScorn>()
            .ActivateOnEnter<ThunderousSlash>()
            .ActivateOnEnter<OrderedFire>()
            .ActivateOnEnter<ElectricExcess>()
            .ActivateOnEnter<StaticForce>()
            .ActivateOnEnter<SectorBisector>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1027, NameID = 13757)]
public class D102SoldierS0(WorldState ws, Actor primary) : BossModule(ws, primary, new(default, -182f), new ArenaBoundsSquare(15.5f));
