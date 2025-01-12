namespace BossMod.Dawntrail.Raid.M02NHoneyBLovely;

class CallMeHoney(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.CallMeHoney));

class TemptingTwist1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.TemptingTwist1), new AOEShapeDonut(7, 30));
class TemptingTwist2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.TemptingTwist2), new AOEShapeDonut(7, 30));

class HoneyBeeline1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.HoneyBeeline1), new AOEShapeRect(30, 7, 30));
class HoneyBeeline2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.HoneyBeeline2), new AOEShapeRect(30, 7, 30));

class HoneyedBreeze(BossModule module) : Components.BaitAwayIcon(module, new AOEShapeCone(40, 15.Degrees()), (uint)IconID.HoneyedBreezeTB, ActionID.MakeSpell(AID.HoneyedBreeze), 5)
{
    public override void AddGlobalHints(GlobalHints hints)
    {
        if (CurrentBaits.Count > 0)
            hints.Add("Tankbuster cleave");
    }
}

class HoneyBLive(BossModule module) : Components.RaidwideCastDelay(module, ActionID.MakeSpell(AID.HoneyBLiveVisual), ActionID.MakeSpell(AID.HoneyBLive), 8.3f);
class Heartsore(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.Heartsore), 6);
class Heartsick(BossModule module) : Components.StackWithCastTargets(module, ActionID.MakeSpell(AID.Heartsick), 6, 4, 4);
class Loveseeker(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.Loveseeker), new AOEShapeCircle(10));
class BlowKiss(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BlowKiss), new AOEShapeCone(40, 60.Degrees()));
class HoneyBFinale(BossModule module) : Components.RaidwideCast(module, ActionID.MakeSpell(AID.HoneyBFinale));
class DropOfVenom(BossModule module) : Components.StackWithCastTargets(module, ActionID.MakeSpell(AID.DropOfVenom), 6, 8, 8);
class SplashOfVenom(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.SplashOfVenom), 6);
class BlindingLove1(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BlindingLove1), new AOEShapeRect(50, 4))
{
    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var aoes = ActiveCasters.Select((c, index) =>
            new AOEInstance(Shape, c.Position, c.CastInfo!.Rotation, Module.CastFinishAt(c.CastInfo),
            index < 2 ? Colors.Danger : Colors.AOE));

        return aoes;
    }
}

class BlindingLove2(BossModule module) : Components.SelfTargetedAOEs(module, ActionID.MakeSpell(AID.BlindingLove2), new AOEShapeRect(50, 4));
class HeartStruck1(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.HeartStruck1), 4);
class HeartStruck2(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.HeartStruck2), 6);
class HeartStruck3(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.HeartStruck3), 10, maxCasts: 8);

class Fracture(BossModule module) : Components.CastTowers(module, ActionID.MakeSpell(AID.Fracture), 4)
{
    public override void Update()
    {
        if (Towers.Count == 0)
            return;
        var forbidden = Raid.WithSlot().WhereActor(p => p.Statuses.Where(i => i.ID is ((uint)SID.HeadOverHeels) or ((uint)SID.HopelessDevotion)).Any()).Mask();
        foreach (ref var t in Towers.AsSpan())
            t.ForbiddenSoakers = forbidden;
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 987, NameID = 12685)]
public class M02NHoneyBLovely(WorldState ws, Actor primary) : BossModule(ws, primary, new(100, 100), new ArenaBoundsCircle(20));
