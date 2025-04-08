namespace BossMod.Dawntrail.Savage.M06SSugarRiot;

class Adds(BossModule module) : Components.AddsMulti(module, [(uint)OID.Mu, (uint)OID.Yan, (uint)OID.FeatherRay, (uint)OID.GimmeCat, (uint)OID.Jabberwock])
{
    public int CountMu;
    public int CountYan;
    public int CountFeatherRay;
    public int CountJabberwock;
    public int CountGimmeCat;

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var count = hints.PotentialTargets.Count;
        for (var i = 0; i < count; ++i)
        {
            var e = hints.PotentialTargets[i];
            e.Priority = e.Actor.OID switch
            {
                (uint)OID.Jabberwock => 5,
                (uint)OID.GimmeCat => 4,
                (uint)OID.FeatherRay => 3,
                (uint)OID.Mu => 2,
                (uint)OID.Yan => 1,
                _ => 0
            };
        }
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        Arena.Actors(Module.Enemies((uint)OID.Jabberwock), Colors.Danger);
        Arena.Actors(Module.Enemies((uint)OID.FeatherRay), Colors.Other1);
        Arena.Actors(Module.Enemies((uint)OID.Mu), Colors.Other2);
        Arena.Actors(Module.Enemies((uint)OID.GimmeCat), Colors.Object);
        Arena.Actors(Module.Enemies((uint)OID.Yan), Colors.Other4);
    }

    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if (id == 0x11D1)
        {
            switch (actor.OID)
            {
                case (uint)OID.Mu:
                    ++CountMu;
                    break;
                case (uint)OID.Yan:
                    ++CountYan;
                    break;
                case (uint)OID.Jabberwock:
                    ++CountJabberwock;
                    break;
                case (uint)OID.FeatherRay:
                    ++CountFeatherRay;
                    break;
                case (uint)OID.GimmeCat:
                    ++CountGimmeCat;
                    break;
            }
        }
    }
}

class ICraveViolence(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.ICraveViolence), 6f);
class OreRigato(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.OreRigato));
class WaterIIIBait(BossModule module) : Components.BaitAwayTethers(module, new AOEShapeCircle(8f), (uint)TetherID.WaterIII, ActionID.MakeSpell(AID.WaterIII), (uint)OID.FeatherRay, 7.3f, true);
class WaterIIIVoidzone(BossModule module) : Components.VoidzoneAtCastTarget(module, 9f, ActionID.MakeSpell(AID.WaterIII), GetVoidzones, 1.6f) // apparently the voidzone is bigger than the AOE?
{
    private static Actor[] GetVoidzones(BossModule module)
    {
        var enemies = module.Enemies((uint)OID.WaterVoidzone);
        var count = enemies.Count;
        if (count == 0)
            return [];

        var voidzones = new Actor[count];
        var index = 0;
        for (var i = 0; i < count; ++i)
        {
            var z = enemies[i];
            if (z.EventState != 7)
                voidzones[index++] = z;
        }
        return voidzones[..index];
    }
}

class ManxomeWindersnatch(BossModule module) : Components.SingleTargetInstant(module, ActionID.MakeSpell(AID.ManxomeWindersnatch), 5f)
{
    private Actor? _target;

    public override void AddGlobalHints(GlobalHints hints)
    {
        if (_target is Actor target)
            hints.Add($"Big hit on {target.Name}!");
    }

    public override void OnEventIcon(Actor actor, uint iconID, ulong targetID)
    {
        if (iconID == (uint)IconID.ManxomeWindersnatch)
        {
            _target = actor;
            Targets.Add((Raid.FindSlot(actor.InstanceID), WorldState.FutureTime(5d)));
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        base.OnEventCast(caster, spell);
        if (spell.Action == WatchedAction)
        {
            _target = null;
        }
    }

    public override void Update()
    {
        if (_target is Actor target)
        {
            var jabberwocks = Module.Enemies((uint)OID.Jabberwock);
            var count = jabberwocks.Count;
            var allDead = true;
            for (var i = 0; i < count; ++i)
            {
                var enemy = jabberwocks[i];
                if (!enemy.IsDeadOrDestroyed)
                {
                    allDead = false;
                    break;
                }
            }
            if (target.IsDead || allDead)
            {
                _target = null;
                Targets.Clear();
                return;
            }
        }
    }
}
