namespace BossMod.Dawntrail.Raid.M05NDancingGreen;

class FunkyFloor(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeRect square = new(2.5f, 2.5f, 2.5f);
    public readonly List<AOEInstance> AOEs = new(32);
    private static readonly WPos[] ENVC20001 = GenerateCheckerboard(0); // 03.20001 top left active
    private static readonly WPos[] ENVC200010 = GenerateCheckerboard(1); // 03.200010 top left inactive

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => CollectionsMarshal.AsSpan(AOEs);

    private static WPos[] GenerateCheckerboard(int offset)
    {
        var centers = new WPos[32];
        var index = 0;
        for (var i = 0; i < 8; ++i)
        {
            var z = i * 5;
            var start = (i + offset) % 2;
            for (var j = start; j < 8; j += 2)
            {
                centers[index++] = new(82.5f + j * 5, 82.5f + z);
            }
        }
        return centers;
    }

    public override void OnEventEnvControl(byte index, uint state)
    {
        if (index == 0x03)
        {
            var set = -1;
            if (state == 0x00020001u)
                set = 1;
            else if (state == 0x00200010u)
                set = 2;
            if (set != -1)
            {
                var tiles = set == 1 ? ENVC20001 : ENVC200010;
                for (var i = 0; i < 32; ++i)
                    AOEs.Add(new(square, tiles[i], default, WorldState.FutureTime(2d)));
            }
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action.ID == (uint)AID.FunkyFloor)
            AOEs.Clear();
    }
}
