﻿using BossMod.AST;

namespace BossMod;

// utility that recalculates ai hints based on different data sources (eg active bossmodule, etc)
// when there is no active bossmodule (eg in outdoor or on trash), we try to guess things based on world state (eg actor casts)
public sealed class AIHintsBuilder : IDisposable
{
    private const float RaidwideSize = 30;
    private const float HalfWidth = 0.5f;
    private readonly WorldState _ws;
    private readonly BossModuleManager _bmm;
    private readonly EventSubscriptions _subscriptions;
    private readonly Dictionary<ulong, (Actor Caster, Actor? Target, AOEShape Shape, bool IsCharge)> _activeAOEs = [];
    private ArenaBoundsCircle? _activeFateBounds;

    public AIHintsBuilder(WorldState ws, BossModuleManager bmm)
    {
        _ws = ws;
        _bmm = bmm;
        _subscriptions = new
        (
            ws.Actors.CastStarted.Subscribe(OnCastStarted),
            ws.Actors.CastFinished.Subscribe(OnCastFinished),
            ws.Client.ActiveFateChanged.Subscribe(_ => _activeFateBounds = null)
        );
    }

    public void Dispose() => _subscriptions.Dispose();

    public void Update(AIHints hints, int playerSlot)
    {
        hints.Clear();
        var player = _ws.Party[playerSlot];
        if (player != null)
        {
            var playerAssignment = Service.Config.Get<PartyRolesConfig>()[_ws.Party.Members[playerSlot].ContentId];
            var activeModule = _bmm.ActiveModule?.StateMachine.ActivePhase != null ? _bmm.ActiveModule : null;
            hints.FillPotentialTargets(_ws, playerAssignment == PartyRolesConfig.Assignment.MT || playerAssignment == PartyRolesConfig.Assignment.OT && !_ws.Party.WithoutSlot().Any(p => p != player && p.Role == Role.Tank));
            hints.RecommendedRangeToTarget = player.Role is Role.Melee or Role.Tank ? 3 : 25;
            if (activeModule != null)
                activeModule.CalculateAIHints(playerSlot, player, playerAssignment, hints);
            else
                CalculateAutoHints(hints, player);
        }
        hints.Normalize();
    }

    private void CalculateAutoHints(AIHints hints, Actor player)
    {
        if (_ws.Client.ActiveFate.ID != 0 && player.Level <= Service.LuminaRow<Lumina.Excel.GeneratedSheets.Fate>(_ws.Client.ActiveFate.ID)?.ClassJobLevelMax)
        {
            hints.Center = new(_ws.Client.ActiveFate.Center.XZ());
            hints.Bounds = (_activeFateBounds ??= new ArenaBoundsCircle(_ws.Client.ActiveFate.Radius));
        }
        else
        {
            hints.Center = player.Position.Rounded(5);
            // keep default bounds
        }

        foreach (var aoe in _activeAOEs.Values)
        {
            var target = aoe.Target?.Position ?? aoe.Caster.CastInfo!.LocXZ;
            var rot = aoe.Caster.CastInfo!.Rotation;
            var finishAt = _ws.FutureTime(aoe.Caster.CastInfo.NPCRemainingTime);
            if (aoe.IsCharge)
            {
                hints.AddForbiddenZone(ShapeDistance.Rect(aoe.Caster.Position, target, ((AOEShapeRect)aoe.Shape).HalfWidth), finishAt);
            }
            else
            {
                hints.AddForbiddenZone(aoe.Shape, target, rot, finishAt);
            }
        }
    }

    private void OnCastStarted(Actor actor)
    {
        if (actor.Type != ActorType.Enemy || actor.IsAlly)
            return;
        var data = actor.CastInfo!.IsSpell() ? Service.LuminaRow<Lumina.Excel.GeneratedSheets.Action>(actor.CastInfo.Action.ID) : null;
        if (data == null || data.CastType == 1)
            return;
        if (data.CastType is 2 or 5 && data.EffectRange >= RaidwideSize)
            return;
        if (actor.CastInfo!.IsSpell((AID)27503)) // friendly AOE in aitiascope that gets dodged for some reason
            return;
        AOEShape? shape = data.CastType switch
        {
            2 => new AOEShapeCircle(data.EffectRange), // used for some point-blank aoes and enemy location-targeted - does not add caster hitbox
            3 => new AOEShapeCone(data.EffectRange + actor.HitboxRadius, DetermineConeAngle(data) * HalfWidth),
            4 => new AOEShapeRect(data.EffectRange + actor.HitboxRadius, data.XAxisModifier * HalfWidth),
            5 => new AOEShapeCircle(data.EffectRange + actor.HitboxRadius),
            //6 => custom shapes
            //7 => new AOEShapeCircle(data.EffectRange), - used for player ground-targeted circles a-la asylum
            8 => new AOEShapeRect((actor.CastInfo!.LocXZ - actor.Position).Length(), data.XAxisModifier * HalfWidth),
            10 => new AOEShapeDonut(3, data.EffectRange), // TODO: find a way to determine inner radius (omen examples: 28762 - 4/40 - gl_sircle_4004bp1)
            11 => new AOEShapeCross(data.EffectRange, data.XAxisModifier * HalfWidth),
            12 => new AOEShapeRect(data.EffectRange, data.XAxisModifier * HalfWidth),
            13 => new AOEShapeCone(data.EffectRange, DetermineConeAngle(data) * HalfWidth),
            _ => null
        };
        if (shape == null)
        {
            Service.Log($"[AutoHints] Unknown cast type {data.CastType} for {actor.CastInfo.Action}");
            return;
        }
        var target = _ws.Actors.Find(actor.CastInfo.TargetID);
        _activeAOEs[actor.InstanceID] = (actor, target, shape, data.CastType == 8);
    }

    private void OnCastFinished(Actor actor) => _activeAOEs.Remove(actor.InstanceID);

    private Angle DetermineConeAngle(Lumina.Excel.GeneratedSheets.Action data)
    {
        var omen = data.Omen.Value;
        if (omen == null)
        {
            Service.Log($"[AutoHints] No omen data for {data.RowId} '{data.Name}'...");
            return 180.Degrees();
        }
        var path = omen.Path.ToString();
        var pos = path.IndexOf("fan", StringComparison.Ordinal);
        if (pos < 0 || pos + 6 > path.Length)
        {
            Service.Log($"[AutoHints] Can't determine angle from omen ({path}/{omen.PathAlly}) for {data.RowId} '{data.Name}'...");
            return 180.Degrees();
        }

        if (!int.TryParse(path.AsSpan(pos + 3, 3), out var angle))
        {
            Service.Log($"[AutoHints] Can't determine angle from omen ({path}/{omen.PathAlly}) for {data.RowId} '{data.Name}'...");
            return 180.Degrees();
        }

        return angle.Degrees();
    }
}
