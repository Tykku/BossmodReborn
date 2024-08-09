﻿namespace BossMod.Dawntrail.Savage.M03SBruteBomber;

class BrutalImpact(BossModule module) : Components.CastCounter(module, ActionID.MakeSpell(AID.BrutalImpactAOE));

[ModuleInfo(BossModuleInfo.Maturity.Verified, Contributors = "veyn", GroupType = BossModuleInfo.GroupType.CFC, GroupID = 990, NameID = 13356, PlanLevel = 100)]
public class M03SBruteBomber(WorldState ws, Actor primary) : BossModule(ws, primary, new(100, 100), new ArenaBoundsSquare(15));
