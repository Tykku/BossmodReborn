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
    private static readonly AOEShapeDonut donutSmall = new(5, 15), donutBig = new(5, 40);
    private static readonly AOEShapeCircle circleSmall = new(8), circleBig = new(16);
    private readonly List<AOEInstance> _aoes = [];
    private readonly List<Actor> _stars = [];

    private bool _tutorialFire, _tutorialIce;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = _aoes.Count;
        if (count != 0)
        {
            for (var i = 0; i < count; ++i)
                yield return _aoes[i];
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
        _stars.Remove(source);
        _stars.Remove(target);
        var activation = WorldState.FutureTime(10.6f);
        _aoes.Add(new(bigShape, midpoint, default, activation));
        if (_aoes.Any(x => x.Shape == donutBig) || _aoes.Count(x => x.Shape == circleBig) == 2)
        {
            for (var i = 0; i < _stars.Count; ++i)
                _aoes.Add(new(smallShape, _stars[i].Position, default, activation));
            _stars.Clear();
        }
    }

    public override void OnActorCreated(Actor actor)
    {
        if ((OID)actor.OID is OID.FrozenStar or OID.BurningStar)
            _stars.Add(actor);
        if (!_tutorialIce && _stars.Count(x => (OID)x.OID == OID.FrozenStar) == 4)
            Tutorial(donutSmall, ref _tutorialIce);
        else if (!_tutorialFire && _stars.Count(x => (OID)x.OID == OID.BurningStar) == 5)
            Tutorial(circleSmall, ref _tutorialFire);
    }

    private void Tutorial(AOEShape shape, ref bool tutorialFlag)
    {
        tutorialFlag = true;
        for (var i = 0; i < _stars.Count; ++i)
            _aoes.Add(new(shape, _stars[i].Position, default, WorldState.FutureTime(7.8f)));
        _stars.Clear();
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.CircleOfIceAOE or AID.CircleOfIcePrimeAOE or AID.FireSphereAOE or AID.FireSpherePrime1)
        {
            _aoes.Clear();
            ++NumCasts;
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

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 38, NameID = 2143, SortOrder = 10)]
public class D063LahabreaIgeyorhm(WorldState ws, Actor primary) : BossModule(ws, primary, arena.Center, arena)
{
    public static readonly ArenaBoundsComplex arena = new([new Polygon(new(230, -181), 20.26f, 24)], [new Rectangle(new(230, -160), 20, 1.94f)]);
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.Igeyorhm));
    }
}
