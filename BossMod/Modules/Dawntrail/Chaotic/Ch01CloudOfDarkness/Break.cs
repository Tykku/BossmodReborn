namespace BossMod.Dawntrail.Chaotic.Ch01CloudOfDarkness;

class Break(BossModule module) : Components.GenericGaze(module)
{
    public readonly List<Eye> Eyes = new(3);

    public override IEnumerable<Eye> ActiveEyes(int slot, Actor actor) => Eyes;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.BreakBoss or AID.BreakEye)
            Eyes.Add(new(spell.LocXZ, Module.CastFinishAt(spell, 0.9f)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.BreakBossAOE or AID.BreakEyeAOE)
            for (var i = 0; i < Eyes.Count; ++i)
            {
                var eye = Eyes[i];
                if (eye.Position == caster.Position)
                {
                    Eyes.Remove(eye);
                    break;
                }
            }
    }
}
