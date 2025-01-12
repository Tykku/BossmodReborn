namespace BossMod.Stormblood.Foray.BaldesionArsenal.BA1Art;

class LegendMythSpinnerCarver(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCircle circle = new(15);
    private static readonly AOEShapeDonut donut = new(7, 22);
    public readonly List<AOEInstance> AOEs = new(5);
    private bool mythcall;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = AOEs.Count;
        if (count == 0)
            yield break;

        for (var i = 0; i < count; ++i)
        {
            var aoe = AOEs[i];
            if (count == 5 && i == 0)
                yield return aoe with { Color = Colors.Danger };
            else
                yield return aoe;
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        void AddAOE(AOEShape shape) => AOEs.Add(new(shape, caster.Position, default, Module.CastFinishAt(spell)));
        void AddAOEs(AOEShape shape)
        {
            var orlasrach = Module.Enemies(OID.Orlasrach);
            for (var i = 0; i < orlasrach.Count; ++i)
                AOEs.Add(new(shape, orlasrach[i].Position, default, Module.CastFinishAt(spell, 2.6f)));
            mythcall = false;
        }
        switch ((AID)spell.Action.ID)
        {
            case AID.Legendcarver:
                AddAOE(circle);
                if (mythcall)
                    AddAOEs(circle);
                break;
            case AID.Legendspinner:
                AddAOE(donut);
                if (mythcall)
                    AddAOEs(donut);
                break;
            case AID.Mythcall:
                mythcall = true;
                break;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (AOEs.Count != 0 && (AID)spell.Action.ID is AID.Legendcarver or AID.Legendspinner or AID.Mythcarver or AID.Mythspinner)
            AOEs.RemoveAt(0);
    }
}
