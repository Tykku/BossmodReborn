namespace BossMod.Shadowbringers.Foray.DelubrumReginae.DRS3Dahu;

// these two abilities are very similar, only differ by activation delay and action id
// TODO: not all the wall is safe...
abstract class FeralHowlHystericAssault(BossModule module, uint aidCast, AID aidAOE, float delay) : Components.GenericKnockback(module, ActionID.MakeSpell(aidAOE), true, stopAtWall: true)
{
    private Knockback? _source;

    public override ReadOnlySpan<Knockback> ActiveKnockbacks(int slot, Actor actor) => Utils.ZeroOrOne(ref _source);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == aidCast)
            _source = new(caster.Position, 30f, Module.CastFinishAt(spell, delay));
    }
}

class FeralHowl(BossModule module) : FeralHowlHystericAssault(module, (uint)AID.FeralHowl, AID.FeralHowlAOE, 2.1f);
class HystericAssault(BossModule module) : FeralHowlHystericAssault(module, (uint)AID.HystericAssault, AID.HystericAssaultAOE, 0.9f);
