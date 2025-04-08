namespace BossMod.Dawntrail.Raid.M08NHowlingBlade;

class WolvesReignRect1(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.WolvesReignRect1), new AOEShapeRect(36f, 5f));
class WolvesReignRect2(BossModule module) : Components.SimpleAOEs(module, ActionID.MakeSpell(AID.WolvesReignRect2), new AOEShapeRect(28f, 5f));

class WolvesReignCone(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCone cone = new(40f, 60f.Degrees());
    private AOEInstance? _aoe;

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(ref _aoe);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.WolvesReignTeleport2 or (uint)AID.WolvesReignTeleport4)
            _aoe = new(cone, spell.LocXZ, caster.Rotation + 180f.Degrees(), Module.CastFinishAt(spell, 5.1f));
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.WolvesReignCone1 or (uint)AID.WolvesReignCone2)
            _aoe = null;
    }
}

class WolvesReignCircle(BossModule module) : Components.GenericAOEs(module)
{
    private readonly List<AOEInstance> _aoes = new(4);
    private static readonly AOEShapeCircle circle = new(6f);

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => CollectionsMarshal.AsSpan(_aoes);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch (spell.Action.ID)
        {
            case (uint)AID.WolvesReignCircle1:
            case (uint)AID.WolvesReignCircle2:
            case (uint)AID.WolvesReignCircle3:
            case (uint)AID.WolvesReignCircle4:
            case (uint)AID.WolvesReignCircle5:
            case (uint)AID.WolvesReignCircle6:
            case (uint)AID.WolvesReignCircle7:
            case (uint)AID.WolvesReignCircle8:
                _aoes.Add(new(circle, spell.LocXZ, default, Module.CastFinishAt(spell), ActorID: caster.InstanceID));
                break;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        var count = _aoes.Count;
        if (count != 0)
            switch (spell.Action.ID)
            {
                case (uint)AID.WolvesReignCircle1:
                case (uint)AID.WolvesReignCircle2:
                case (uint)AID.WolvesReignCircle3:
                case (uint)AID.WolvesReignCircle4:
                case (uint)AID.WolvesReignCircle5:
                case (uint)AID.WolvesReignCircle6:
                case (uint)AID.WolvesReignCircle7:
                case (uint)AID.WolvesReignCircle8:
                    for (var i = 0; i < count; ++i)
                    {
                        if (_aoes[i].ActorID == caster.InstanceID)
                        {
                            _aoes.RemoveAt(i);
                            break;
                        }
                    }
                    break;
            }
    }
}
