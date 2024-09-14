namespace BossMod.Heavensward.Dungeon.D05GreatGubalLibrary.D052Byblos;

public enum OID : uint
{
    Boss = 0xE83, // R3.0
    Page64 = 0x10B7, // R1.2
    WhaleOil = 0x10C4, // R2.0
    Tomewind = 0x10C5, // R1.0
    Page64BooksVisual = 0x1E99EE, // R0.5
    BossBookVisual = 0x1E996A // R0.5
}

public enum AID : uint
{
    AutoAttack = 870, // Boss->player, no cast, single-target
    PageTear = 4159, // Boss->self, no cast, range 5+R ?-degree cone
    HeadDown = 4163, // Boss->player, 4.0s cast, width 8 rect charge
    BoneShaker = 4164, // Boss->self, no cast, range 50+R circle
    DeathRay = 5058, // Page64->self, 4.0s cast, range 23+R width 3 rect
    Bibliocide = 4167, // WhaleOil->self, no cast, range 3 circle
    GaleCut = 4158, // Boss->self, 3.0s cast, single-target, spawns Tomewinds
    VacuumBlade = 4168, // TomeWind->self, no cast, range 3 circle, touched Tomewind
    TailSmash = 4165 // Boss->self, 2.5s cast, range 9+R 90-degree cone
}

public enum TetherID : uint
{
    WhaleOil = 3
}

public enum SID : uint
{
    Invincibility = 325 // none->Boss, extra=0x0
}

class PageTear(BossModule module) : Components.Cleave(module, ActionID.MakeSpell(AID.PageTear), new AOEShapeCone(8, 45.Degrees()))
{
    public static bool IsInvincible(Actor actor) => actor.FindStatus(SID.Invincibility) != null;

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (!IsInvincible(Module.PrimaryActor))
            base.AddHints(slot, actor, hints);
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (!IsInvincible(Module.PrimaryActor))
            base.AddAIHints(slot, actor, assignment, hints);
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        if (!IsInvincible(Module.PrimaryActor))
            base.DrawArenaForeground(pcSlot, pc);
    }
}

class HeadDown(BossModule module) : Components.BaitAwayChargeCast(module, ActionID.MakeSpell(AID.HeadDown), 4);
class DeathRay(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.DeathRay), new AOEShapeRect(24.2f, 1.5f));
class Tomewind(BossModule module) : Components.PersistentVoidzone(module, 3, m => m.Enemies(OID.Tomewind).Where(x => !x.IsDead));
class TailSmash(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.TailSmash), new AOEShapeCone(12, 45.Degrees()));

class Bibliocide(BossModule module) : Components.BaitAwayTethers(module, new AOEShapeCircle(0), (uint)TetherID.WhaleOil, activationDelay: 5)
{
    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (!ActiveBaits.Any(x => x.Target == actor))
            return;
        hints.Add("Pull the orb to the boss!");
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (ActiveBaits.Any(x => x.Target == actor))
            hints.AddForbiddenZone(ShapeDistance.InvertedCircle(Module.PrimaryActor.Position, Module.PrimaryActor.HitboxRadius), ActiveBaits.FirstOrDefault().Activation);
    }
}

class D052ByblosStates : StateMachineBuilder
{
    public D052ByblosStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<PageTear>()
            .ActivateOnEnter<HeadDown>()
            .ActivateOnEnter<TailSmash>()
            .ActivateOnEnter<DeathRay>()
            .ActivateOnEnter<Tomewind>()
            .ActivateOnEnter<Bibliocide>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 31, NameID = 3925)]
public class D052Byblos(WorldState ws, Actor primary) : BossModule(ws, primary, arena.Center, arena)
{
    private static readonly List<WPos> vertices = [new(182.26f, 3.32f), new(182.73f, 3.56f), new(183.19f, 3.90f), new(183.75f, 3.97f), new(184.83f, 3.93f),
    new(185.39f, 3.94f), new(186.84f, 4.77f), new(187.35f, 4.97f), new(187.86f, 5.11f), new(188.42f, 5.24f),
    new(188.97f, 5.45f), new(190.39f, 6.58f), new(191.87f, 7.27f), new(192.3f, 7.56f), new(193.11f, 8.43f),
    new(193.52f, 8.86f), new(195, 9.83f), new(195.3f, 10.24f), new(195.94f, 11.22f), new(196.26f, 11.63f),
    new(197.51f, 12.78f), new(198.19f, 14.29f), new(198.47f, 14.71f), new(199.54f, 16.05f), new(199.97f, 17.64f),
    new(200.16f, 18.14f), new(200.77f, 19.2f), new(200.99f, 19.68f), new(201.11f, 20.75f), new(201.2f, 21.28f),
    new(201.32f, 21.79f), new(201.89f, 23.34f), new(201.8f, 25.04f), new(201.87f, 25.56f), new(202.12f, 26.73f),
    new(202.16f, 27.25f), new(201.83f, 28.83f), new(201.82f, 29.37f), new(201.88f, 30.56f), new(201.85f, 31.06f),
    new(201.23f, 32.68f), new(201.06f, 34.28f), new(200.9f, 34.84f), new(200.02f, 36.35f), new(199.64f, 37.84f),
    new(199.45f, 38.33f), new(198.42f, 39.59f), new(198.14f, 40.07f), new(197.67f, 41.13f), new(197.42f, 41.56f),
    new(196.24f, 42.62f), new(195.91f, 43.04f), new(195.33f, 43.96f), new(194.95f, 44.44f), new(193.52f, 45.37f),
    new(193.12f, 45.78f), new(192.34f, 46.65f), new(191.86f, 46.99f), new(190.44f, 47.63f), new(190.03f, 47.92f),
    new(189.22f, 48.58f), new(188.75f, 48.89f), new(187.09f, 49.33f), new(185.59f, 50.19f), new(185.02f, 50.31f),
    new(183.39f, 50.25f), new(182.89f, 50.56f), new(182.4f, 50.92f), new(181.39f, 50.94f), new(173.37f, 50.97f),
    new(172.93f, 50.66f), new(172.48f, 50.39f), new(171.93f, 50.26f), new(170.21f, 50.33f), new(168.68f, 49.44f),
    new(168.17f, 49.25f), new(167.17f, 49.01f), new(166.66f, 48.84f), new(165.43f, 47.84f), new(164.9f, 47.52f),
    new(163.83f, 47.04f), new(163.35f, 46.77f), new(162.21f, 45.5f), new(161.78f, 45.2f), new(160.9f, 44.63f),
    new(160.45f, 44.26f), new(159.58f, 42.92f), new(159.21f, 42.52f), new(158.42f, 41.81f), new(158.01f, 41.39f),
    new(157.38f, 39.98f), new(157.07f, 39.47f), new(156.04f, 38.21f), new(155.6f, 36.57f), new(155.36f, 36.04f),
    new(154.83f, 35.13f), new(154.56f, 34.58f), new(154.37f, 32.85f), new(154.19f, 32.25f), new(153.78f, 31.19f),
    new(153.71f, 30.69f), new(153.76f, 29.55f), new(153.76f, 28.96f), new(153.43f, 27.37f), new(153.44f, 26.86f),
    new(153.78f, 25.25f), new(153.7f, 23.69f), new(153.76f, 23.1f), new(154.31f, 21.66f), new(154.41f, 21.08f),
    new(154.53f, 19.94f), new(154.69f, 19.4f), new(155.53f, 17.93f), new(155.98f, 16.27f), new(156.23f, 15.81f),
    new(157.2f, 14.6f), new(157.91f, 13.06f), new(158.2f, 12.64f), new(159.4f, 11.54f), new(159.72f, 11.13f),
    new(160.01f, 10.69f), new(160.28f, 10.23f), new(160.64f, 9.8f), new(162.06f, 8.87f), new(162.48f, 8.46f),
    new(163.23f, 7.63f), new(163.72f, 7.27f), new(165.24f, 6.59f), new(165.65f, 6.22f), new(166.52f, 5.52f),
    new(166.99f, 5.29f), new(168.49f, 4.88f), new(169, 4.63f), new(169.53f, 4.31f), new(170.06f, 4.02f),
    new(170.65f, 3.94f), new(172.25f, 3.99f), new(173.17f, 3.37f), new(173.8f, 3.28f), new(181.12f, 3.27f)];
    private static readonly ArenaBoundsComplex arena = new([new PolygonCustom(vertices)]);

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.Page64));
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        foreach (var e in hints.PotentialTargets)
        {
            e.Priority = (OID)e.Actor.OID switch
            {
                OID.Page64 => 2,
                OID.Boss => 1,
                _ => 0
            };
        }
    }
}
