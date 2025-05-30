﻿using ImGuiNET;

namespace BossMod.ReplayVisualization;

// a set of columns containing cast events (typically by non-players)
// by default contains a single column showing all actions from all sources, but extra columns can be added and per-column filters can be assigned
public sealed class ColumnEnemiesCastEvents : Timeline.ColumnGroup
{
    private readonly StateMachineTree _tree;
    private readonly List<int> _phaseBranches;
    private readonly Replay.Encounter _encounter;
    private readonly BossModuleRegistry.Info? _moduleInfo;
    private readonly List<Replay.Action> _actions;
    private readonly Dictionary<ActionID, List<(Replay.Participant source, BitMask cols)>> _filters = []; // [action][sourceid]

    public ColumnEnemiesCastEvents(Timeline timeline, StateMachineTree tree, List<int> phaseBranches, Replay replay, Replay.Encounter enc)
        : base(timeline)
    {
        //Name = "Enemy cast events";
        _tree = tree;
        _phaseBranches = phaseBranches;
        _encounter = enc;
        _moduleInfo = BossModuleRegistry.FindByOID(enc.OID);
        _actions = [.. replay.EncounterActions(enc).Where(a => a.Source.Type is not (ActorType.Player or ActorType.Pet or ActorType.Chocobo or ActorType.Buddy))];
        foreach (var a in _actions)
        {
            var f = _filters.GetOrAdd(a.ID);
            if (!f.Any(e => e.source == a.Source))
                f.Add((a.Source, new(1)));
        }
        AddColumn();
        RebuildEvents();
    }

    public void DrawConfig(UITree tree)
    {
        if (ImGui.Button("Add new!"))
            AddColumn();

        var needRebuild = false;
        foreach (var na in tree.Nodes(_filters, kv => new($"{kv.Key} ({_moduleInfo?.ActionIDType?.GetEnumName(kv.Key.ID)})")))
        {
            foreach (ref var v in na.Value.AsSpan())
            {
                needRebuild |= DrawConfigColumns(ref v.cols, $"{ReplayUtils.ParticipantString(v.source, v.source.WorldExistence.FirstOrDefault().Start)} ({_moduleInfo?.ObjectIDType?.GetEnumName(v.source.OID)})");
            }
        }

        if (needRebuild)
            RebuildEvents();
    }

    private void AddColumn()
    {
        Add(new ColumnGenericHistory(Timeline, _tree, _phaseBranches));
    }

    private bool DrawConfigColumns(ref BitMask mask, string name)
    {
        var changed = false;
        for (var c = 0; c < Columns.Count; ++c)
        {
            var set = mask[c];
            if (ImGui.Checkbox($"###{name}/{c}", ref set))
            {
                mask[c] = set;
                changed = true;
            }
            ImGui.SameLine();
        }
        ImGui.TextUnformatted(name);
        return changed;
    }

    private void RebuildEvents()
    {
        foreach (var c in Columns.Cast<ColumnGenericHistory>())
            c.Entries.Clear();

        foreach (var a in _actions)
        {
            var cols = _filters[a.ID].Find(c => c.source == a.Source).cols;
            if (cols.None())
                continue;

            var name = $"{a.ID} ({_moduleInfo?.ActionIDType?.GetEnumName(a.ID.ID)}) {ReplayUtils.ParticipantString(a.Source, a.Timestamp)} -> {ReplayUtils.ParticipantString(a.MainTarget, a.Timestamp)} #{a.GlobalSequence}";
            var color = EventColor(a);
            foreach (var c in cols.SetBits())
            {
                var col = (ColumnGenericHistory)Columns[c];
                col.AddHistoryEntryDot(_encounter.Time.Start, a.Timestamp, name, color).AddActionTooltip(a);
            }
        }
    }

    private uint EventColor(Replay.Action action)
    {
        var phys = false;
        var magic = false;
        foreach (var t in action.Targets.Where(t => t.Target.Type is ActorType.Player or ActorType.Buddy))
        {
            foreach (var e in t.Effects.Where(e => e.Type is ActionEffectType.Damage or ActionEffectType.BlockedDamage or ActionEffectType.ParriedDamage))
            {
                switch (e.DamageType)
                {
                    case DamageType.Slashing:
                    case DamageType.Piercing:
                    case DamageType.Blunt:
                    case DamageType.Shot:
                        phys = true;
                        break;
                    case DamageType.Magic:
                        magic = true;
                        break;
                    default:
                        phys = magic = true; // TODO: reconsider
                        break;
                }
            }
        }
        return phys ? (magic ? Colors.TextColor1 : Colors.TextColor5) : (magic ? Colors.TextColor6 : Colors.TextColor7);
    }
}
