﻿namespace BossMod.Endwalker.Extreme.Ex5Rubicante;

// mechanic implementation:
// 1. all three circles gain a status that determines their pattern; this pattern is relative to actor's direction
//    inner can be one of three patterns (either single straight line in actor's main direction, or two straight lines - one in actor's direction, another at -90/180 degrees)
//    middle is always one pattern (four straight lines at direction, +-45 and 180, then two curved lines starting at +-135 and leading to +-90)
//    outer is always one pattern (all 8 straight lines)
// 2. 8 actors (triangle/square) get PATE 11D1; they are all in center, but face their visual position
// 3. we get envcontrols for rotations (at least one for each mechanic instance; from this point we can start showing aoes)
// 4. at the same time, all players get 12s penance debuff, which is a deadline to resolve
// 5. right after that, boss starts casting visual cast - at this point we start showing the mechanic
// 5. penance expires and is replaced with 9s shackles debuff, this happens right before cast end
class OrdealOfPurgation(BossModule module) : Components.GenericAOEs(module)
{
    public enum Symbol { Unknown, Tri, Sq }

    private int _dirInner;
    private int _dirInnerExtra; // == dirInner if there is only 1 fireball
    private int _midIncrement; // inner with this index is incremented by one (rotated CCW) when passing middle ring
    private int _midDecrement; // inner with this index is decremented by one (rotated CW) when passing middle ring
    private int _rotationOuter;
    private readonly Symbol[] _symbols = new Symbol[8];
    private DateTime _activation;

    private static readonly AOEShapeCone _shapeTri = new(60f, 30f.Degrees());
    private static readonly AOEShapeRect _shapeSq = new(20f, 40f);

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (_activation != default)
        {
            var aoes = new List<AOEInstance>(2);
            if (AOEFromDirection(_dirInner) is var aoe1 && aoe1 != null)
                aoes.Add(aoe1.Value);
            if (_dirInnerExtra != _dirInner && AOEFromDirection(_dirInnerExtra) is var aoe2 && aoe2 != null)
                aoes.Add(aoe2.Value);
            return CollectionsMarshal.AsSpan(aoes);
        }
        return [];
    }

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if (status.ID == (uint)SID.Pattern)
        {
            switch (actor.OID)
            {
                case (uint)OID.CircleOfPurgatoryInner:
                    _dirInner = AngleToDirectionIndex(actor.Rotation);
                    _dirInnerExtra = status.Extra switch
                    {
                        0x21F => AngleToDirectionIndex(actor.Rotation - 90f.Degrees()),
                        0x220 => AngleToDirectionIndex(actor.Rotation + 180f.Degrees()),
                        _ => _dirInner
                    };
                    break;
                case (uint)OID.CircleOfPurgatoryMiddle:
                    _midIncrement = AngleToDirectionIndex(actor.Rotation - 135f.Degrees());
                    _midDecrement = AngleToDirectionIndex(actor.Rotation + 135f.Degrees());
                    break;
            }
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == (uint)AID.OrdealOfPurgation)
            _activation = Module.CastFinishAt(spell); // note: actual activation is several seconds later, but we need to finish our movements before shackles, so effective activation is around cast end
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action.ID is (uint)AID.FieryExpiationTri or (uint)AID.FieryExpiationSq)
            ++NumCasts;
    }

    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        var symbol = actor.OID switch
        {
            (uint)OID.CircleOfPurgatoryTriange => Symbol.Tri,
            (uint)OID.CircleOfPurgatorySquare => Symbol.Sq,
            _ => Symbol.Unknown
        };
        if (symbol != Symbol.Unknown && id == 0x11D1)
        {
            var dir = AngleToDirectionIndex(actor.Rotation);
            if (_symbols[dir] != Symbol.Unknown)
                ReportError($"Duplicate symbols at {dir}");
            _symbols[dir] = symbol;
        }
    }

    public override void OnEventEnvControl(byte index, uint state)
    {
        var dir = state switch
        {
            0x00020001 => -1,
            0x00200010 => +1,
            _ => 0 // 0x00080004 = remove rotation markers
        };
        if (dir != 0)
        {
            switch (index)
            {
                case 1:
                    _dirInner = NormalizeDirectionIndex(_dirInner + dir);
                    _dirInnerExtra = NormalizeDirectionIndex(_dirInnerExtra + dir);
                    break;
                case 2:
                    _midIncrement = NormalizeDirectionIndex(_midIncrement + dir);
                    _midDecrement = NormalizeDirectionIndex(_midDecrement + dir);
                    break;
                case 3:
                    _rotationOuter = dir;
                    break;
            }
        }
    }

    // 0 is N, then increases in CCW order
    private static int NormalizeDirectionIndex(int index) => index & 7;
    private static int AngleToDirectionIndex(Angle rotation) => NormalizeDirectionIndex((int)(Math.Round(rotation.Deg / 45f) + 4));
    private static Angle DirectionIndexToAngle(int index) => (index - 4) * 45f.Degrees();

    private int TransformByMiddle(int index)
    {
        if (index == _midIncrement)
            return NormalizeDirectionIndex(index + 1);
        else if (index == _midDecrement)
            return NormalizeDirectionIndex(index - 1);
        else
            return index;
    }

    private AOEShape? ShapeAtDirection(int index) => _symbols[NormalizeDirectionIndex(index - _rotationOuter)] switch
    {
        Symbol.Tri => _shapeTri,
        Symbol.Sq => _shapeSq,
        _ => null
    };

    private AOEInstance? AOEFromDirection(int index)
    {
        index = TransformByMiddle(index);
        var shape = ShapeAtDirection(index);
        var dir = DirectionIndexToAngle(index);
        return shape != null ? new(shape, Arena.Center + Arena.Bounds.Radius * dir.ToDirection(), dir + 180f.Degrees(), _activation) : null;
    }
}
