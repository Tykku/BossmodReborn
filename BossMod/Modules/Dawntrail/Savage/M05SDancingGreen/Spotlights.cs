namespace BossMod.Dawntrail.Savage.M05SDancingGreen;

class Spotlights1(BossModule module) : Components.GenericTowers(module)
{
    protected readonly int[] _orders = new int[PartyState.MaxPartySize];
    private bool? patternENVC200010;
    public int FinishedCount;
    protected bool? spotlightSet; // true set 1, false set 2
    private static readonly WPos[] spotlightSet1 =
    [
        new(87.48f, 112.474f), new(107.5f, 102.495f), new(92.485f, 97.49f), new(112.474f, 87.48f),
        new(102.495f, 92.485f), new(87.48f, 87.48f), new(97.49f, 107.5f), new(112.474f, 112.474f)
    ];

    private static readonly WPos[] spotlightSet2 =
    [
        new(87.48f, 112.474f), new(97.49f, 92.49f), new(112.474f, 87.48f), new(102.495f, 107.5f),
        new(107.5f, 97.49f), new(112.474f, 112.474f), new(87.48f, 87.48f), new(92.485f, 102.495f)
    ];
    protected readonly List<Tower> cachedTowers = new(4);
    private readonly M05SDancingGreenConfig _config = Service.Config.Get<M05SDancingGreenConfig>();

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (_orders[slot] > 0)
            hints.Add($"Spotlight order: {_orders[slot]}", false);
    }

    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if (spotlightSet == null && actor.OID == (uint)OID.Spotlight && id == 0x11DC)
        {
            var position = actor.Position;
            if (position == new WPos(102.5f, 107.5f))
                spotlightSet = true;
            else if (position == new WPos(102.5f, 92.5f))
                spotlightSet = false;
        }
    }

    public override void OnEventEnvControl(byte index, uint state)
    {
        if (index != 0x03u || patternENVC200010 != null)
            return;

        patternENVC200010 ??= state switch
        {
            0x00020001u => false,
            0x00200010u => true,
            _ => null
        };

        if (patternENVC200010 is not bool pattern || spotlightSet is not bool set)
            return;

        var (spotlights, spotlights2) = GetSpotlightSets(pattern, set);

        var (forbiddenFirst, forbiddenSecond) = ForbiddenBitMasks;

        for (var i = 0; i < 4; ++i)
        {
            Towers.Add(new(spotlights[i], 2.5f, forbiddenSoakers: forbiddenFirst, activation: WorldState.FutureTime(22d)));
            cachedTowers.Add(new(spotlights2[i], 2.5f, forbiddenSoakers: forbiddenSecond, activation: WorldState.FutureTime(33.6d)));
        }
        if (_config.ShowAllSpotlights)
        {
            Towers.AddRange(cachedTowers);
            cachedTowers.Clear();
            Towers.SortByReverse(t => t.Activation); // prevent allowed towers overlapping forbidden
        }
    }

    private static (WPos[], WPos[]) GetSpotlightSets(bool pattern, bool set) => (GetSpotlightSet(pattern, set, true), GetSpotlightSet(pattern, set, false));

    private static WPos[] GetSpotlightSet(bool pattern, bool set, bool firstSet)
    {
        var source = firstSet
            ? (set ? spotlightSet1 : spotlightSet2)
            : (set ? spotlightSet2 : spotlightSet1);

        var spotlights = new WPos[4];
        Array.Copy(source, pattern ? 0 : 4, spotlights, 0, 4);
        return spotlights;
    }

    protected (BitMask, BitMask) ForbiddenBitMasks
    {
        get
        {
            BitMask forbiddenFirst = new();
            BitMask forbiddenSecond = new();

            for (var i = 0; i < 8; ++i)
            {
                if (_orders[i] == 2)
                    forbiddenFirst[i] = true;
                else
                    forbiddenSecond[i] = true;
            }

            return (forbiddenFirst, forbiddenSecond);
        }
    }

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if (status.ID == (uint)SID.BurnBabyBurn && Raid.FindSlot(actor.InstanceID) is var slot && slot >= 0)
            _orders[slot] = (status.ExpireAt - WorldState.CurrentTime).TotalSeconds < 30d ? 1 : 2;
    }

    public override void OnStatusLose(Actor actor, ActorStatus status)
    {
        if (status.ID == (uint)SID.BurnBabyBurn && Raid.FindSlot(actor.InstanceID) is var slot && slot >= 0)
        {
            _orders[slot] = 0;
            if (++FinishedCount >= 4)
            {
                if (Towers.Count == 8)
                    Towers.RemoveRange(4, 4);
                else
                    Towers.Clear();
                if (FinishedCount == 4 && cachedTowers.Count != 0)
                    Towers = cachedTowers;
            }
        }
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        var count = Towers.Count;
        if (count == 0)
            return;
        var towers = CollectionsMarshal.AsSpan(Towers);
        for (var i = 0; i < count; ++i)
        {
            ref var t = ref towers[i];
            var isInside = t.IsInside(pc);
            var numInside = t.NumInside(Module);
            var safe = !t.ForbiddenSoakers[pcSlot] && (numInside < t.MaxSoakers || isInside && numInside <= t.MaxSoakers);
            t.Shape.Outline(Arena, t.Position, t.Rotation, safe ? Colors.Safe : default, 2f);
        }
    }

    public override void DrawArenaBackground(int pcSlot, Actor pc) { }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (Towers.Count == 0)
            return;
        if (!Towers[0].ForbiddenSoakers[slot])
            base.AddAIHints(slot, actor, assignment, hints);
    }
}

class Spotlights2(BossModule module) : Spotlights1(module)
{
    private static readonly WPos[] spotlightSet1 = [new(84.977f, 84.977f), new(114.977f, 84.977f), new(84.977f, 114.977f), new(114.977f, 114.977f)];
    private static readonly WPos[] spotlightSet2 = [new(84.977f, 99.992f), new(99.992f, 84.977f), new(99.992f, 114.977f), new(114.977f, 99.992f)];

    public override void OnActorPlayActionTimelineEvent(Actor actor, ushort id)
    {
        if (spotlightSet == null && actor.OID == (uint)OID.Spotlight && id == 0x11DC)
        {
            if (actor.Position == new WPos(115f, 115f))
                spotlightSet = true;
            else if (actor.Position == new WPos(115f, 100f))
                spotlightSet = false;
            if (spotlightSet is bool set)
            {
                var spotlights1 = set ? spotlightSet2 : spotlightSet1;
                var spotlights2 = set ? spotlightSet1 : spotlightSet2;

                var (forbiddenFirst, forbiddenSecond) = ForbiddenBitMasks;

                for (var i = 0; i < 4; ++i)
                {
                    Towers.Add(new(spotlights1[i], 2.5f, forbiddenSoakers: forbiddenFirst, activation: WorldState.FutureTime(9.5d)));
                    cachedTowers.Add(new(spotlights2[i], 2.5f, forbiddenSoakers: forbiddenSecond, activation: WorldState.FutureTime(19.5d)));
                }
            }
        }
    }

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if (status.ID == (uint)SID.BurnBabyBurn && Raid.FindSlot(actor.InstanceID) is var slot && slot >= 0)
            _orders[slot] = (status.ExpireAt - WorldState.CurrentTime).TotalSeconds < 15d ? 1 : 2;
    }
}
