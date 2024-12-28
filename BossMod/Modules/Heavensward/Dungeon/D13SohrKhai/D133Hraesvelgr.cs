namespace BossMod.Heavensward.Dungeon.D13SohrKhai.D133Hraesvelgr;

public enum OID : uint
{
    Boss = 0x3D17, // R19.0
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 32142, // Boss->player, no cast, single-target

    Wyrmclaw = 32141, // Boss->player, 5.0s cast, single-target, tankbuster
    HallowedWings1 = 32136, // Boss->self, 6.0s cast, range 50 width 22 rect, left
    HallowedWings2 = 32137, // Boss->self, 6.0s cast, range 50 width 22 rect, right
    HolyStorm = 32127, // Boss->self, 5.0s cast, range 40 circle, raidwide
    HallowedDive = 32131, // Boss->self, 6.0s cast, range 40 width 20 rect
    FrigidDive = 32134, // Boss->self, 6.0s cast, range 40 width 20 rect
    HolyOrbFirst = 32129, // Helper->self, 5.0s cast, range 6 circle, exaflares
    HolyOrbRest = 32130, // Helper->self, no cast, range 6 circle
    AkhMornFirst = 32132, // Boss->players, 5.0s cast, range 6 circle, stack 5 (first) or 6 hits
    AkhMornRest = 32133, // Boss->players, no cast, range 6 circle
    HolyBreathVisual = 32138, // Boss->self, 5.0+1,0s cast, single-target, spread
    HolyBreath = 32139, // Helper->player, 6.0s cast, range 6 circle
    DiamondStorm = 32128, // Boss->self, 5.0s cast, range 40 circle, raidwide
    FrostedOrb = 32135 // Helper->self, 5.0s cast, range 6 circle
}

public enum IconID : uint
{
    Spreadmarker = 311 // player->self
}

abstract class HallowedWings(BossModule module, AID aid) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(aid), new AOEShapeRect(50, 11));
class HallowedWings1(BossModule module) : HallowedWings(module, AID.HallowedWings1);
class HallowedWings2(BossModule module) : HallowedWings(module, AID.HallowedWings2);

class Wyrmclaw(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.Wyrmclaw));
class HolyStorm(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.HolyStorm));
class DiamondStorm(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.DiamondStorm));

abstract class Dive(BossModule module, AID aid) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(aid), new AOEShapeRect(40, 10));
class HallowedDive(BossModule module) : Dive(module, AID.HallowedDive);
class FrigidDive(BossModule module) : Dive(module, AID.FrigidDive);

class FrostedOrb(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.FrostedOrb), new AOEShapeCircle(6));

class AkhMorn(BossModule module) : Components.UniformStackSpread(module, 6, 0, 4, 4)
{
    private int numCasts;
    private bool first = true;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.AkhMornFirst)
            AddStack(WorldState.Actors.Find(spell.TargetID)!, Module.CastFinishAt(spell));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.AkhMornFirst or AID.AkhMornRest)
        {
            ++numCasts;
            if (first && numCasts == 5 || numCasts == 6)
            {
                Stacks.Clear();
                numCasts = 0;
                first = false;
            }
        }
    }
}

class HolyOrb(BossModule module) : Components.Exaflare(module, new AOEShapeCircle(6))
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.HolyOrbFirst)
            Lines.Add(new() { Next = spell.LocXZ, Advance = caster.Rotation.ToDirection() * 6.8f, NextExplosion = Module.CastFinishAt(spell), TimeToMove = 1, ExplosionsLeft = 5, MaxShownExplosions = 3 });
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.HolyOrbFirst or AID.HolyOrbRest)
        {
            var index = Lines.FindIndex(item => item.Next.AlmostEqual(caster.Position, 1));
            if (index < 0)
                return;
            AdvanceLine(Lines[index], caster.Position);
            if (Lines[index].ExplosionsLeft == 0)
                Lines.RemoveAt(index);
        }
    }
}

class HolyBreath(BossModule module) : Components.SpreadFromIcon(module, (uint)IconID.Spreadmarker, ActionID.MakeSpell(AID.HolyBreath), 6, 6);

class ThinIce(BossModule module) : Components.ThinIce(module, 11, true, stopAtWall: true)
{
    public override bool DestinationUnsafe(int slot, Actor actor, WPos pos) => (Module.FindComponent<FrostedOrb>()?.ActiveAOEs(slot, actor).Any(z => z.Shape.Check(pos, z.Origin, z.Rotation)) ?? false) ||
    (Module.FindComponent<FrigidDive>()?.ActiveAOEs(slot, actor).Any(z => z.Shape.Check(pos, z.Origin, z.Rotation)) ?? false);
}

class D133HraesvelgrStates : StateMachineBuilder
{
    public D133HraesvelgrStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<HallowedWings1>()
            .ActivateOnEnter<HallowedWings2>()
            .ActivateOnEnter<Wyrmclaw>()
            .ActivateOnEnter<HolyStorm>()
            .ActivateOnEnter<DiamondStorm>()
            .ActivateOnEnter<HallowedDive>()
            .ActivateOnEnter<FrigidDive>()
            .ActivateOnEnter<FrostedOrb>()
            .ActivateOnEnter<AkhMorn>()
            .ActivateOnEnter<HolyOrb>()
            .ActivateOnEnter<HolyBreath>()
            .ActivateOnEnter<ThinIce>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 171, NameID = 4954, SortOrder = 6)]
public class D133Hraesvelgr(WorldState ws, Actor primary) : BossModule(ws, primary, arena.Center, arena)
{
    private static readonly ArenaBoundsComplex arena = new([new Polygon(new(400, -400), 19.5f, 36)]);
}