namespace BossMod.Dawntrail.Savage.M08SHowlingBlade;

class Mooncleaver2(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Mooncleaver2), 8f);

class HowlingEight(BossModule module) : Components.GenericTowers(module)
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.HowlingEightFirst1 or (uint)AID.HowlingEightRest1)
            Towers.Add(new(spell.LocXZ, 8f, 8, 8, activation: Module.CastFinishAt(spell)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        switch (spell.Action.ID)
        {
            case (uint)AID.HowlingEightFirst1:
            case (uint)AID.HowlingEightRest1:
                ++NumCasts;
                break;
            case (uint)AID.HowlingEightFirst8:
            case (uint)AID.HowlingEightRest8:
                Towers.Clear();
                break;
        }
    }
}
