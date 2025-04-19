namespace BossMod.Dawntrail.Extreme.Ex4Zelenia;

class RosebloodDrop(BossModule module) : Components.Adds(module, (uint)OID.RosebloodDrop2);

class Towers2(BossModule module) : Components.GenericTowers(module)
{
    private BitMask forbidden;

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        switch (spell.Action.ID)
        {
            case (uint)AID.Explosion2:
                Towers.Add(new(spell.LocXZ, 3f, 3, 3, forbidden, Module.CastFinishAt(spell)));
                break;
            case (uint)AID.SpearpointPush1:
            case (uint)AID.SpearpointPush2:
                forbidden = default;
                break;
        }
    }

    public override void OnEventIcon(Actor actor, uint iconID, ulong targetID)
    {
        if (iconID == (uint)IconID.SpearpointPush)
            forbidden[Raid.FindSlot(targetID)] = true;
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == (uint)AID.Explosion2)
        {
            Towers.Clear();
        }
    }
}

class SpearpointPushAOE(BossModule module) : Components.GenericAOEs(module)
{
    public readonly List<AOEInstance> AOEs = new(2);
    private static readonly AOEShapeRect rect = new(33f, 37f);

    public override ReadOnlySpan<AOEInstance> ActiveAOEs(int slot, Actor actor) => CollectionsMarshal.AsSpan(AOEs);

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.SpearpointPush1 or (uint)AID.SpearpointPush2)
        {
            AOEs.Add(new(rect, spell.LocXZ, spell.Rotation, Module.CastFinishAt(spell)));
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.SpearpointPush1 or (uint)AID.SpearpointPush2)
        {
            AOEs.Clear();
        }
    }
}

class SpearpointPushBait(BossModule module) : Components.GenericBaitAway(module, onlyShowOutlines: true)
{
    private static readonly AOEShapeRect rect = new(32f, 37f, 1f);
    private Angle offset;

    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if (actor.OID == (uint)OID.ZeleniasShade)
        {
            offset = id switch
            {
                0x0C90 => -90f.Degrees(),
                0x0C91 => 90f.Degrees(),
                _ => default
            };
        }
    }

    public override void OnTethered(Actor source, ActorTetherInfo tether)
    {
        if (tether.ID == (uint)TetherID.SpearpointPush)
        {
            var target = WorldState.Actors.Find(tether.Target);
            if (target is Actor t)
                CurrentBaits.Add(new(source, t, rect, WorldState.FutureTime(6.7d)));
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID is (uint)AID.SpearpointPush1 or (uint)AID.SpearpointPush2)
        {
            CurrentBaits.Clear();
        }
    }

    public override void Update()
    {
        var count = CurrentBaits.Count;
        if (count == 0)
            return;
        var baits = CollectionsMarshal.AsSpan(CurrentBaits);
        for (var i = 0; i < count; ++i)
        {
            ref var b = ref baits[i];
            b.CustomRotation = b.Source.Rotation + offset;
        }
    }
}
