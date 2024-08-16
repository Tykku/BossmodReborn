namespace BossMod.Endwalker.Variant.V02MR.V022Moko;

class SpearmanOrdersFast(BossModule module) : Components.Exaflare(module, new AOEShapeRect(3, 2, 0.474f))
{
    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if ((OID)actor.OID == OID.AshigaruSoheiFast && id == 0x25E9)
            Lines.Add(new() { Next = actor.Position, Advance = 3.474f * actor.Rotation.ToDirection(), NextExplosion = Module.WorldState.FutureTime(7.8f), TimeToMove = 0.6f, ExplosionsLeft = 12, MaxShownExplosions = 3 });
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.SpearpointPushFast)
        {
            ++NumCasts;
            var index = Lines.FindIndex(item => item.Next.AlmostEqual(caster.Position, 1));
            if (index >= 0)
            {
                AdvanceLine(Lines[index], caster.Position);
                if (Lines[index].ExplosionsLeft == 0)
                    Lines.RemoveAt(index);
            }
        }
    }
}

class SpearmanOrdersSlow(BossModule module) : Components.Exaflare(module, new AOEShapeRect(2, 2, 0.316f))
{
    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if ((OID)actor.OID == OID.AshigaruSoheiSlow && id == 0x25E9)
            Lines.Add(new() { Next = actor.Position, Advance = 2.316f * actor.Rotation.ToDirection(), NextExplosion = Module.WorldState.FutureTime(7.8f), TimeToMove = 0.6f, ExplosionsLeft = 18, MaxShownExplosions = 3 });
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if ((AID)spell.Action.ID == AID.SpearpointPushSlow)
        {
            ++NumCasts;
            var index = Lines.FindIndex(item => item.Next.AlmostEqual(caster.Position, 1));
            if (index >= 0)
            {
                AdvanceLine(Lines[index], caster.Position);
                if (Lines[index].ExplosionsLeft == 0)
                    Lines.RemoveAt(index);
            }
        }
    }
}
