namespace BossMod.Dawntrail.Alliance.A11Prishe;

class AuroralUppercut(BossModule module) : Components.Knockback(module, ignoreImmunes: true)
{
    private Source? _source;
    public override IEnumerable<Source> Sources(int slot, Actor actor) => Utils.ZeroOrOne(_source);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.AuroralUppercut1:
                _source = new(Arena.Center, 12, Module.CastFinishAt(spell));
                break;
            case AID.AuroralUppercut2:
                _source = new(Arena.Center, 25, Module.CastFinishAt(spell));
                break;
            case AID.AuroralUppercut3:
                _source = new(Arena.Center, 38, Module.CastFinishAt(spell));
                break;
        }
    }

    public override void OnStatusLose(Actor actor, ActorStatus status)
    {
        if (_source != null && (SID)status.ID == SID.Knockback)
        {
            NumCasts = 1;
            _source = null;
        }
    }
}
