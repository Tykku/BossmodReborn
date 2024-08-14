namespace BossMod.Dawntrail.Dungeon.D08TheStrayboroughDeadwalk.D082JackInThePot;

public enum OID : uint
{
    Boss = 0x41CA, // R4.16
    SpectralSamovar = 0x41CB, // R2.88
    StrayPhantagenitrix = 0x41D2, // R2.1
    TeacupHelper = 0x41D5, // R1.5
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 37169, // Boss->player, no cast, single-target

    TroublingTeacups = 36716, // Boss->self, 3.0s cast, single-target, Spawns teacups
    TeaAwhirl = 36717, // Boss->self, 6.0s cast, single-target, ghost(s) tether teacup and enters, teacups spin then possesed teacup explodes in AOE
    TricksomeTreat = 36720, // StrayPhantagenitrix->self, 3.0s cast, range 19 circle, TeaAwhirl AOE

    ToilingTeapots = 36722, // Boss->self, 3.0s cast, single-target, spawns 13 teacups

    Puppet = 36721, // StrayPhantagenitrix->location, 4.0s cast, single-target
    PipingPour = 36723, // SpectralSamovar->location, 2.0s cast, single-target, spreading AOE

    MadTeaParty = 36724, // Helper->self, no cast, range 0 circle, DOT applied to players in puddles

    LastDrop = 36726, // Boss->player, 5.0s cast, single-target, tankbuster

    SordidSteam = 36725, // Boss->self, 5.0s cast, range 40 circle, raidwide
}

public enum TetherID : uint
{
    CupTether = 276 // UnknownActor->StrayPhantagenitrix
}

public enum SID : uint
{
    AreaOfInfluenceUp = 1909 // none->Helper, extra=0x1/0x2/0x3/0x4
}

class PipingPour(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCircle circle = new(8);
    private readonly List<AOEInstance> _aoes = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;

    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if (_aoes.Count > 0 && id == 0x11DD && (OID)actor.OID == OID.SpectralSamovar)
            _aoes.RemoveAt(0);
    }

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if ((SID)status.ID == SID.AreaOfInfluenceUp && status.Extra == 0x1)
            _aoes.Add(new(circle, actor.Position));
    }
}

class TeaAwhirl(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCircle circle = new(19);
    private readonly List<Actor> _cups = [];
    private readonly List<AOEInstance> _aoes = [];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;

    public override void OnTethered(Actor source, ActorTetherInfo tether)
    {
        if (tether.ID == (uint)TetherID.CupTether)
            _cups.Add(source);
    }

    public override void OnEventEnvControl(byte index, uint state)
    {
        switch (index)
        {
            case 0x23:
            case 0x01:
                HandleCups(state);
                break;
        }
    }

    private void HandleCups(uint state)
    {
        if (state == 0x02000100)
        {
            var activation = Module.WorldState.FutureTime(11.5f);
            if (CheckPositions(new WPos(17, -163), new WPos(17, -177)))
                AddAOEs(activation, new(3.5f, -161.5f), new(30.5f, -178.5f));
            else if (CheckPositions(new WPos(17, -153), new WPos(10, -170)))
                AddAOEs(activation, new(25.5f, -156.5f), new(20.5f, -178.5f));
            else if (CheckPositions(new WPos(34, -170), null))
                AddAOEs(activation, [new(8.5f, -173.5f)]);
            else if (CheckPositions(new WPos(0, -170), null))
                AddAOEs(activation, [new(25.5f, -166.5f)]);
        }
        else if (state == 0x10000800)
        {
            var activation = Module.WorldState.FutureTime(14.5f);
            if (CheckPositions(new WPos(0, -170), new WPos(34, -170)))
                AddAOEs(activation, new(8.5f, -156.5f), new(25.5f, -183.5f));
            else if (CheckPositions(new WPos(0, -170), new WPos(17, -187)))
                AddAOEs(activation, new(3.5f, -178.5f), new(8.5f, -156.5f));
        }
        else if (state == 0x00100001)
            AddAOEs(Module.WorldState.FutureTime(16), _cups[0].Position, _cups[1].Position);
        else if (state == 0x00400020)
        {
            var activation = Module.WorldState.FutureTime(19);
            if (CheckPositions(new WPos(0, -170), new WPos(17, -163)))
                AddAOEs(activation, new(5, -165), new(22, -182));
            else if (CheckPositions(new WPos(17, -177), new WPos(17, -153)))
                AddAOEs(activation, new(5, -175), new(29, -175));
        }
    }

    private bool CheckPositions(WPos pos1, WPos? pos2) => pos2 != null ? _cups.Any(x => x.Position == pos2) : _cups.Any(x => x.Position == pos1);

    private void AddAOEs(DateTime activation, params WPos[] positions)
    {
        foreach (var pos in positions)
            _aoes.Add(new(circle, pos, default, activation));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.TricksomeTreat)
        {
            _aoes.Clear();
            _cups.Clear();
        }
    }
}

class SordidSteam(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.SordidSteam));
class LastDrop(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.LastDrop));

class D082JackInThePotStates : StateMachineBuilder
{
    public D082JackInThePotStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<TeaAwhirl>()
            .ActivateOnEnter<PipingPour>()
            .ActivateOnEnter<SordidSteam>()
            .ActivateOnEnter<LastDrop>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 981, NameID = 12760)]
public class D082JackInThePot(WorldState ws, Actor primary) : BossModule(ws, primary, arena.Center, arena)
{
    private static readonly List<Shape> union = [new Circle(new(17, -170), 19.5f)];
    private static readonly List<Shape> difference = [new Rectangle(new(17, -150.15f), 20, 1.25f), new Rectangle(new(17, -189.5f), 20, 1.25f)];
    private static readonly ArenaBounds arena = new ArenaBoundsComplex(union, difference);
}
