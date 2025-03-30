namespace BossMod.Dawntrail.Trial.T04Zelenia;

abstract class AlexandrianThunderIIICircle(BossModule module, AID aid) : Components.SimpleAOEs(module, ActionID.MakeSpell(aid), 4f);
class AlexandrianThunderIIICircle1(BossModule module) : AlexandrianThunderIIICircle(module, AID.AlexandrianThunderIII1);
class AlexandrianThunderIIICircle2(BossModule module) : AlexandrianThunderIIICircle(module, AID.AlexandrianThunderIII2);

class AlexandrianThunderIIICones(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCone coneSmall = new(16f, 30f.Degrees()), coneBig = new(16f, 90f.Degrees());
    private readonly List<AOEInstance> _aoes = new(4);
    private enum Pattern { None, A, B, C }
    private Pattern CurrentPattern;

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
            case 0x18u:
                CurrentPattern = Pattern.C;
                break;
        }
    }
    // arena slices 10000040:
    // 0x04: 180°
    // 0x05: 120°
    // 0x06: 60°
    // 0x07: 0°
    // 0x08: -60°
    // 0x08: -120°
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
                _aoes.Add(new(coneBig, WPos.ClampToGrid(Arena.Center), 180f.Degrees(), Module.CastFinishAt(spell)));
                rotation = new Angle();
            }
            if (rotation is Angle rot)
                _aoes.Add(new(coneSmall, WPos.ClampToGrid(Arena.Center), rot, Module.CastFinishAt(spell)));
        }
    }

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
                    else if (CurrentPattern == Pattern.C && NumCasts == 3)
                        _aoes.Clear();
                    if (_aoes.Count == 0)
                        NumCasts = 0;
                    break;
            }
    }
}
