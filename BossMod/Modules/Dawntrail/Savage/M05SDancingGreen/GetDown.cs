namespace BossMod.Dawntrail.Savage.M05SDancingGreen;

class GetDownCone(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.GetDownCone), GetDownBait.Cone);

class GetDownOutIn(BossModule module) : Components.GenericAOEs(module)
{
    public readonly List<AOEInstance> AOEs = new(8);
    private static readonly AOEShapeCircle circle = new(7f);
    private static readonly AOEShapeDonut donut = new(5f, 40f);

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => AOEs.Count != 0 ? CollectionsMarshal.AsSpan(AOEs)[..1] : [];

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == (uint)AID.GetDownCircle1)
        {
            for (var i = 0; i < 4; ++i)
            {
                AddAOE(circle, i * 5f);
                AddAOE(donut, i * 5f + 2.5f);
            }
        }

        void AddAOE(AOEShape shape, float delay)
            => AOEs.Add(new(shape, spell.LocXZ, default, Module.CastFinishAt(spell, delay)));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action.ID is (uint)AID.GetDownCircle1 or (uint)AID.GetDownCircle2 or (uint)AID.GetDownDonut)
        {
            ++NumCasts;
            if (AOEs.Count != 0)
                AOEs.RemoveAt(0);
        }
    }
}

class GetDownBait(BossModule module) : Components.GenericBaitAway(module)
{
    public static readonly AOEShapeCone Cone = new(40f, 22.5f.Degrees());

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == (uint)AID.GetDownCircle1)
        {
            var party = Raid.WithoutSlot(true, true, true);
            var len = party.Length;
            for (var i = 0; i < len; ++i)
            {
                ref readonly var p = ref party[i];
                CurrentBaits.Add(new(Module.PrimaryActor, p, Cone, Module.CastFinishAt(spell, 0.3f)));
            }
        }
    }

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if (status.ID is (uint)SID.WavelengthAlpha or (uint)SID.WavelengthBeta)
        {
            var count = CurrentBaits.Count;
            for (var i = 0; i < count; ++i)
            {
                if (CurrentBaits[i].Target == actor)
                {
                    CurrentBaits.RemoveAt(i);
                    break;
                }
            }
            var remaining = CollectionsMarshal.AsSpan(CurrentBaits);
            var len = remaining.Length;
            var act = WorldState.FutureTime(2.5d);
            for (var i = 0; i < len; ++i)
            {
                ref var b = ref remaining[i];
                b.Activation = act;
            }
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action.ID == (uint)AID.GetDownBait)
        {
            if (++NumCasts == 8) // not sure yet what happens if a player dies before baiting, so this is a failsafe
                CurrentBaits.Clear();
        }
    }
}
