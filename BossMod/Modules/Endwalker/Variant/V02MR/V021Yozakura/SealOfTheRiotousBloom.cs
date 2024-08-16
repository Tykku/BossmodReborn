namespace BossMod.Endwalker.Variant.V02MR.V021Yozakura;

class SealOfRiotousBloom(BossModule module) : Components.GenericAOEs(module)
{
    private enum Element { Fire, Water, Thunder, Wind }
    private readonly List<AOEInstance> _aoes = [];
    private readonly HashSet<Element> elements = [];
    private static readonly AOEShapeCircle circle = new(9);
    private static readonly AOEShapeDonut donut = new(5, 60);
    private static readonly AOEShapeCone cone = new(70, 22.5f.Degrees());
    private static readonly Angle[] anglesIntercardinals = [-45.003f.Degrees(), 44.998f.Degrees(), 134.999f.Degrees(), -135.005f.Degrees()];
    private static readonly Angle[] anglesCardinals = [-90.004f.Degrees(), -0.003f.Degrees(), 180.Degrees(), 89.999f.Degrees()];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes.Take(5);

    public override void OnActorEAnim(Actor actor, uint state)
    {
        if (Enum.TryParse(Enum.GetName(typeof(OID), actor.OID), out Element element))
        {
            switch (state)
            {
                case 0x00100020: // seals spawn
                    elements.Add(element);
                    break;

                case 0x00400080: // seal activates
                    if (elements.Contains(element))
                        ActivateAOE(element, Module.WorldState.FutureTime(8.1f));
                    break;
            }
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (_aoes.Count > 4 && (AID)spell.Action.ID is AID.SealOfTheFireblossom or AID.SealOfTheWindblossom)
            _aoes.RemoveRange(0, 5);
    }

    private void ActivateAOE(Element element, DateTime activation)
    {
        switch (element)
        {
            case Element.Fire:
                _aoes.Add(new(circle, Arena.Center, default, activation));
                break;

            case Element.Thunder:
                AddConeAOEs(anglesCardinals, activation);
                break;

            case Element.Water:
                AddConeAOEs(anglesIntercardinals, activation);
                break;

            case Element.Wind:
                _aoes.Add(new(donut, Arena.Center, default, activation));
                break;
        }
        elements.Remove(element);
        if (_aoes.Count == 5 && elements.Count > 0)
            foreach (var e in elements)
                ActivateAOE(e, Module.WorldState.FutureTime(16.3f));
    }

    private void AddConeAOEs(Angle[] angles, DateTime activationTime)
    {
        foreach (var angle in angles)
            _aoes.Add(new(cone, Arena.Center, angle, activationTime));
    }
}
