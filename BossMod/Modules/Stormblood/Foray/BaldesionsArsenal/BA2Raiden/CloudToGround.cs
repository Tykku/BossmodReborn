namespace BossMod.Stormblood.Foray.BaldesionArsenal.BA2Raiden;

class CloudToGround(BossModule module) : Components.Exaflare(module, 6)
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.CloudToGroundFirst)
        {
            var explosions = spell.LocXZ.InRect(Arena.Center, spell.Rotation, 35, 35, 15) ? 8 : spell.LocXZ.InRect(Arena.Center, spell.Rotation, 35, 35, 20) ? 6 : 4;
            Lines.Add(new() { Next = spell.LocXZ, Advance = 8 * spell.Rotation.ToDirection(), NextExplosion = Module.CastFinishAt(spell), TimeToMove = 1.1f, ExplosionsLeft = explosions, MaxShownExplosions = 10 });
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID is AID.CloudToGroundFirst or AID.CloudToGroundRest)
        {
            var index = Lines.FindIndex(item => item.Next.AlmostEqual(caster.Position, 1));
            if (index == -1)
                return;
            AdvanceLine(Lines[index], caster.Position);
            if (Lines[index].ExplosionsLeft == 0)
                Lines.RemoveAt(index);
        }
    }
}
