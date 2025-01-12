namespace BossMod.Shadowbringers.Alliance.A32HanselGretel;

class Wail1(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.Wail1));
class Wail2(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.Wail2));

class CripplingBlow1(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.CripplingBlow1));
class CripplingBlow2(BossModule module) : Components.SingleTargetCast(module, ActionID.MakeSpell(AID.CripplingBlow2));

class BloodySweep3(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BloodySweep3), new AOEShapeRect(50, 50, +5, 90.Degrees()));
class BloodySweep4(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BloodySweep4), new AOEShapeRect(50, 50, +5, -90.Degrees()));
class BloodySweep7(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BloodySweep7), new AOEShapeRect(50, 50, +10, 90.Degrees()));
class BloodySweep8(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BloodySweep8), new AOEShapeRect(50, 50, +10, -90.Degrees()));

class SeedOfMagicAlpha2(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.SeedOfMagicAlpha2), 5);
class RiotOfMagic2(BossModule module) : Components.StackWithCastTargets(module, ActionID.MakeSpell(AID.RiotOfMagic2), 5, 8);

class PassingLance3(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.PassingLance3), new AOEShapeRect(50, 12, 50));
class Explosion(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Explosion), new AOEShapeRect(4, 25, 25));
class UnevenFooting(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.UnevenFooting), new AOEShapeCircle(17));

class HungryLance1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.HungryLance1), new AOEShapeCone(40, 60.Degrees()));
class HungryLance2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.HungryLance2), new AOEShapeCone(40, 60.Degrees()));

class Breakthrough1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Breakthrough1), new AOEShapeRect(50, 50, +10, 90.Degrees()));
class SeedOfMagicBeta3(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.SeedOfMagicBeta3), 5);
class Lamentation(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.Lamentation));

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "The Combat Reborn Team", PrimaryActorOID = (uint)OID.Gretel, GroupType = BossModuleInfo.GroupType.CFC, GroupID = 779, NameID = 9990)]
public class A32HanselGretel(WorldState ws, Actor primary) : BossModule(ws, primary, new(-800, -951), new ArenaBoundsCircle(25))
{
    private Actor? _hansel;

    public Actor? Gretel() => PrimaryActor;
    public Actor? Hansel() => _hansel;

    protected override void UpdateModule()
    {
        // TODO: this is an ugly hack, think how multi-actor fights can be implemented without it...
        // the problem is that on wipe, any actor can be deleted and recreated in the same frame
        _hansel ??= StateMachine.ActivePhaseIndex == 0 ? Enemies(OID.Hansel).FirstOrDefault() : null;
    }

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actor(_hansel);
    }
}
