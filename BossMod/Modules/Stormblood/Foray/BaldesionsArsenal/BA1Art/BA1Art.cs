namespace BossMod.Stormblood.Foray.BaldesionArsenal.BA1Art;

class Thricecull(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.Thricecull));
class AcallamNaSenorach(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.AcallamNaSenorach));
class DefilersDeserts(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.DefilersDeserts), new AOEShapeRect(35.5f, 4));
class Pitfall(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Pitfall), 20);
class LegendaryGeasAOE(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.LegendaryGeas), 8);

class DefilersDesertsPredict(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCross cross = new(35.5f, 4);
    private readonly List<AOEInstance> _aoes = new(2);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        void AddAOE(Angle angle) => _aoes.Add(new(cross, spell.LocXZ, angle, Module.CastFinishAt(spell, 6.9f)));
        if ((AID)spell.Action.ID == AID.LegendaryGeas)
        {
            AddAOE(45.Degrees());
            AddAOE(default);
        }
        else if ((AID)spell.Action.ID == AID.DefilersDeserts)
            _aoes.Clear();
    }
}

class LegendaryGeasStay(BossModule module) : Components.StayMove(module)
{
    public override void OnActorEAnim(Actor actor, uint state)
    {
        if ((OID)actor.OID == OID.ShadowLinksHelper && actor.Position.AlmostEqual(new(-134.917f, 750.44f), 1))
        {
            if (state == 0x00010002)
                Array.Fill(PlayerStates, new(Requirement.Stay2, WorldState.CurrentTime, 1));
            else if (state == 0x00040008)
                Array.Clear(PlayerStates);
        }
    }
}

class GloryUnearthed(BossModule module) : Components.OpenWorldChasingAOEs(module, new AOEShapeCircle(10), ActionID.MakeSpell(AID.GloryUnearthedFirst), ActionID.MakeSpell(AID.GloryUnearthedRest), 6.5f, 1.5f, 5, true, (uint)IconID.ChasingAOE);
class PiercingDark(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.PiercingDark), 6);

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus)", GroupType = BossModuleInfo.GroupType.BaldesionArsenal, GroupID = 639, NameID = 7968, PlanLevel = 70, SortOrder = 1)]
public class BA1Art(WorldState ws, Actor primary) : BossModule(ws, primary, arena.Center, arena)
{
    private static readonly ArenaBoundsComplex arena = new([new Polygon(new(-128.98f, 748), 29.5f, 64)], [new Rectangle(new(-129, 718), 20, 1.15f), new Rectangle(new(-129, 778), 20, 1.48f),
    new Polygon(new(-123.5f, 778), 1.7f, 8), new Polygon(new(-134.5f, 778), 1.7f, 8), new Polygon(new(-123.5f, 718), 1.5f, 8), new Polygon(new(-134.5f, 718), 1.5f, 8)]);
}
