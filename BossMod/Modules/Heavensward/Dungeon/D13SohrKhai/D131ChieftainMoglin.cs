namespace BossMod.Heavensward.Dungeon.D13SohrKhai.D131ChieftainMoglin;

public enum OID : uint
{
    Boss = 0x15FB, // R1.8
    PomguardPomchopper = 0x15FD, // R0.9
    PomguardPompincher = 0x1602, // R0.9
    CaptainMogsun = 0x15FC, // R0.9
    DemoniacalMogcane = 0x1603, // R5.0
    PomguardPomcrier = 0x1601, // R0.9
    PomguardPompiercer = 0x15FF, // R0.9
    PomguardPomfryer = 0x1600, // R0.9
    PomguardPomfluffer = 0x15FE, // R0.9
    Helper = 0x1B2
}

public enum AID : uint
{
    AutoAttack1 = 872, // Boss/PomguardPomfryer/PomguardPomcrier->player, no cast, single-target
    AutoAttack2 = 870, // PomguardPompincher/CaptainMogsun->player, no cast, single-target

    ThousandKuponzeCharge = 6019, // Boss->self, no cast, range 8+R 120-degree cone
    HundredKuponzeSwipe = 4429, // PomguardPomchopper->self, 3.0s cast, range 20+R 90-degree cone
    PoisonNeedle = 4432, // PomguardPompincher->player, no cast, single-target
    PomHoly = 6020, // Boss->location, 3.0s cast, range 50 circle, raidwide
    DemoniacalMogcane = 6017, // Boss->self, 2.0s cast, single-target
    PomBomVisual = 6018, // DemoniacalMogcane->self, no cast, single-target
    PomBom = 6212, // Helper->self, no cast, range 40+R width 4 rect
    MoogleEyeShot = 4427, // PomguardPompiercer->player, no cast, single-target

    MarchOfTheMoogles = 4431, // PomguardPomcrier->self, 5.0s cast, range 10 circle, apply damage buff to affected moogles
    PomPraiseVisual = 6015, // Boss->self, 13.0s cast, single-target
    PomPraise = 6016, // Helper->self, 13.0s cast, range 4 circle, raise moogle + apply HP buff to affected moogles
    SpinningMogshield = 4425, // CaptainMogsun->self, 3.0s cast, range 6+R circle
    PomCure = 4426, // PomguardPomfluffer->Boss, 3.0s cast, single-target
    PomFlare = 4428 // PomguardPomfryer->self, 6.0s cast, range 21+R circle
}

public enum SID : uint
{
    Invincibility = 325, // none->Boss, extra=0x0
    OffBalance = 1064 // none->PomguardPomchopper/PomguardPompincher/CaptainMogsun/PomguardPomfluffer, extra=0x0
}

class PomBom(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCross cross = new(40.5f, 2);
    private AOEInstance? _aoe;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID == OID.DemoniacalMogcane)
            _aoe = new(cross, actor.Position, default, WorldState.FutureTime(6.3f));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.PomBom)
            _aoe = null;
    }
}

class ThousandKuponzeCharge(BossModule module) : Components.Cleave(module, ActionID.MakeSpell(AID.ThousandKuponzeCharge), new AOEShapeCone(9.8f, 60.Degrees()))
{
    private bool active = true; // cleave happens near the start of the fight then again after every demoniacal mogcane cast

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.DemoniacalMogcane)
            active = true;
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.ThousandKuponzeCharge)
            active = false;
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (active)
            base.AddHints(slot, actor, hints);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (active)
            base.AddAIHints(slot, actor, assignment, hints);
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        if (active)
            base.DrawArenaForeground(pcSlot, pc);
    }
}

class PomPraise(BossModule module) : BossComponent(module)
{
    private readonly List<WPos> positions = [];
    public IEnumerable<Actor> RelevantMoogles = [];

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.PomPraise)
            positions.Add(spell.LocXZ);
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.PomPraiseVisual)
            positions.Clear();
    }

    public override void Update()
    {
        if (positions.Count != 0) // it is assumed that the moogle hitbox must not intersect the circle, as usual for NPCs
            RelevantMoogles = Module.Enemies(D131ChieftainMoglin.SmallMoogles).Where(e => e.FindStatus(SID.OffBalance) != null && positions.Any(p => e.Position.InCircle(p, 4.9f)));
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (positions.Count != 0)
        {
            foreach (var r in RelevantMoogles)
            {
                var target = hints.PotentialTargets.FirstOrDefault(x => x.Actor == r);
                if (target != null)
                {
                    target.Priority = 3;
                }
            }
        }
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (RelevantMoogles.Any())
            hints.Add("Push defeated moogles out of circles!");
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        if (RelevantMoogles.Any())
            for (var i = 0; i < positions.Count; ++i)
                Arena.AddCircle(positions[i], 4.9f, Colors.Vulnerable);
    }
}

class HundredKuponzeSwipe(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.HundredKuponzeSwipe), new AOEShapeCone(20.9f, 45.Degrees()));
class PomFlare(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.PomFlare), 20.9f);
class PomHoly(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.PomHoly));
class SpinningMogshield(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.SpinningMogshield), 6.9f);

class D131ChieftainMoglinStates : StateMachineBuilder
{
    public D131ChieftainMoglinStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<PomBom>()
            .ActivateOnEnter<ThousandKuponzeCharge>()
            .ActivateOnEnter<HundredKuponzeSwipe>()
            .ActivateOnEnter<PomFlare>()
            .ActivateOnEnter<PomHoly>()
            .ActivateOnEnter<SpinningMogshield>()
            .ActivateOnEnter<PomPraise>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 171, NameID = 4943, SortOrder = 2)]
public class D131ChieftainMoglin(WorldState ws, Actor primary) : BossModule(ws, primary, arena.Center, arena)
{
    private static readonly ArenaBoundsComplex arena = new([new Polygon(new(-400, -158.04f), 19.5f * CosPI.Pi60th, 64)], [new Rectangle(new(-400, -138), 20, 1.05f),
    new Rectangle(new(-400, -178), 20, 0.8f)]);

    public static readonly uint[] SmallMoogles = [(uint)OID.CaptainMogsun, (uint)OID.PomguardPomfluffer, (uint)OID.PomguardPomfryer, (uint)OID.PomguardPompincher,
    (uint)OID.PomguardPomchopper, (uint)OID.PomguardPompiercer, (uint)OID.PomguardPomcrier];

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        if (PrimaryActor.FindStatus(SID.Invincibility) == null)
            Arena.Actor(PrimaryActor);
        var smallmoogles = Enemies(SmallMoogles);
        Arena.Actors(smallmoogles.Where(x => x.FindStatus(SID.OffBalance) == null));
        Arena.Actors(smallmoogles.Where(x => x.FindStatus(SID.OffBalance) != null), Colors.Vulnerable);
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (!FindComponent<PomPraise>()!.RelevantMoogles.Any())
            for (var i = 0; i < hints.PotentialTargets.Count; ++i)
            {
                var e = hints.PotentialTargets[i];
                if (e.Actor.FindStatus(SID.Invincibility) != null)
                {
                    e.Priority = AIHints.Enemy.PriorityInvincible;
                    continue;
                }
                e.Priority = (OID)e.Actor.OID switch
                {
                    OID.CaptainMogsun => 2,
                    OID.PomguardPomfluffer or OID.PomguardPomfryer or OID.PomguardPomcrier or OID.PomguardPompincher or OID.PomguardPompiercer or OID.PomguardPomchopper => 1,
                    _ => 0
                };
            }
    }
}
