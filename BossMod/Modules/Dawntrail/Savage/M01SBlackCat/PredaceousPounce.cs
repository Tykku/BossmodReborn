namespace BossMod.Dawntrail.Savage.M01SBlackCat;

class PredaceousPounce(BossModule module) : Components.GenericAOEs(module)
{
    public readonly List<AOEInstance> AOEs = [];
    private bool sorted;
    private static readonly AOEShapeCircle circle = new(11);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (AOEs.Count > 0)
        {
            var aoeCount = Math.Clamp(AOEs.Count, 0, 2);
            for (var i = aoeCount; i < AOEs.Count; i++)
                yield return AOEs[i];
            for (var i = 0; i < aoeCount; i++)
                yield return AOEs[i] with { Color = Colors.Danger };
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch ((AID)spell.Action.ID)
        {
            case AID.PredaceousPounceTelegraphCharge1:
            case AID.PredaceousPounceTelegraphCharge2:
            case AID.PredaceousPounceTelegraphCharge3:
            case AID.PredaceousPounceTelegraphCharge4:
            case AID.PredaceousPounceTelegraphCharge5:
            case AID.PredaceousPounceTelegraphCharge6:
                var dir = spell.LocXZ - caster.Position;
                AOEs.Add(new(new AOEShapeRect(dir.Length(), 3), caster.Position, Angle.FromDirection(dir), Module.CastFinishAt(spell)));
                break;
            case AID.PredaceousPounceTelegraphCircle1:
            case AID.PredaceousPounceTelegraphCircle2:
            case AID.PredaceousPounceTelegraphCircle3:
            case AID.PredaceousPounceTelegraphCircle4:
            case AID.PredaceousPounceTelegraphCircle5:
            case AID.PredaceousPounceTelegraphCircle6:
                AOEs.Add(new(circle, caster.Position, default, Module.CastFinishAt(spell)));
                break;
        }
        if (AOEs.Count == 12 && !sorted)
        {
            AOEs.SortBy(x => x.Activation);
            for (var i = 0; i < AOEs.Count; i++)
            {
                var aoe = AOEs[i];
                aoe.Activation = Module.WorldState.FutureTime(13.5f + i * 0.5f);
                AOEs[i] = aoe;
            }
            sorted = true;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.PredaceousPounceCharge1 or AID.PredaceousPounceCharge2 or AID.PredaceousPounceCircle1 or AID.PredaceousPounceCircle2)
        {
            ++NumCasts;
            if (AOEs.Count > 0)
                AOEs.RemoveAt(0);
        }
    }
}
