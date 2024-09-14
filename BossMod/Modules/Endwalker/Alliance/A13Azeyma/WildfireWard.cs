namespace BossMod.Endwalker.Alliance.A13Azeyma;

class WildfireWard(BossModule module) : Components.KnockbackFromCastTarget(module, ActionID.MakeSpell(AID.IlluminatingGlimpse), 15, false, 1, kind: Kind.DirLeft);
class ArenaBounds(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly WPos triangleCenter = new(-750, -753.325f);
    private static readonly TriangleE triangle = new(triangleCenter, 24);
    private static readonly AOEShapeCustom triangleCutOut = new([new Circle(A13Azeyma.NormalCenter, 29.5f)], [triangle]);
    private static readonly ArenaBoundsComplex triangleBounds = new([triangle]);

    private AOEInstance? _aoe;

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);

    public override void OnEventEnvControl(byte index, uint state)
    {
        if (index == 0x1C)
        {
            if (state == 0x00020001)
                _aoe = new(triangleCutOut, A13Azeyma.NormalCenter, default, WorldState.FutureTime(5.7f));
            else if (state == 0x00200010)
            {
                _aoe = null;
                Arena.Bounds = triangleBounds;
                Arena.Center = triangleCenter;
            }
            else if (state == 0x00080004)
                Arena.Bounds = A13Azeyma.NormalBounds;
        }
    }
}
