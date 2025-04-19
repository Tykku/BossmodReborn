namespace BossMod.Dawntrail.Chaotic.Ch01CloudOfDarkness;

class Phaser(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Phaser, new AOEShapeCone(23f, 30f.Degrees())) // TODO: verify angle
{
    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        var count = Casters.Count;
        if (count == 0)
            return [];
        var aoes = CollectionsMarshal.AsSpan(Casters);
        var deadline = aoes[0].Activation.AddSeconds(1d);

        var index = 0;
        while (index < count && aoes[index].Activation < deadline)
            ++index;

        return aoes[..index];
    }
}
