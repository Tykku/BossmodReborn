namespace BossMod.Heavensward.DeepDungeon.PalaceOfTheDead.DD50EddaBlackbosom;

public enum OID : uint
{
    Boss = 0x16C6, // R1.500, x1
    DemonButler = 0x16E9, // R1.200, x4
    GargoyleSteward = 0x16E8 // R2.300, x4
}

public enum AID : uint
{
    AutoAttack = 6497, // Boss->player, no cast, single-target

    BlackHoneymoon = 6402, // Boss->location, 3.0s cast, range 40 circle
    ColdFeet = 6403, // Boss->self, 3.0s cast, range 40 circle // gaze mechanic
    DarkHarvest = 6400, // Boss->player, 2.0s cast, single-target
    Desolation = 6404, // GargoyleSteward->self, 4.0s cast, range 55+R width 6 rect
    InHealthCircle = 6398, // Boss->self, 4.5s cast, range 16 circle
    InHealthDonut = 6399, // Boss->self, 4.5s cast, range 50+R circle // actually a donut, inner is ~2.5f
    TerrorEye = 6405 // DemonButler->location, 4.0s cast, range 6 circle
}

class BlackHoneymoon(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.BlackHoneymoon));
class ColdFeet(BossModule module) : Components.CastGaze(module, ActionID.MakeSpell(AID.ColdFeet));
class DarkHarvest(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.DarkHarvest), "Tankbuster");
class Desolation(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.Desolation), new AOEShapeRect(57.3f, 3));
class InHeathCircle(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.InHealthCircle), 16);
class InHeathDonut(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.InHealthDonut), new AOEShapeDonut(2.5f, 50));
class TerrorEye(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.TerrorEye), 6);

class DD50EddaBlackbosomStates : StateMachineBuilder
{
    public DD50EddaBlackbosomStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<BlackHoneymoon>()
            .ActivateOnEnter<ColdFeet>()
            .ActivateOnEnter<DarkHarvest>()
            .ActivateOnEnter<Desolation>()
            .ActivateOnEnter<InHeathCircle>()
            .ActivateOnEnter<InHeathDonut>()
            .ActivateOnEnter<TerrorEye>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Contributed, Contributors = "LegendofIceman", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 178, NameID = 5038)]
public class DD50EddaBlackbosom(WorldState ws, Actor primary) : BossModule(ws, primary, new(300, 375), new ArenaBoundsCircle(25));
