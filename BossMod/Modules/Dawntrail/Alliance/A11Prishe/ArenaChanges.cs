namespace BossMod.Dawntrail.Alliance.A11Prishe;

class ArenaChanges(BossModule module) : Components.GenericAOEs(module, ActionID.MakeSpell(AID.Thornbite))
{
    public bool Active => _aoe != null || Arena.Bounds != A11Prishe.DefaultBounds;
    private AOEInstance? _aoe;
    private static readonly Square[] defaultSquare = [new(A11Prishe.ArenaCenter, 35)];
    private static readonly Shape[] differenceENVC00020001 = [new Square(new(795, 405), 10), new Square(new(805, 395), 10), new Rectangle(new(810, 430), 15, 5),
    new Rectangle(new(830, 420), 5, 15), new Rectangle(new(790, 370), 15, 5), new Rectangle(new(770, 380), 5, 15)];
    private static readonly AOEShapeCustom arenaChangeENVC00020001 = new(defaultSquare, differenceENVC00020001);
    private static readonly ArenaBoundsComplex arenaENVC00020001 = new(differenceENVC00020001);
    private static readonly Shape[] differenceENVC02000100 = [new Square(new(795, 395), 10), new Square(new(805, 405), 10), new Rectangle(new(820, 370), 15, 5),
    new Rectangle(new(830, 390), 5, 15), new Rectangle(new(780, 430), 15, 5), new Rectangle(new(770, 410), 5, 15)];
    private static readonly AOEShapeCustom arenaChangeENVC02000100 = new(defaultSquare, differenceENVC02000100);
    private static readonly ArenaBoundsComplex arenaENVC02000100 = new(differenceENVC02000100);
    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => Utils.ZeroOrOne(_aoe);

    public override void OnEventEnvControl(byte index, uint state)
    {
        if (index != 0x01)
            return;
        switch (state)
        {
            case 0x00020001:
                SetAOE(arenaChangeENVC00020001);
                break;
            case 0x02000100:
                SetAOE(arenaChangeENVC02000100);
                break;
            case 0x00200010:
                SetArena(arenaENVC00020001);
                break;
            case 0x08000400:
                SetArena(arenaENVC02000100);
                break;
            case 0x00080004 or 0x00800004:
                SetDefaultArena();
                break;
        }
    }

    private void SetDefaultArena()
    {
        Arena.Bounds = A11Prishe.DefaultBounds;
        Arena.Center = A11Prishe.ArenaCenter;
    }

    private void SetArena(ArenaBoundsComplex bounds)
    {
        Arena.Bounds = bounds;
        Arena.Center = bounds.Center;
        _aoe = null;
    }

    private void SetAOE(AOEShapeCustom shape)
    {
        _aoe = new(shape, A11Prishe.ArenaCenter, default, WorldState.FutureTime(5));
    }
}
