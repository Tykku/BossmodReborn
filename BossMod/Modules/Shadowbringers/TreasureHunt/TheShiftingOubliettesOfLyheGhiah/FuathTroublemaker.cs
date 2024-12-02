namespace BossMod.Shadowbringers.TreasureHunt.ShiftingOubliettesOfLyheGhiah.FuathTroublemaker;

public enum OID : uint
{
    Boss = 0x302F, //R=3.25
    FuathTrickster = 0x3033, // R0.750
    Helper = 0x233C
}

public enum AID : uint
{
    AutoAttack = 21733, // Boss->player, no cast, single-target

    FrigidNeedleVisual = 21739, // Boss->self, 3.0s cast, single-target
    FrigidNeedle = 21740, // Helper->self, 3.0s cast, range 40 width 5 cross
    SpittleVisual = 21735, // Boss->self, no cast, single-target
    Spittle = 21736, // Helper->location, 4.0s cast, range 8 circle
    CroakingChorus = 21738, // Boss->self, 3.0s cast, single-target, calls adds
    ToyHammer = 21734, // Boss->player, 4.0s cast, single-target
    Hydrocannon = 21737, // Boss->players, 5.0s cast, range 6 circle

    Telega = 9630 // FuathTrickster->self, no cast, single-target, bonus adds disappear
}

class CroakingChorus(BossModule module) : Components.CastHint(module, ActionID.MakeSpell(AID.CroakingChorus), "Calls adds");
class FrigidNeedle(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.FrigidNeedle), new AOEShapeCross(40, 2.5f));
class Spittle(BossModule module) : Components.LocationTargetedAOEs(module, ActionID.MakeSpell(AID.Spittle), 8);
class ToyHammer(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.ToyHammer));
class Hydrocannon(BossModule module) : Components.StackWithCastTargets(module, ActionID.MakeSpell(AID.Hydrocannon), 6, 8, 8);

class FuathTroublemakerStates : StateMachineBuilder
{
    public FuathTroublemakerStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<CroakingChorus>()
            .ActivateOnEnter<ToyHammer>()
            .ActivateOnEnter<Spittle>()
            .ActivateOnEnter<Hydrocannon>()
            .ActivateOnEnter<FrigidNeedle>()
            .Raw.Update = () => Module.WorldState.Actors.Where(x => !x.IsAlly && x.IsTargetable).All(x => x.IsDeadOrDestroyed);
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "Malediktus", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 745, NameID = 9786)]
public class FuathTroublemaker(WorldState ws, Actor primary) : BossModule(ws, primary, new(100, 100), new ArenaBoundsCircle(19))
{
    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(OID.FuathTrickster), Colors.Vulnerable);
    }

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        for (var i = 0; i < hints.PotentialTargets.Count; ++i)
        {
            var e = hints.PotentialTargets[i];
            e.Priority = (OID)e.Actor.OID switch
            {
                OID.FuathTrickster => 1,
                _ => 0
            };
        }
    }
}
