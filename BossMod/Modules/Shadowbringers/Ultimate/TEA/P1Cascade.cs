namespace BossMod.Shadowbringers.Ultimate.TEA;

class P1Cascade(BossModule module) : Components.Voidzone(module, 8, m => m.Enemies(OID.LiquidRage))
{
    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        Arena.Actors(Module.Enemies((uint)OID.Embolus), Colors.Object, true);
    }
}
