namespace BossMod.Dawntrail.Savage.M03SBruteBomber;

class TagTeamLariatCombo(BossModule module) : Components.GenericAOEs(module)
{
    public readonly List<AOEInstance> AOEs = [];
    private readonly Actor?[] _tetherSource = new Actor?[PartyState.MaxPartySize];
    private const string hint = "Go to correct spot!";
    private static readonly AOEShapeRect rect = new(70, 17);
    private ConeHA? cone;
    private static readonly HashSet<AID> castStart = [AID.TagTeamLariatComboFirstRAOE, AID.TagTeamLariatComboFirstLAOE,
        AID.FusesOfFuryLariatComboFirstRAOE, AID.FusesOfFuryLariatComboFirstLAOE];
    private static readonly HashSet<AID> castEnd = [.. castStart, .. new HashSet<AID> { AID.TagTeamLariatComboSecondRAOE,
            AID.TagTeamLariatComboSecondLAOE, AID.FusesOfFuryLariatComboSecondRAOE, AID.FusesOfFuryLariatComboSecondLAOE}];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_tetherSource[slot] != null && AOEs.Count > 0)
        {
            var (safeShapes, dangerShapes) = GetShapesForAOEs(slot);
            var coneShapes = cone != null ? [cone] : new List<Shape>();

            yield return new(new AOEShapeCustom(safeShapes, dangerShapes, coneShapes, true, cone != null ? OperandType.Intersection : OperandType.Union),
                Arena.Center, default, AOEs.FirstOrDefault().Activation, Colors.SafeFromAOE);
        }
        else
            foreach (var aoe in AOEs)
                yield return aoe;
    }

    private (List<Shape> safeShapes, List<Shape> dangerShapes) GetShapesForAOEs(int slot)
    {
        List<Shape> safeShapes = [];
        List<Shape> dangerShapes = [];
        foreach (var aoe in AOEs.Where(x => x.Shape == rect))
        {
            var isSafe = _tetherSource[slot]?.Position.AlmostEqual(aoe.Origin, 25) ?? false;
            var shape = new RectangleSE(aoe.Origin, aoe.Origin + rect.LengthFront * aoe.Rotation.ToDirection(), rect.HalfWidth);

            if (isSafe)
                safeShapes.Add(shape);
            else
                dangerShapes.Add(shape);
        }
        return (safeShapes, dangerShapes);
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (ActiveAOEs(slot, actor).Any(c => c.Shape == rect))
            base.AddHints(slot, actor, hints);
        else if (ActiveAOEs(slot, actor).Any(c => !c.Check(actor.Position)))
            hints.Add(hint);
        else if (ActiveAOEs(slot, actor).Any(c => c.Check(actor.Position)))
            hints.Add(hint, false);
    }

    public override void OnTethered(Actor source, ActorTetherInfo tether)
    {
        if (tether.ID == (uint)TetherID.ChainDeathmatch && Raid.FindSlot(tether.Target) is var slot && slot >= 0)
            _tetherSource[slot] = source;
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (castStart.Contains((AID)spell.Action.ID))
            AOEs.Add(new(rect, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell)));
        else if ((AID)spell.Action.ID == AID.FusesOfFuryMurderousMist)
            cone = new(caster.Position, 40, spell.Rotation, 135.Degrees());
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (castEnd.Contains((AID)spell.Action.ID))
        {
            ++NumCasts;
            Array.Fill(_tetherSource, null);
            var index = AOEs.FindIndex(aoe => aoe.Origin.AlmostEqual(spell.LocXZ, 1));
            if (index < 0)
            {
                ReportError($"Failed to find AOE for {spell.LocXZ}");
            }
            else if (castStart.Contains((AID)spell.Action.ID))
            {
                ref var aoe = ref AOEs.Ref(index);
                aoe.Origin = Module.Center - (aoe.Origin - Module.Center);
                aoe.Rotation += 180.Degrees();
                aoe.Activation = WorldState.FutureTime(4.3f);
            }
            else
            {
                AOEs.RemoveAt(index);
            }
        }
        else if ((AID)spell.Action.ID == AID.FusesOfFuryMurderousMist)
            cone = null;
    }
}
