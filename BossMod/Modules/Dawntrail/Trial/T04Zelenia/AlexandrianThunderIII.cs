namespace BossMod.Dawntrail.Trial.T04Zelenia;

abstract class AlexandrianThunderIIICircle(BossModule module, AID aid) : Components.SimpleAOEs(module, ActionID.MakeSpell(aid), 4f);
class AlexandrianThunderIIICircle1(BossModule module) : AlexandrianThunderIIICircle(module, AID.AlexandrianThunderIII1);
class AlexandrianThunderIIICircle2(BossModule module) : AlexandrianThunderIIICircle(module, AID.AlexandrianThunderIII2);

class AlexandrianThunderIIICones(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCone cone = new(16f, 30f.Degrees()), coneBig = new(16f, 90f.Degrees());
    private readonly List<AOEInstance> _aoes = new(4);
    private enum Pattern { None, A, B, C, D1, D2, E, F, G, H }
    private Pattern CurrentPattern;
    private AOEInstance? cachedAOE;
    // TODO: rework all of this, its unreliable since no information boss mod replays contain accurately reflect the pattern changes correctly
    // both ENVCs and DIRUs are reused between different patterns
    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => CollectionsMarshal.AsSpan(_aoes);

    public override void OnEventEnvControl(byte index, uint state)
    {
        if (index == 0x01 && state == 0x00080004u)
            CurrentPattern = Pattern.None;
    }

    public override void OnEventDirectorUpdate(uint updateID, uint param1, uint param2, uint param3, uint param4)
    {
        if (updateID != 0x80000027u)
            return;
        switch (param1)
        {
            case 0x19u:
                CurrentPattern = Pattern.A;
                break;
            case 0x16u:
                CurrentPattern = Pattern.B;
                break;
            case 0x17u:
                CurrentPattern = Pattern.C;
                break;
            case 0x24u:
                CurrentPattern = Pattern.D1;
                break;
            case 0x25u:
                CurrentPattern = Pattern.E;
                break;
            case 0x18u:
                if (CurrentPattern != Pattern.C)
                    CurrentPattern = Pattern.F;
                break;
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.AlexandrianThunderIII1 or (uint)AID.AlexandrianThunderIII2)
        {
            Angle? rotation = null;
            var pos = caster.Position;
            var spellPosition = (int)(pos.X + pos.Z);
            if (CurrentPattern == Pattern.A)
            {
                switch (spellPosition)
                {
                    case 183: // 94, 89.608
                        rotation = 180f.Degrees();
                        break;
                    case 216: // 106, 110.392
                        rotation = new Angle();
                        break;
                }
            }
            else if (CurrentPattern == Pattern.B)
            {
                switch (spellPosition)
                {
                    case 183: // 94, 89.608
                        rotation = 180f.Degrees();
                        break;
                    case 216: // 106, 110.392
                        rotation = new Angle();
                        break;
                    case 195: // 89.608, 106
                        rotation = -60f.Degrees();
                        break;
                    case 204: // 110.392, 94
                        rotation = 120f.Degrees();
                        break;
                }
            }
            else if (CurrentPattern == Pattern.C && _aoes.Count == 0)
            {
                _aoes.Add(new(cone, WPos.ClampToGrid(Arena.Center), 120f.Degrees(), Module.CastFinishAt(spell)));
                _aoes.Add(new(cone, WPos.ClampToGrid(Arena.Center), -120f.Degrees(), Module.CastFinishAt(spell)));
                rotation = new Angle();
                if (cachedAOE is AOEInstance aoe)
                {
                    aoe.Activation = Module.CastFinishAt(spell);
                    _aoes.Add(aoe);
                }
            }
            else if (CurrentPattern == Pattern.G && _aoes.Count == 0)
            {
                _aoes.Add(new(cone, WPos.ClampToGrid(Arena.Center), -60f.Degrees(), Module.CastFinishAt(spell)));
                _aoes.Add(new(cone, WPos.ClampToGrid(Arena.Center), 120f.Degrees(), Module.CastFinishAt(spell)));
                if (cachedAOE is AOEInstance aoe)
                {
                    aoe.Activation = Module.CastFinishAt(spell);
                    _aoes.Add(aoe);
                }
            }
            else if (CurrentPattern == Pattern.E && _aoes.Count == 0)
            {
                _aoes.Add(new(cone, WPos.ClampToGrid(Arena.Center), 60f.Degrees(), Module.CastFinishAt(spell)));
                _aoes.Add(new(cone, WPos.ClampToGrid(Arena.Center), -120f.Degrees(), Module.CastFinishAt(spell)));
                if (cachedAOE is AOEInstance aoe)
                {
                    aoe.Activation = Module.CastFinishAt(spell);
                    _aoes.Add(aoe);
                }
            }
            else if (CurrentPattern == Pattern.D1 && _aoes.Count == 0)
            {
                if (cachedAOE is AOEInstance aoe)
                {
                    aoe.Shape = coneBig;
                    _aoes.Add(aoe);
                }
            }
            else if (CurrentPattern == Pattern.D2 && _aoes.Count == 0)
            {
                if (cachedAOE is AOEInstance aoe1)
                {
                    aoe1.Rotation += 180f.Degrees();
                    _aoes.Add(aoe1);
                }
                if (cachedAOE is AOEInstance aoe2)
                {
                    aoe2.Shape = coneBig;
                    _aoes.Add(aoe2);
                }
            }
            if (rotation is Angle rot)
                _aoes.Add(new(cone, WPos.ClampToGrid(Arena.Center), rot, Module.CastFinishAt(spell)));
        }
        else if (spell.Action.ID is (uint)AID.AlexandrianThunderIVCircle2 or (uint)AID.AlexandrianThunderIVDonut2 && _aoes.Count == 0)
        {
            if (CurrentPattern == Pattern.C)
            {
                _aoes.Add(new(cone, WPos.ClampToGrid(Arena.Center), 120f.Degrees(), Module.CastFinishAt(spell)));
                _aoes.Add(new(cone, WPos.ClampToGrid(Arena.Center), -120f.Degrees(), Module.CastFinishAt(spell)));
                _aoes.Add(new(cone, WPos.ClampToGrid(Arena.Center), default, Module.CastFinishAt(spell)));
                CurrentPattern = Pattern.D1;
            }
            else if (CurrentPattern == Pattern.F)
            {
                _aoes.Add(new(cone, WPos.ClampToGrid(Arena.Center), 60f.Degrees(), Module.CastFinishAt(spell)));
                _aoes.Add(new(cone, WPos.ClampToGrid(Arena.Center), -60f.Degrees(), Module.CastFinishAt(spell)));
                _aoes.Add(new(cone, WPos.ClampToGrid(Arena.Center), 180f.Degrees(), Module.CastFinishAt(spell)));
                CurrentPattern = Pattern.D2;
            }
            if (cachedAOE is AOEInstance aoe)
            {
                aoe.Activation = Module.CastFinishAt(spell);
                _aoes.Add(aoe);
            }
        }
    }

    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if (id == 0x1E46)
        {
            var rotrounded = (int)actor.Rotation.Deg;
            Angle? rotation = null;
            switch (rotrounded)
            {
                case -96: // 117.321, 110
                    rotation = -60f.Degrees();
                    CurrentPattern = Pattern.C;
                    break;
                case 96: // 82.679, 110
                    rotation = 60f.Degrees();
                    CurrentPattern = Pattern.C;
                    break;
                case -23: // 100, 80
                    rotation = -60f.Degrees();
                    break;
                case 23: // 100, 80
                    rotation = 60f.Degrees();
                    CurrentPattern = Pattern.G;
                    break;
                case 83: // 82.679, 90
                    rotation = 120f.Degrees();
                    CurrentPattern = Pattern.E;
                    break;
                case 143: // 82.679, 110
                    CurrentPattern = Pattern.C;
                    rotation = 180f.Degrees();
                    break;
                case -143: // 117.321, 110
                    rotation = 180f.Degrees();
                    break;
                case -120: // 117.321, 110
                    rotation = -120f.Degrees();
                    CurrentPattern = Pattern.F;
                    break;
                case 0: // 100, 80
                case -36: // 117.321, 90
                    rotation = new Angle();
                    break;
            }
            if (rotation is Angle rot)
                cachedAOE = new(cone, WPos.ClampToGrid(Arena.Center), rot);
        }
    }
    // arena slices 10000040:
    // 0x04: 180°
    // 0x05: 120°
    // 0x06: 60°
    // 0x07: 0°
    // 0x08: -60°
    // 0x08: -120°
    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (_aoes.Count != 0)
            switch (spell.Action.ID)
            {
                case (uint)AID.AlexandrianThunderIII1:
                case (uint)AID.AlexandrianThunderIII2:
                    ++NumCasts;
                    if (CurrentPattern == Pattern.A && NumCasts is 1 or 3)
                        _aoes.RemoveAt(0);
                    else if (CurrentPattern == Pattern.B && NumCasts is 2 or 4)
                        _aoes.RemoveRange(0, 2);
                    else if (CurrentPattern == Pattern.C && NumCasts == 3 || CurrentPattern is Pattern.D1 or Pattern.D2 or Pattern.E or Pattern.G)
                        _aoes.Clear();
                    if (_aoes.Count == 0)
                        NumCasts = 0;
                    break;
                case (uint)AID.AlexandrianThunderIVCircle2:
                case (uint)AID.AlexandrianThunderIVDonut2:
                    if (++NumCasts == 2)
                    {
                        _aoes.Clear();
                        NumCasts = 0;
                    }
                    break;
            }
    }
}
