namespace BossMod.Heavensward.Dungeon.D06AetherochemicalResearchFacility.D063LahabreaIgeyorhm;

public enum OID : uint
{
    Boss = 0x3DA4, // R3.5
    Igeyorhm = 0x3DA3, // R3.5
    BurningStar = 0x3DA6, // R1.5
    FrozenStar = 0x3DA5, // R1.5
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 32818, // Boss/Lahabrea->player, no cast, single-target
    Teleport = 32791, // Boss->location, no cast, single-target, teleport to center

    AetherialDivideIgeyorhm = 32686, // Boss->Lahabrea, no cast, single-target, Igeyorhm donates HP to Lahabrea
    AetherialDivideLahabrea = 32685, // Lahabrea->Boss, no cast, single-target, Lahabrea donates HP to Igeyorhm

    CircleOfIce = 31878, // Boss->self, 3.0s cast, single-target
    CircleOfIceAOE = 31879, // FrozenStar->self, 3.0s cast, range 5-15 donut

    CircleOfIcePrime1 = 31881, // FrozenStar->self, no cast, single-target
    CircleOfIcePrime2 = 31882, // FrozenStar->self, no cast, single-target
    CircleOfIcePrimeAOE = 33019, // Helper->self, 3.0s cast, range 5-40 donut

    DarkFireII = 32687, // Lahabrea->self, 6.0s cast, single-target
    DarkFireIIAOE = 32688, // Helper->player, 6.0s cast, range 6 circle

    EndOfDays = 31892, // Helper->player, no cast, single-target
    EndOfDays1 = 31891, // Lahabrea->self, 5.0s cast, single-target
    EndOfDays2 = 33029, // Helper->self, no cast, range 50 width 8 rect, line stack

    EsotericFusion1 = 31880, // Boss->self, 3.0s cast, single-target
    EsotericFusion2 = 31888, // Lahabrea->self, 3.0s cast, single-target

    FireSphere = 31886, // Lahabrea->self, 3.0s cast, single-target
    FireSphereAOE = 31887, // BurningStar->self, 3.0s cast, range 8 circle

    FireSpherePrime1 = 31889, // BurningStar->self, no cast, single-target
    FireSpherePrime2 = 31890, // BurningStar->self, no cast, single-target
    FireSpherePrimeAOE = 33020, // Helper->self, 2.0s cast, range 16 circle

    GripOfNight = 32790, // Boss->self, 6.0s cast, range 40 150-degree cone

    ShadowFlare = 31885 // Boss/Lahabrea->self, 5.0s cast, range 40 circle
}

public enum TetherID : uint
{
    StarTether = 110 // BurningStar/FrozenStar->BurningStar/FrozenStar
}

class ShadowFlare(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.ShadowFlare));
class GripOfNight(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.GripOfNight), new AOEShapeCone(40, 75.Degrees()));
class DarkFireIIAOE(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.DarkFireIIAOE), 6);
class EndOfDays(BossModule module) : Components.LineStack(module, ActionID.MakeSpell(AID.EndOfDays), ActionID.MakeSpell(AID.EndOfDays2), 5.1f);

class Stars(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeDonut donutSmall = new(5, 15);
    private static readonly AOEShapeDonut donutBig = new(5, 40);
    private static readonly AOEShapeCircle circleSmall = new(8);
    private static readonly AOEShapeCircle circleBig = new(16);

    private readonly List<AOEInstance> _aoes = [];
    private readonly List<Actor> _stars = [];

    private bool _tutorialFire;
    private bool _tutorialIce;
    private DateTime _activation;
    private AOEShape? _shape;
    private bool _active;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_shape != null && _active)
        {
            foreach (var star in _stars)
                yield return new(_shape, star.Position, default, _activation);
            foreach (var aoe in _aoes)
                yield return new(aoe.Shape, aoe.Origin, default, aoe.Activation);
        }
    }

    public override void OnTethered(Actor source, ActorTetherInfo tether)
    {
        if (tether.ID == (uint)TetherID.StarTether)
        {
            var target = WorldState.Actors.Find(tether.Target)!;
            var targetPos = target.Position;
            var midpoint = new WPos((source.Position.X + targetPos.X) / 2, (source.Position.Z + targetPos.Z) / 2);
            switch (source.OID)
            {
                case (uint)OID.FrozenStar:
                    ActivateAOE(donutSmall, donutBig, midpoint, source, target);
                    break;
                case (uint)OID.BurningStar:
                    ActivateAOE(circleSmall, circleBig, midpoint, source, target);
                    break;
            }
        }
    }

    private void ActivateAOE(AOEShape smallShape, AOEShape bigShape, WPos midpoint, Actor source, Actor target)
    {
        _shape = smallShape;
        _active = true;
        _stars.Remove(source);
        _stars.Remove(target);
        _activation = WorldState.FutureTime(10.6f);
        _aoes.Add(new AOEInstance(bigShape, midpoint, default, _activation));
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID is OID.FrozenStar or OID.BurningStar)
            _stars.Add(actor);
        if ((OID)actor.OID == OID.FrozenStar && !_tutorialIce)
            Tutorial(donutSmall, ref _tutorialIce);
        else if ((OID)actor.OID == OID.BurningStar && !_tutorialFire)
            Tutorial(circleSmall, ref _tutorialFire);
    }

    private void Tutorial(AOEShape shape, ref bool tutorialFlag)
    {
        _activation = WorldState.FutureTime(7.8f);
        tutorialFlag = true;
        _shape = shape;
        _active = true;
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.CircleOfIceAOE or AID.CircleOfIcePrimeAOE or AID.FireSphereAOE or AID.FireSpherePrime1)
        {
            _shape = null;
            _stars.Clear();
            _aoes.Clear();
            NumCasts++;
            _active = false;
        }
    }
}

class D063LahabreaIgeyorhmStates : StateMachineBuilder
{
    public D063LahabreaIgeyorhmStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<ShadowFlare>()
            .ActivateOnEnter<GripOfNight>()
            .ActivateOnEnter<Stars>()
            .ActivateOnEnter<EndOfDays>()
            .ActivateOnEnter<DarkFireIIAOE>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 38, NameID = 2143)]
public class D063LahabreaIgeyorhm(WorldState ws, Actor primary) : BossModule(ws, primary, arena.Center, arena)
{
    private static readonly WPos[] vertices = [new(230.1f, -201.34f), new(234.87f, -200.71f), new(235.38f, -200.61f), new(240.02f, -198.69f), new(240.45f, -198.41f),
    new(244.38f, -195.4f), new(247.63f, -191.14f), new(249.61f, -186.38f), new(249.71f, -185.81f), new(250.29f, -181.43f),
    new(250.33f, -180.92f), new(249.62f, -175.65f), new(247.73f, -171.08f), new(247.43f, -170.58f), new(244.36f, -166.6f),
    new(240.13f, -163.36f), new(236.08f, -161.86f), new(223.25f, -161.84f), new(222.41f, -161.88f), new(221.95f, -162.24f),
    new(220.03f, -163.28f), new(219.54f, -163.61f), new(215.6f, -166.65f), new(212.39f, -170.8f), new(210.35f, -175.71f),
    new(209.66f, -180.9f), new(209.71f, -181.43f), new(210.29f, -185.84f), new(210.39f, -186.39f), new(212.37f, -191.13f),
    new(212.72f, -191.62f), new(215.54f, -195.29f), new(215.93f, -195.64f), new(219.66f, -198.5f), new(220.15f, -198.76f),
    new(224.76f, -200.66f), new(229.87f, -201.33f)];
    public static readonly ArenaBoundsComplex arena = new([new PolygonCustom(vertices)]);
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.Igeyorhm));
    }
}
