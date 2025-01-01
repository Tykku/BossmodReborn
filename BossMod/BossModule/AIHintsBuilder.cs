﻿namespace BossMod;

// utility that recalculates ai hints based on different data sources (eg active bossmodule, etc)
// when there is no active bossmodule (eg in outdoor or on trash), we try to guess things based on world state (eg actor casts)
public sealed class AIHintsBuilder : IDisposable
{
    private const float RaidwideSize = 30;
    private const float HalfWidth = 0.5f;
    public readonly Pathfinding.ObstacleMapManager Obstacles;
    private readonly WorldState _ws;
    private readonly BossModuleManager _bmm;
    private readonly ZoneModuleManager _zmm;
    private readonly EventSubscriptions _subscriptions;
    private readonly Dictionary<ulong, (Actor Caster, Actor? Target, AOEShape Shape, bool IsCharge)> _activeAOEs = [];
    private ArenaBoundsCircle? _activeFateBounds;
    private static readonly HashSet<uint> ignore = [27503, 33626]; // action IDs that the AI should ignore

    public AIHintsBuilder(WorldState ws, BossModuleManager bmm, ZoneModuleManager zmm)
    {
        _ws = ws;
        _bmm = bmm;
        _zmm = zmm;
        Obstacles = new(ws);
        _subscriptions = new
        (
            ws.Actors.CastStarted.Subscribe(OnCastStarted),
            ws.Actors.CastFinished.Subscribe(OnCastFinished),
            ws.Client.ActiveFateChanged.Subscribe(_ => _activeFateBounds = null)
        );
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
        Obstacles.Dispose();
    }

    public void Update(AIHints hints, int playerSlot, float maxCastTime)
    {
        hints.Clear();
        hints.MaxCastTimeEstimate = maxCastTime;
        var player = _ws.Party[playerSlot];
        if (player != null)
        {
            var playerAssignment = Service.Config.Get<PartyRolesConfig>()[_ws.Party.Members[playerSlot].ContentId];
            var activeModule = _bmm.ActiveModule?.StateMachine.ActivePhase != null ? _bmm.ActiveModule : null;
            hints.FillPotentialTargets(_ws, playerAssignment == PartyRolesConfig.Assignment.MT || playerAssignment == PartyRolesConfig.Assignment.OT && !_ws.Party.WithoutSlot().Any(p => p != player && p.Role == Role.Tank));
            if (activeModule != null)
            {
                activeModule.CalculateAIHints(playerSlot, player, playerAssignment, hints);
            }
            else
            {
                CalculateAutoHints(hints, player);
                _zmm.ActiveModule?.CalculateAIHints(playerSlot, player, hints);
            }
        }
        hints.Normalize();
    }

    private void CalculateAutoHints(AIHints hints, Actor player)
    {
        var inFate = _ws.Client.ActiveFate.ID != 0 && (_ws.Party.Player()?.Level <= Service.LuminaRow<Lumina.Excel.Sheets.Fate>(_ws.Client.ActiveFate.ID)?.ClassJobLevelMax
        || Service.LuminaRow<Lumina.Excel.Sheets.Fate>(_ws.Client.ActiveFate.ID)?.EurekaFate == 1); // TODO: find out how to get the current player elemental level
        var center = inFate ? _ws.Client.ActiveFate.Center : player.PosRot.XYZ();
        var (e, bitmap) = Obstacles.Find(center);
        var resolution = bitmap?.PixelSize ?? 0.5f;
        if (inFate)
        {
            hints.PathfindMapCenter = new(_ws.Client.ActiveFate.Center.XZ());
            hints.PathfindMapBounds = (_activeFateBounds ??= new ArenaBoundsCircle(_ws.Client.ActiveFate.Radius, resolution));
            if (e != null && bitmap != null)
            {
                var originCell = (hints.PathfindMapCenter - e.Origin) / resolution;
                var originX = (int)originCell.X;
                var originZ = (int)originCell.Z;
                var halfSize = (int)(_ws.Client.ActiveFate.Radius / resolution);
                hints.PathfindMapObstacles = new(bitmap, new(originX - halfSize, originZ - halfSize, originX + halfSize, originZ + halfSize));
            }
        }
        else if (e != null && bitmap != null)
        {
            var originCell = (player.Position - e.Origin) / resolution;
            var originX = (int)originCell.X;
            var originZ = (int)originCell.Z;
            // if player is too close to the border, adjust origin
            originX = Math.Min(originX, bitmap.Width - e.ViewWidth);
            originZ = Math.Min(originZ, bitmap.Height - e.ViewHeight);
            originX = Math.Max(originX, e.ViewWidth);
            originZ = Math.Max(originZ, e.ViewHeight);
            // TODO: consider quantizing even more, to reduce jittering when player moves?..
            hints.PathfindMapCenter = e.Origin + resolution * new WDir(originX, originZ);
            hints.PathfindMapBounds = new ArenaBoundsRect(e.ViewWidth * resolution, e.ViewHeight * resolution, MapResolution: resolution); // note: we don't bother caching these bounds, they are very lightweight
            hints.PathfindMapObstacles = new(bitmap, new(originX - e.ViewWidth, originZ - e.ViewHeight, originX + e.ViewWidth, originZ + e.ViewHeight));
        }
        else
        {
            hints.PathfindMapCenter = player.Position.Rounded(5);
            // try to keep player near grid center
            var playerOffset = player.Position - hints.PathfindMapCenter;
            if (playerOffset.X < -1.25f)
                hints.PathfindMapCenter.X -= 2.5f;
            else if (playerOffset.X > 1.25f)
                hints.PathfindMapCenter.X += 2.5f;
            if (playerOffset.Z < -1.25f)
                hints.PathfindMapCenter.Z -= 2.5f;
            else if (playerOffset.Z > 1.25f)
                hints.PathfindMapCenter.Z += 2.5f;
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
        if (actor.Type is not ActorType.Enemy and not ActorType.Helper || actor.IsAlly)
            return;
        var data = actor.CastInfo!.IsSpell() ? Service.LuminaRow<Lumina.Excel.Sheets.Action>(actor.CastInfo.Action.ID) : null;
        var dat = data!.Value;
        if (data == null || dat.CastType == 1)
            return;
        if (dat.CastType is 2 or 5 && dat.EffectRange >= RaidwideSize)
            return;
        if (ignore.Contains(actor.CastInfo!.Action.ID))
            return;
        AOEShape? shape = dat.CastType switch
        {
            2 => new AOEShapeCircle(dat.EffectRange), // used for some point-blank aoes and enemy location-targeted - does not add caster hitbox
            3 => new AOEShapeCone(dat.EffectRange + actor.HitboxRadius, DetermineConeAngle(dat) * HalfWidth),
            4 => new AOEShapeRect(dat.EffectRange + actor.HitboxRadius, dat.XAxisModifier * HalfWidth),
            5 => new AOEShapeCircle(dat.EffectRange + actor.HitboxRadius),
            //6 => custom shapes
            //7 => new AOEShapeCircle(data.EffectRange), - used for player ground-targeted circles a-la asylum
            8 => new AOEShapeRect((actor.CastInfo!.LocXZ - actor.Position).Length(), dat.XAxisModifier * HalfWidth),
            10 => new AOEShapeDonut(3, dat.EffectRange),
            11 => new AOEShapeCross(dat.EffectRange, dat.XAxisModifier * HalfWidth),
            12 => new AOEShapeRect(dat.EffectRange, dat.XAxisModifier * HalfWidth),
            13 => new AOEShapeCone(dat.EffectRange, DetermineConeAngle(dat) * HalfWidth),
            _ => null
        };
        if (shape == null)
        {
            Service.Log($"[AutoHints] Unknown cast type {dat.CastType} for {actor.CastInfo.Action}");
            return;
        }
        var target = _ws.Actors.Find(actor.CastInfo.TargetID);
        _activeAOEs[actor.InstanceID] = (actor, target, shape, dat.CastType == 8);
    }

    private void OnCastFinished(Actor actor) => _activeAOEs.Remove(actor.InstanceID);

    private Angle DetermineConeAngle(Lumina.Excel.Sheets.Action data)
    {
        var omen = data.Omen.ValueNullable;
        var om = omen!.Value;
        if (omen == null)
        {
            Service.Log($"[AutoHints] No omen data for {data.RowId} '{data.Name}'...");
            return 180.Degrees();
        }
        var path = om.Path.ToString();
        var pos = path.IndexOf("fan", StringComparison.Ordinal);
        if (pos < 0 || pos + 6 > path.Length || !int.TryParse(path.AsSpan(pos + 3, 3), out var angle))
        {
            Service.Log($"[AutoHints] Can't determine angle from omen ({path}/{om.PathAlly}) for {data.RowId} '{data.Name}'...");
            return 180.Degrees();
        }
        return angle.Degrees();
    }

    // private float DetermineDonutInner(Lumina.Excel.Sheets.Action data)
    // {
    //     var omen = data.Omen.ValueNullable;
    //     if (omen == null)
    //     {
    //         Service.Log($"[AutoHints] No omen data for {data.RowId} '{data.Name}'...");
    //         return 0;
    //     }
    //     var path = omen.Value.Path.ToString();
    //     var pos = path.IndexOf("sircle_", StringComparison.Ordinal);
    //     if (pos >= 0 && pos + 11 <= path.Length && int.TryParse(path.AsSpan(pos + 9, 2), out var inner))
    //         return inner;

    //     pos = path.IndexOf("circle", StringComparison.Ordinal);
    //     if (pos >= 0 && pos + 10 <= path.Length && int.TryParse(path.AsSpan(pos + 8, 2), out inner))
    //         return inner;

    //     Service.Log($"[AutoHints] Can't determine inner radius from omen ({path}/{omen.Value.PathAlly}) for {data.RowId} '{data.Name}'...");
    //     return 0;
    // }
}
