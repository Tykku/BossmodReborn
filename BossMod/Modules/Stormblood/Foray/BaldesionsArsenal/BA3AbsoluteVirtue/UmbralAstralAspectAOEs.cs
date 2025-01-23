namespace BossMod.Stormblood.Foray.BaldesionArsenal.BA3AbsoluteVirtue;

class BrightDarkAurora(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect rect = new(30, 50);
    public readonly List<AOEInstance> _aoes = new(3);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        void AddAOE() => _aoes.Add(new(rect, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell), ActorID: caster.InstanceID));
        switch ((AID)spell.Action.ID)
        {
            case AID.DarkAurora1:
            case AID.DarkAurora2:
                if (caster.FindStatus(SID.UmbralEssence) != null)
                    AddAOE();
                break;
            case AID.BrightAurora1:
            case AID.BrightAurora2:

                if (caster.FindStatus(SID.AstralEssence) != null)
                    AddAOE();
                break;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        var count = _aoes.Count;
        if (_aoes.Count != 0 && (AID)spell.Action.ID is AID.BrightAurora1 or AID.BrightAurora2 or AID.DarkAurora1 or AID.DarkAurora2)
            for (var i = 0; i < count; ++i)
            {
                var aoe = _aoes[i];
                if (aoe.ActorID == caster.InstanceID)
                {
                    _aoes.Remove(aoe);
                    break;
                }
            }
    }
}

class BrightDarkAuroraCounter(BossModule module) : Components.CastCounterMulti(module, [ActionID.MakeSpell(AID.DarkAurora1), ActionID.MakeSpell(AID.BrightAurora1),
ActionID.MakeSpell(AID.DarkAurora2), ActionID.MakeSpell(AID.BrightAurora2)]);

class AstralUmbralRays(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCircle circleSmall = new(8), circleBig = new(16);
    public readonly List<AOEInstance> _aoes = new(9);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        void AddAOE(bool big) => _aoes.Add(new(big ? circleBig : circleSmall, spell.LocXZ, default, Module.CastFinishAt(spell)));
        switch ((AID)spell.Action.ID)
        {
            case AID.UmbralRays1:
            case AID.UmbralRays2:
                AddAOE(Module.PrimaryActor.FindStatus(SID.UmbralEssence) != null);
                break;
            case AID.AstralRays1:
            case AID.AstralRays2:
                AddAOE(Module.PrimaryActor.FindStatus(SID.AstralEssence) != null);
                break;
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.UmbralRays1 or AID.UmbralRays2 or AID.AstralRays1 or AID.AstralRays2)
            _aoes.Clear();
    }
}
