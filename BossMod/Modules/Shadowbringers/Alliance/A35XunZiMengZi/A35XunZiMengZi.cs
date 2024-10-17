namespace BossMod.Shadowbringers.Alliance.A35XunZiMengZi;

class DeployArmaments1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.DeployArmaments1), new AOEShapeRect(50, 9));
class DeployArmaments2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.DeployArmaments2), new AOEShapeRect(50, 9));
class DeployArmaments3(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.DeployArmaments3), new AOEShapeRect(50, 9));
class DeployArmaments4(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.DeployArmaments4), new AOEShapeRect(50, 9));
class DeployArmaments5(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.DeployArmaments5), new AOEShapeRect(50, 9));
class DeployArmaments6(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.DeployArmaments6), new AOEShapeRect(50, 9));
class DeployArmaments7(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.DeployArmaments7), new AOEShapeRect(50, 9));
class DeployArmaments8(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.DeployArmaments8), new AOEShapeRect(50, 9));
class UniversalAssault(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.UniversalAssault));

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team", PrimaryActorOID = (uint)OID.XunZi, GroupType = BossModuleInfo.GroupType.CFC, GroupID = 779, NameID = 9921)]
public class A35XunZiMengZi(WorldState ws, Actor primary) : BossModule(ws, primary, new(800, 800), new ArenaBoundsSquare(25))
{
    private Actor? _mengZi;

    public Actor? XunZi() => PrimaryActor;
    public Actor? MengZi() => _mengZi;

    protected override void UpdateModule()
    {
        // TODO: this is an ugly hack, think how multi-actor fights can be implemented without it...
        // the problem is that on wipe, any actor can be deleted and recreated in the same frame
        _mengZi ??= StateMachine.ActivePhaseIndex == 0 ? Enemies(OID.MengZi).FirstOrDefault() : null;
    }

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actor(_mengZi);
    }
}
