namespace BossMod.Dawntrail.Alliance.A12Fafnir;

class DarkMatterBlast(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.DarkMatterBlast));
class HorridRoar2(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.HorridRoar2), 4);
class HorridRoar3(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.HorridRoar3), 8);
class SpikeFlail(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.SpikeFlail), new AOEShapeCone(80, 135.Degrees()));
class Touchdown(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Touchdown), new AOEShapeCircle(27));

class HurricaneWing1(BossModule module) : Components.ConcentricAOEs(module, _shapes)
{
    private static readonly AOEShape[] _shapes = [new AOEShapeCircle(9), new AOEShapeDonut(9, 16), new AOEShapeDonut(16, 23), new AOEShapeDonut(23, 30)];

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.HurricaneWing7)
            AddSequence(Arena.Center, Module.CastFinishAt(spell));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (Sequences.Count > 0)
        {
            var order = (AID)spell.Action.ID switch
            {
                AID.HurricaneWing7 => 0,
                AID.HurricaneWing8 => 1,
                AID.HurricaneWing9 => 2,
                AID.HurricaneWing10 => 3,
                _ => -1
            };
            AdvanceSequence(order, Arena.Center, WorldState.FutureTime(3));
        }
    }
}

class HurricaneWing2(BossModule module) : Components.ConcentricAOEs(module, _shapes)
{
    private static readonly AOEShape[] _shapes = [new AOEShapeCircle(9), new AOEShapeDonut(9, 16), new AOEShapeDonut(16, 23), new AOEShapeDonut(23, 30)];

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.HurricaneWing11)
            AddSequence(Arena.Center, Module.CastFinishAt(spell));
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (Sequences.Count > 0)
        {
            var order = (AID)spell.Action.ID switch
            {
                AID.HurricaneWing11 => 0,
                AID.HurricaneWing12 => 1,
                AID.HurricaneWing13 => 2,
                AID.HurricaneWing14 => 3,
                _ => -1
            };
            AdvanceSequence(order, Arena.Center, WorldState.FutureTime(3));
        }
    }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team (LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 1015, NameID = 13662)]
public class A12Fafnir(WorldState ws, Actor primary) : BossModule(ws, primary, new(-500, 600), new ArenaBoundsCircle(35));
