namespace BossMod.Dawntrail.Savage.M07SBruteAbombinator;

class QuarrySwamp(BossModule module) : Components.CastLineOfSightAOE(module, (uint)AID.QuarrySwamp, 60f)
{
    public override ReadOnlySpan<Actor> BlockerActors() => CollectionsMarshal.AsSpan(Module.Enemies((uint)OID.BloomingAbomination));

    public override void Update()
    {
        if (Casters.Count != 0 && BlockerActors().Length != 0)
        {
            Safezones.Clear();
            Refresh();
            AddSafezone(Module.CastFinishAt(Casters[0].CastInfo));
        }
    }
}

class SporeSac(BossModule module) : Components.SimpleAOEs(module, (uint)AID.SporeSac, 8f);
class Pollen(BossModule module) : Components.SimpleAOEs(module, (uint)AID.Pollen, 8f);
class RootsOfEvil(BossModule module) : Components.SimpleAOEs(module, (uint)AID.RootsOfEvil, 12f);
class CrossingCrosswinds(BossModule module) : Components.SimpleAOEs(module, (uint)AID.CrossingCrosswinds, new AOEShapeCross(50f, 5f));
class CrossingCrosswindsHint(BossModule module) : Components.CastInterruptHint(module, (uint)AID.CrossingCrosswinds, showNameInHint: true);
class WindingWildwinds(BossModule module) : Components.SimpleAOEs(module, (uint)AID.WindingWildwinds, new AOEShapeDonut(5f, 60f));
class WindingWildwindsHint(BossModule module) : Components.CastInterruptHint(module, (uint)AID.WindingWildwinds, showNameInHint: true);
