namespace BossMod.Dawntrail.Extreme.Ex3QueenEternal;

class RadicalShift(BossModule module) : Components.GenericAOEs(module)
{
    public enum Rotation { None, Left, Right }

    private ArenaBoundsComplex? _left;
    private ArenaBoundsComplex? _right;
    private Rotation _nextRotation;
    private AOEInstance? _aoe;
    private static readonly Square[] defaultSquare = [new(Ex3QueenEternal.ArenaCenter, 20)];
    private static readonly AOEShapeCustom windArena = new(defaultSquare, Trial.T03QueenEternal.T03QueenEternal.XArenaRects);
    private static readonly AOEShapeCustom earthArena = new(defaultSquare, Trial.T03QueenEternal.T03QueenEternal.SplitArenaRects);
    private static readonly AOEShapeCustom iceArena = new(defaultSquare, Ex3QueenEternal.IceRectsAll);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);

    public override void OnEventEnvControl(byte index, uint state)
    {
        if (index == 12)
        {
            var rot = state switch
            {
                0x01000080 => Rotation.Left,
                0x08000400 => Rotation.Right,
                _ => Rotation.None
            };
            if (rot != Rotation.None)
            {
                _nextRotation = rot;
                UpdateAOE(NextPlatform);
            }
        }
        else if (state is 0x00020001 or 0x00200010)
        {
            var platform = index switch
            {
                9 => Ex3QueenEternal.WindBounds,
                10 => Ex3QueenEternal.EarthBounds,
                11 => Ex3QueenEternal.IceBridgeBounds,
                _ => null
            };
            if (platform != null)
            {
                (state == 0x00020001 ? ref _right : ref _left) = platform;
                UpdateAOE(NextPlatform);
            }
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.RadicalShift)
        {
            var platform = NextPlatform;
            if (platform != null)
            {
                Arena.Bounds = platform;
                Arena.Center = platform.Center;
            }
            _left = _right = null;
            _nextRotation = Rotation.None;
            _aoe = null;
        }
    }

    private ArenaBoundsComplex? NextPlatform => _nextRotation switch
    {
        Rotation.Left => _left,
        Rotation.Right => _right,
        _ => null
    };

    private void UpdateAOE(ArenaBoundsComplex? platform)
    {
        var activation = WorldState.FutureTime(6);
        var center = Arena.Bounds == Ex3QueenEternal.NormalBounds ? Ex3QueenEternal.ArenaCenter
        : Arena.Bounds == Ex3QueenEternal.IceBridgeBounds ? Ex3QueenEternal.IceBridgeBounds.Center
        : Arena.Bounds == Ex3QueenEternal.EarthBounds ? Ex3QueenEternal.EarthBounds.Center
        : Arena.Bounds == Ex3QueenEternal.WindBounds ? Ex3QueenEternal.WindBounds.Center : Arena.Center;
        if (platform == Ex3QueenEternal.WindBounds)
            _aoe = new(windArena, center, default, activation);
        else if (platform == Ex3QueenEternal.EarthBounds)
            _aoe = new(earthArena, center, default, activation);
        else if (platform == Ex3QueenEternal.IceBridgeBounds)
            _aoe = new(iceArena, center, default, activation);
    }
}

class RadicalShiftAOE(BossModule module) : Components.SpreadFromCastTargets(module, ActionID.MakeSpell(AID.RadicalShiftAOE), 5);
