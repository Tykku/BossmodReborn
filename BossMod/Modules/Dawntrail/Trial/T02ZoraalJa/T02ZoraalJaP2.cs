﻿namespace BossMod.Dawntrail.Trial.T02ZoraalJaP2;

abstract class Donuts(BossModule module, uint aid) : Components.SimpleAOEs(module, aid, new AOEShapeDonut(10f, 30f));
class SmitingCircuitDonut(BossModule module) : Donuts(module, (uint)AID.SmitingCircuitDonut);
class HalfCircuitDonut(BossModule module) : Donuts(module, (uint)AID.HalfCircuitDonut);

abstract class Circles(BossModule module, uint aid) : Components.SimpleAOEs(module, aid, 10f);
class SmitingCircuitCircle(BossModule module) : Circles(module, (uint)AID.SmitingCircuitCircle);
class HalfCircuitCircle(BossModule module) : Circles(module, (uint)AID.HalfCircuitCircle);

class DawnOfAnAge(BossModule module) : Components.RaidwideCast(module, (uint)AID.DawnOfAnAge);
class BitterReaping(BossModule module) : Components.SingleTargetCast(module, (uint)AID.BitterReaping);
class Actualize(BossModule module) : Components.RaidwideCast(module, (uint)AID.Actualize);

abstract class HalfRect(BossModule module, uint aid) : Components.SimpleAOEs(module, aid, new AOEShapeRect(60f, 30f));
class HalfFull(BossModule module) : HalfRect(module, (uint)AID.HalfFull)
{
    private readonly ChasmOfVollok _aoe = module.FindComponent<ChasmOfVollok>()!;
    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        return Casters.Count != 0 && _aoe.AOEs.Count == 0 ? new AOEInstance[1] { Casters[0] } : [];
    }
}

class HalfCircuitRect(BossModule module) : HalfRect(module, (uint)AID.HalfCircuitRect);

class FireIII(BossModule module) : Components.SpreadFromIcon(module, (uint)IconID.Spreadmarker, (uint)AID.FireIII, 6f, 5.1f);
class DutysEdge(BossModule module) : Components.LineStack(module, (uint)AID.DutysEdgeMarker, (uint)AID.DutysEdge, 5.4f, 100f, minStackSize: 8, maxStackSize: 8, maxCasts: 4, markerIsFinalTarget: false);

// P2 is a checkpoint so we can't make it one module since it would prevent reloading the module incase of wipes
class T02ZoraalJaP2States : StateMachineBuilder
{
    public T02ZoraalJaP2States(BossModule module) : base(module)
    {
        TrivialPhase()
            .ActivateOnEnter<DawnOfAnAgeArenaChange>()
            .ActivateOnEnter<SmitingCircuitDonut>()
            .ActivateOnEnter<SmitingCircuitCircle>()
            .ActivateOnEnter<DawnOfAnAge>()
            .ActivateOnEnter<BitterReaping>()
            .ActivateOnEnter<ChasmOfVollok>()
            .ActivateOnEnter<ForgedTrack>()
            .ActivateOnEnter<Actualize>()
            .ActivateOnEnter<HalfFull>()
            .ActivateOnEnter<HalfCircuitRect>()
            .ActivateOnEnter<HalfCircuitDonut>()
            .ActivateOnEnter<HalfCircuitCircle>()
            .ActivateOnEnter<FireIII>()
            .ActivateOnEnter<DutysEdge>();
    }
}

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "The Combat Reborn Team (Malediktus, LTS)", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 995, NameID = 12882, SortOrder = 2)]
public class T02ZoraalJaP2(WorldState ws, Actor primary) : T02ZoraalJa.ZoraalJa(ws, primary);
