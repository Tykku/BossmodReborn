namespace BossMod.Dawntrail.Savage.M05SDancingGreen;

class WavelengthAlphaBeta(BossModule module) : BossComponent(module)
{
    private readonly (int Order, Actor Actor, DateTime Expiration)[] expirationBySlot = new (int, Actor, DateTime)[8];
    private int numCasts;
    private DateTime firstActivation;

    public override void AddHints(int slot, Actor actor, TextHints hints)
    {
        if (numCasts == 8)
        {
            ref readonly var player = ref expirationBySlot[slot];

            bool? inRisk = null;
            if (player != default)
                hints.Add($"Order: {player.Order} -", false);
            for (var i = 0; i < 8; ++i)
            {
                ref readonly var exp = ref expirationBySlot[i];
                if (exp == default || slot == i)
                    continue;
                var remaining = Math.Max(0d, (exp.Expiration - WorldState.CurrentTime).TotalSeconds);
                var check = remaining < 5d;
                var partner = exp.Actor;
                if (exp.Order == player.Order)
                {
                    hints.Add($"Partner: {partner.Name} in {remaining:f0}s", check);
                }
                else if (check)
                {
                    if (actor.Position.InCircle(partner.Position, 2f))
                        inRisk = true;
                }
            }
            if (inRisk != null)
                hints.Add($"GTFO from incorrect stacks!");
        }
    }

    public override PlayerPriority CalcPriority(int pcSlot, Actor pc, int playerSlot, Actor player, ref uint customColor)
    {
        if (numCasts == 8)
        {
            ref readonly var pcOrder = ref expirationBySlot[pcSlot].Order;
            ref readonly var playerOrder = ref expirationBySlot[playerSlot].Order;
            if (pcOrder != default && pcOrder == playerOrder)
            {
                return PlayerPriority.Critical;
            }
            else if (playerOrder != default)
            {
                return PlayerPriority.Danger;
            }
        }
        return PlayerPriority.Irrelevant;
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        if (numCasts == 8)
        {
            ref readonly var player = ref expirationBySlot[pcSlot].Order;
            for (var i = 0; i < 8; ++i)
            {
                ref readonly var exp = ref expirationBySlot[i];
                if (exp == default || pcSlot == i)
                    continue;
                var remaining = Math.Max(0d, (exp.Expiration - WorldState.CurrentTime).TotalSeconds) < 5d;
                var partner = exp.Actor;
                if (!remaining)
                    continue;
                if (exp.Order == player)
                {
                    Arena.AddCircle(partner.Position, 2f, Colors.Safe);
                }
                else
                {
                    Arena.AddCircle(partner.Position, 2f);
                }
            }
        }
    }
    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if (status.ID is (uint)SID.WavelengthAlpha or (uint)SID.WavelengthBeta)
        {
            var slot = WorldState.Party.FindSlot(actor.InstanceID);
            if (slot < 0)
                return;
            if (firstActivation == default)
                firstActivation = WorldState.FutureTime(27.5d);
            var order = (status.ExpireAt - firstActivation).TotalSeconds switch
            {
                < 4d => 1,
                < 9d => 2,
                < 14d => 3,
                _ => 4
            };
            expirationBySlot[slot] = (order, actor, status.ExpireAt);
        }
    }

    public override void OnStatusLose(Actor actor, ActorStatus status)
    {
        if (status.ID is (uint)SID.WavelengthAlpha or (uint)SID.WavelengthBeta)
        {
            var slot = WorldState.Party.FindSlot(actor.InstanceID);
            if (slot < 0)
                return;
            expirationBySlot[slot] = default;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action.ID == (uint)AID.GetDownBait)
            ++numCasts;
    }
}
