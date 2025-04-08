namespace BossMod.RealmReborn.Dungeon.D02Deepcroft.D022GalvanthTheDominator;

public enum OID : uint
{
    //Boss
    Boss = 0x4C, // Galvanth The Dominator

    //Adds
    InconspicuousImp = 0x7D, // Spawn during fight
    DeepcroftMiteling = 0x7F, // Spawn during fight
    SkeletonSoldier = 0x7E, // Spawn during fight
}

public enum AID : uint
{
    //Boss
    AutoAttackBoss = 870, // Boss->player, no cast
    Water = 971, // Boss->player, 1.0s cast, single target
    DrainTouch = 988, // Boss->player, no cast, single target
    MindBlast = 987, // Boss->self, 5.0s cast, range 9.95 circle aoe

    //BaleenGuard
    //AutoAttackGuard = 870, // Guard->player, no cast
    HellSlash = 341, // SkeletonSoldier->player, no cast, single target
}

class MindBlast(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.MindBlast), new AOEShapeCircle(9.95f));

class D022GalvanthTheDominatorStates : StateMachineBuilder
{
    public D022GalvanthTheDominatorStates(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<MindBlast>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.WIP, Contributors = "Chuggalo", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 2, NameID = 73)]
public class D022GalvanthTheDominator(WorldState ws, Actor primary) : BossModule(ws, primary, new(-52.765f, -12.789f), new ArenaBoundsCircle(18f))
{
    private static readonly uint[] trash = [(uint)OID.InconspicuousImp, (uint)OID.DeepcroftMiteling, (uint)OID.SkeletonSoldier];

    protected override void CalculateModuleAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var count = hints.PotentialTargets.Count;
        for (var i = 0; i < count; ++i)
        {
            var e = hints.PotentialTargets[i];
            e.Priority = e.Actor.OID switch
            {
                (uint)OID.InconspicuousImp => 2,
                (uint)OID.Boss => 1,
                _ => 0
            };
        }
    }

    protected override void DrawEnemies(int pcSlot, Actor pc)
    {
        Arena.Actor(PrimaryActor);
        Arena.Actors(Enemies(trash));
    }
}
