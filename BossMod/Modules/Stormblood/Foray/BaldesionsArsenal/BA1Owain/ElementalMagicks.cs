namespace BossMod.Stormblood.Foray.BaldesionArsenal.BA1Owain;

class ElementalMagicks(BossModule module) : Components.GenericAOEs(module)
{
    private static readonly AOEShapeCircle circle = new(13);
    public readonly List<AOEInstance> AOEs = new(5);

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => AOEs;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        void AddAOEs(SID sid)
        {
            var mundberg = Module.Enemies(OID.Munderg);
            var activation = Module.CastFinishAt(spell);
            for (var i = 0; i < mundberg.Count; ++i)
            {
                var spear = mundberg[i];
                if (spear.FindStatus(sid) != null)
                    AOEs.Add(new(circle, spear.Position, default, activation));
            }
            AOEs.Add(new(circle, spell.LocXZ, default, activation));
        }
        switch ((AID)spell.Action.ID)
        {
            case AID.ElementalMagicksFireBoss:
                AddAOEs(SID.SoulOfFire);
                break;
            case AID.ElementalMagicksIceBoss:
                AddAOEs(SID.SoulOfIce);
                break;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (AOEs.Count != 0 && (AID)spell.Action.ID is AID.ElementalMagicksFireBoss or AID.ElementalMagicksFireSpears or AID.ElementalMagicksIceBoss or AID.ElementalMagicksIceSpears)
            AOEs.RemoveAt(0);
    }
}
