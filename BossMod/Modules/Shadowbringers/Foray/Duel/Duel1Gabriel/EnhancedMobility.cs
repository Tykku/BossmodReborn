namespace BossMod.Shadowbringers.Foray.Duel.Duel1Gabriel;

class Burst(BossModule module) : Components.CastTowers(module, (uint)AID.Burst, 4f);

class EnhancedMobility(BossModule module) : Components.GenericKnockback(module, (uint)AID.EnhancedMobility)
{
    private Knockback? _kb;
    private WPos tower;
    private readonly MagitekCannonVoidzone _aoe = module.FindComponent<MagitekCannonVoidzone>()!;

    public override ReadOnlySpan<Knockback> ActiveKnockbacks(int slot, Actor actor) => Utils.ZeroOrOne(ref _kb);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == WatchedAction)
        {
            _kb = new(caster.Position, 12f, Module.CastFinishAt(spell));
        }
        else if (spell.Action.ID == (uint)AID.Burst)
        {
            tower = spell.LocXZ;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == WatchedAction)
        {
            _kb = null;
        }
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (_kb is Knockback kb)
        {
            var act = kb.Activation;
            if (!IsImmune(slot, act))
            {
                var dir = (Module.PrimaryActor.Position - Arena.Center).Normalized();
                hints.AddForbiddenZone(ShapeDistance.InvertedRect(tower + 12f * dir, dir, 4f, default, 1f), act);
            }
        }
    }

    public override bool DestinationUnsafe(int slot, Actor actor, WPos pos)
    {
        var comp = _aoe.ActiveAOEs(slot, actor);
        var len = comp.Length;
        for (var i = 0; i < len; ++i)
        {
            if (comp[i].Check(pos))
            {
                return true;
            }
        }
        return !Module.InBounds(pos);
    }
}

class EnhancedMobilityFullyAnalyzedHint(BossModule module) : Components.CastHint(module, (uint)AID.EnhancedMobility, "Use manawall or tank invuln to survive!");
class EnhancedMobilityWeakpoint(BossModule module) : Components.CastWeakpoint(module, (uint)AID.EnhancedMobility, 40f, default, (uint)SID.BackUnseen, (uint)SID.LeftUnseen, (uint)SID.RightUnseen);
