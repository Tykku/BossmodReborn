﻿namespace BossMod.Stormblood.Ultimate.UCOB;

class P5Exaflare(BossModule module) : Components.Exaflare(module, 6f)
{
    public void Reset() => NumCasts = 0;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == (uint)AID.ExaflareFirst)
        {
            Lines.Add(new() { Next = caster.Position, Advance = 8f * spell.Rotation.ToDirection(), NextExplosion = Module.CastFinishAt(spell), TimeToMove = 1.5f, ExplosionsLeft = 6, MaxShownExplosions = 4 });
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action.ID is (uint)AID.ExaflareFirst or (uint)AID.ExaflareRest)
        {
            ++NumCasts;
            var count = Lines.Count;
            var pos = caster.Position;
            for (var i = 0; i < count; ++i)
            {
                var line = Lines[i];
                if (line.Next.AlmostEqual(pos, 1f))
                {
                    AdvanceLine(line, pos);
                    if (line.ExplosionsLeft == 0)
                        Lines.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
