namespace BossMod.Endwalker.FATE.Daivadipa;

public enum OID : uint
{
    Boss = 0x356D, // R=8.0
    OrbOfImmolationBlue = 0x3570, //R=1.0
    OrbOfImmolationRed = 0x356F, //R=1.0
    OrbOfConflagrationBlue = 0x3572, //R=1.0
    OrbOfConflagrationRed = 0x3571, //R=1.0
    Helper1 = 0x3573, //R=0.5
    Helper2 = 0x3574, //R=0.5
    Helper3 = 0x3575, //R=0.5
}

public enum AID : uint
{
    AutoAttack = 872, // Boss->player, no cast, single-target

    Drumbeat = 26510, // Boss->player, 5.0s cast, single-target
    LeftwardTrisula = 26508, // Boss->self, 7.0s cast, range 65 180-degree cone
    RightwardParasu = 26509, // Boss->self, 7.0s cast, range 65 180-degree cone
    Lamplight = 26497, // Boss->self, 2.0s cast, single-target
    LoyalFlameBlue = 26499, // Boss->self, 5.0s cast, single-target, blue first
    LoyalFlameRed = 26498, // Boss->self, 5.0s cast, single-target, red first
    LitPathBlue = 26501, // OrbOfImmolation->self, 1.0s cast, range 50 width 10 rect, blue orb
    LitPathRed = 26500, // OrbOfImmolation2->self, 1.0s cast, range 50 width 10 rect, red orbs
    CosmicWeave = 26513, // Boss->self, 4.0s cast, range 18 circle
    YawningHellsVisual = 26511, // Boss->self, no cast, single-target
    YawningHells = 26512, // Helper1->location, 3.0s cast, range 8 circle
    ErrantAkasa = 26514, // Boss->self, 5.0s cast, range 60 90-degree cone
    InfernalRedemptionVisual = 26517, // Boss->self, 5.0s cast, single-target
    InfernalRedemption = 26518, // Helper3->location, no cast, range 60 circle
    IgnitingLights1 = 26503, // Boss->self, 2.0s cast, single-target
    IgnitingLights2 = 26502, // Boss->self, 2.0s cast, single-target
    BurnBlue = 26507, // OrbOfConflagration->self, 1.0s cast, range 10 circle, blue orbs
    BurnRed = 26506, // OrbOfConflagration2->self, 1.0s cast, range 10 circle, red orbs   
    KarmicFlamesVisual = 26515, // Boss->self, 5.5s cast, single-target
    KarmicFlames = 26516, // Helper2->location, 5.0s cast, range 50 circle, damage fall off, safe distance should be about 20
    DivineCall1 = 27080, // Boss->self, 4.0s cast, range 65 circle, forced backwards march
    DivineCall2 = 26520, // Boss->self, 4.0s cast, range 65 circle, forced right march
    DivineCall3 = 27079, // Boss->self, 4.0s cast, range 65 circle, forced forward march
    DivineCall4 = 26519 // Boss->self, 4.0s cast, range 65 circle, forced left march
}

public enum SID : uint
{
    AboutFace = 1959, // Boss->player, extra=0x0
    RightFace = 1961, // Boss->player, extra=0x0
    ForwardMarch = 1958, // Boss->player, extra=0x0
    LeftFace = 1960, // Boss->player, extra=0x0
}

class LitPath(BossModule module) : Components.GenericAOEs(module)
{
    public readonly List<AOEInstance> AOEs = new(5);
    private static readonly AOEShapeRect rect = new(50, 5);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = AOEs.Count;
        if (count == 0)
            return [];
        var max = count > 3 ? 3 : count;
        var firstact = AOEs[0].Activation;
        List<AOEInstance> aoes = new(max);
        for (var i = 0; i < max; ++i) // either 2 or 3 AOEs in a wave, no need to iterate on all 5
        {
            var aoe = AOEs[i];
            if ((aoe.Activation - firstact).TotalSeconds < 1)
                aoes.Add(aoe);
        }
        return aoes;
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.LoyalFlameBlue or AID.LoyalFlameRed)
        {
            var isBlue = (AID)spell.Action.ID == AID.LoyalFlameBlue;
            AddAOEs(Module.Enemies(OID.OrbOfImmolationBlue), spell, isBlue ? 2.2f : 4.4f);
            AddAOEs(Module.Enemies(OID.OrbOfImmolationRed), spell, isBlue ? 4.4f : 2.2f);
            if (!isBlue)
                AOEs.Reverse();
        }
    }

    private void AddAOEs(List<Actor> orbs, ActorCastInfo spell, float delay)
    {
        for (var i = 0; i < orbs.Count; ++i)
        {
            var orb = orbs[i];
            AOEs.Add(new(rect, orb.Position, orb.Position.X < -632 ? Angle.AnglesCardinals[3] : Angle.AnglesCardinals[2], Module.CastFinishAt(spell, delay)));
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (AOEs.Count != 0 && (AID)spell.Action.ID is AID.LitPathBlue or AID.LitPathRed)
            AOEs.RemoveAt(0);
    }
}

class Burn(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = new(16);
    private static readonly AOEShapeCircle circle = new(10);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = _aoes.Count;
        if (count == 0)
            return [];
        var max = count > 8 ? 8 : count;
        List<AOEInstance> aoes = new(max);
        for (var i = 0; i < max; ++i) // 8 AOEs in a wave, no need to iterate on all 16
            aoes.Add(_aoes[i]);
        return aoes;
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.LoyalFlameBlue or AID.LoyalFlameRed)
        {
            var isBlue = (AID)spell.Action.ID == AID.LoyalFlameBlue;
            AddAOEs(Module.Enemies(OID.OrbOfConflagrationBlue), spell, isBlue ? 2.2f : 6.2f);
            AddAOEs(Module.Enemies(OID.OrbOfConflagrationRed), spell, isBlue ? 6.2f : 2.2f);
            if (!isBlue)
                _aoes.Reverse();
        }
    }

    private void AddAOEs(List<Actor> orbs, ActorCastInfo spell, float delay)
    {
        for (var i = 0; i < orbs.Count; ++i)
        {
            var orb = orbs[i];
            _aoes.Add(new(circle, orb.Position, default, Module.CastFinishAt(spell, delay)));
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (_aoes.Count != 0 && (AID)spell.Action.ID is AID.BurnBlue or AID.BurnRed)
            _aoes.RemoveAt(0);
    }
}

class Drumbeat(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.Drumbeat));

abstract class Cleave(BossModule module, AID aid) : Components.SimpleAOEs(module, ActionID.MakeSpell(aid), new AOEShapeCone(65, 90.Degrees()));
class LeftwardTrisula(BossModule module) : Cleave(module, AID.LeftwardTrisula);
class RightwardParasu(BossModule module) : Cleave(module, AID.RightwardParasu);

class ErrantAkasa(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.ErrantAkasa), new AOEShapeCone(60, 45.Degrees()));
class CosmicWeave(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.CosmicWeave), 18);
class KarmicFlames(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.KarmicFlames), 20);
class YawningHells(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.YawningHells), 8);
class InfernalRedemption(BossModule module) : Components.RaidwideCastDelay(module, ActionID.MakeSpell(AID.InfernalRedemptionVisual), ActionID.MakeSpell(AID.InfernalRedemption), 1);

class DivineCall(BossModule module) : Components.StatusDrivenForcedMarch(module, 2, (uint)SID.ForwardMarch, (uint)SID.AboutFace, (uint)SID.LeftFace, (uint)SID.RightFace)
{
    private readonly LitPath _lit = module.FindComponent<LitPath>()!;
    private readonly LeftwardTrisula _aoe1 = module.FindComponent<LeftwardTrisula>()!;
    private readonly RightwardParasu _aoe2 = module.FindComponent<RightwardParasu>()!;

    private static readonly Dictionary<AID, string> directionhints = new()
    {
        { AID.DivineCall1, "Apply backwards march debuff" },
        { AID.DivineCall2, "Apply right march debuff" },
        { AID.DivineCall3, "Apply forwards march debuff" },
        { AID.DivineCall4, "Apply left march debuff" }
    };

    public override bool DestinationUnsafe(int slot, Actor actor, WPos pos)
    {
        return _aoe1.ActiveAOEs(slot, actor).Any(z => z.Shape.Check(pos, z.Origin, z.Rotation)) ||
        _aoe2.ActiveAOEs(slot, actor).Any(z => z.Shape.Check(pos, z.Origin, z.Rotation)) ||
        _lit.AOEs.Count != 0 && !_lit.ActiveAOEs(slot, actor).Any(z => z.Shape.Check(pos, z.Origin, z.Rotation));
    }

    public override void AddGlobalHints(GlobalHints hints)
    {
        if (Module.PrimaryActor.CastInfo != null)
            foreach (var entry in directionhints)
            {
                if (Module.PrimaryActor.CastInfo.IsSpell(entry.Key))
                {
                    hints.Add(entry.Value);
                    break;
                }
            }
    }

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        const string hint = "Aim into AOEs!";
        var movements = ForcedMovements(actor).ToList();
        if (movements.Count == 0)
            return;
        if (_aoe1.Casters.Count != 0 || _aoe2.Casters.Count != 0)
            base.AddHints(slot, actor, hints);
        else if (_lit.AOEs.Count != 0)
            if (DestinationUnsafe(slot, actor, movements.LastOrDefault().to))
                hints.Add(hint);
            else
                hints.Add(hint, false);
    }
}

class DaivadipaStates : StateMachineBuilder
{
    public DaivadipaStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<Drumbeat>()
            .ActivateOnEnter<LeftwardTrisula>()
            .ActivateOnEnter<RightwardParasu>()
            .ActivateOnEnter<InfernalRedemption>()
            .ActivateOnEnter<CosmicWeave>()
            .ActivateOnEnter<YawningHells>()
            .ActivateOnEnter<ErrantAkasa>()
            .ActivateOnEnter<KarmicFlames>()
            .ActivateOnEnter<LitPath>()
            .ActivateOnEnter<Burn>()
            .ActivateOnEnter<DivineCall>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.Fate, GroupID = 1763, NameID = 10269)]
public class Daivadipa(WorldState ws, Actor primary) : BossModule(ws, primary, new(-608, 811), new ArenaBoundsSquare(24.5f));
