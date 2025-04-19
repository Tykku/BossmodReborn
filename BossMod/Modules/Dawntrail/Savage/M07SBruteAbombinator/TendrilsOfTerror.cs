namespace BossMod.Dawntrail.Savage.M07SBruteAbombinator;

class SinisterSeedsAOE(BossModule module) : Components.SimpleAOEs(module, (uint)AID.SinisterSeeds, 7f);
class SinisterSeedsSpread(BossModule module) : Components.SpreadFromCastTargets(module, (uint)AID.SinisterSeedsSpread, 6f);
class StrangeSeeds(BossModule module) : Components.SpreadFromCastTargets(module, (uint)AID.StrangeSeeds, 6f);
class KillerSeeds(BossModule module) : Components.StackWithCastTargets(module, (uint)AID.KillerSeeds, 6f, 2, 2);

class TendrilsOfTerrorBait(BossModule module) : Components.GenericBaitAway(module)
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.SinisterSeedsSpread or (uint)AID.StrangeSeeds or (uint)AID.KillerSeeds)
        {
            var target = WorldState.Actors.Find(spell.TargetID)!;
            AddBait();
            AddBait(45f.Degrees());
            void AddBait(Angle rotation = default) => CurrentBaits.Add(new(target, target, TendrilsOfTerror.Cross, Module.CastFinishAt(spell, 4.6f), rotation));
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.SinisterSeedsSpread or (uint)AID.StrangeSeeds or (uint)AID.KillerSeeds)
        {
            CurrentBaits.Clear();
        }
    }

    public override void AddHints(int slot, Actor actor, TextHints hints) { }
    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints) { }
}

class TendrilsOfTerrorPrediction(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = new(8);

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => CollectionsMarshal.AsSpan(_aoes);

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        switch (spell.Action.ID)
        {
            case (uint)AID.SinisterSeedsSpread:
            case (uint)AID.StrangeSeeds:
            case (uint)AID.KillerSeeds:
                var pos = WorldState.Actors.Find(caster.CastInfo!.TargetID)?.Position;
                if (pos is WPos position)
                {
                    var act = Module.CastFinishAt(spell, 4f);
                    AddAOE();
                    AddAOE(45f.Degrees());
                    void AddAOE(Angle rotation = default) => _aoes.Add(new(TendrilsOfTerror.Cross, position, rotation, act));
                }
                break;
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch (spell.Action.ID) // prediction is slightly off due to server weirdness, also if target dies from spread/stack there are no tendrils, so we clear when real cast starts
        {
            case (uint)AID.TendrilsOfTerrorCross1:
            case (uint)AID.TendrilsOfTerrorCross2:
            case (uint)AID.TendrilsOfTerrorCross3:
                _aoes.Clear();
                break;
        }
    }
}

class TendrilsOfTerror(BossModule module) : Components.GenericAOEs(module)
{
    public static readonly AOEShapeCross Cross = new(60f, 2f);
    public readonly List<AOEInstance> AOEs = new(8);

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => CollectionsMarshal.AsSpan(AOEs);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.TendrilsOfTerrorCross1 or (uint)AID.TendrilsOfTerrorCross2 or (uint)AID.TendrilsOfTerrorCross3)
            AOEs.Add(new(Cross, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell)));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        switch (spell.Action.ID)
        {
            case (uint)AID.TendrilsOfTerrorCross1:
            case (uint)AID.TendrilsOfTerrorCross2:
            case (uint)AID.TendrilsOfTerrorCross3:
                AOEs.Clear();
                ++NumCasts;
                break;
        }
    }
}

class Impact(BossModule module) : Components.GenericStackSpread(module, true, raidwideOnResolve: false)
{
    public int NumCasts;

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (Stacks.Count == 0 && spell.Action.ID == (uint)AID.SinisterSeedsSpread)
        {
            var party = Raid.WithoutSlot(true, true, true);
            var len = party.Length;
            var act = Module.CastFinishAt(spell, 4.6f);
            for (var i = 0; i < len; ++i)
            {
                ref readonly var p = ref party[i];
                if (p.Role == Role.Healer)
                    Stacks.Add(new(p, 6f, 4, 4, act));
            }
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action.ID == (uint)AID.Impact)
        {
            ++NumCasts;
        }
    }
}
