﻿namespace BossMod.Endwalker.Savage.P5SProtoCarbuncle;

class VenomTowers(BossModule module) : BossComponent(module)
{
    private List<WDir> _activeTowerOffsets = new();

    private static readonly float _radius = 3; // not sure...
    private static readonly float _meleeOffset = 7;
    private static readonly float _rangedOffset = 11; // not sure...

    public bool Active => _activeTowerOffsets.Count > 0;

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        foreach (var t in _activeTowerOffsets)
        {
            var origin = Module.Bounds.Center + t;
            Arena.AddCircle(origin, _radius, Raid.WithoutSlot().InRadius(origin, _radius).Any() ? ArenaColor.Safe : ArenaColor.Danger);
        }
    }

    public override void OnEventEnvControl(byte index, uint state)
    {
        WDir offset = index switch
        {
            3 => new(-_rangedOffset, -_rangedOffset),
            4 => new(+_rangedOffset, -_rangedOffset),
            5 => new(0, -_meleeOffset),
            6 => new(-_meleeOffset, 0),
            7 => new(+_meleeOffset, 0),
            8 => new(0, +_meleeOffset),
            9 => new(-_rangedOffset, +_rangedOffset),
            10 => new(+_rangedOffset, +_rangedOffset),
            _ => new()
        };
        if (offset == new WDir())
            return;

        if (state == 0x00020001)
            _activeTowerOffsets.Add(offset);
        else if (state == 0x00080004 || state == 0x00100004) // soaked or unsoaked
            _activeTowerOffsets.Remove(offset);
    }
}
