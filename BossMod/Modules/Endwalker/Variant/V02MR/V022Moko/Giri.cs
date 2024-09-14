namespace BossMod.Endwalker.VariantCriterion.V02MR.V022Moko;

class Giri(BossModule module) : Components.GenericAOEs(module)
{
    private enum NextSafeDirection { None, Front, Back, Left, Right }
    private NextSafeDirection nextDirection;
    private static readonly AOEShapeCone cone = new(60, 135.Degrees());
    private readonly List<AOEInstance> _aoes = [];
    private (WPos, Angle, DateTime) initialRotation;

    private static readonly HashSet<AID> casts =
    [
        AID.IaiKasumiGiri1, AID.IaiKasumiGiri2, AID.IaiKasumiGiri3, AID.IaiKasumiGiri4,
        AID.DoubleKasumiGiriFirst1, AID.DoubleKasumiGiriFirst2, AID.DoubleKasumiGiriFirst3, AID.DoubleKasumiGiriFirst4,
        AID.DoubleKasumiGiriRest1, AID.DoubleKasumiGiriRest2, AID.DoubleKasumiGiriRest3, AID.DoubleKasumiGiriRest4
    ];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor) => _aoes.Take(1);
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (casts.Take(8).ToHashSet().Contains((AID)spell.Action.ID))
        {
            initialRotation = (caster.Position, spell.Rotation, Module.CastFinishAt(spell));
            InitIfReady();
        }
    }

    public override void OnStatusGain(Actor actor, ActorStatus status)
    {
        if ((SID)status.ID == SID.GiriDirection)
        {
            nextDirection = status.Extra switch
            {
                0x248 => NextSafeDirection.Back,
                0x24A => NextSafeDirection.Front,
                0x249 => NextSafeDirection.Left,
                0x24B => NextSafeDirection.Right,
                _ => NextSafeDirection.None
            };
            InitIfReady();
        }
    }

    private void InitIfReady()
    {
        if (nextDirection != NextSafeDirection.None && initialRotation != default)
        {
            var count = _aoes.Count == 1;
            var angle = initialRotation.Item2;
            var rotation = CalculateRotation(nextDirection, angle, count);
            var finishTime = count ? initialRotation.Item3.AddSeconds(3.5f) : initialRotation.Item3;

            _aoes.Add(new(cone, initialRotation.Item1, rotation, finishTime));
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (_aoes.Count > 0 && casts.Contains((AID)spell.Action.ID))
        {
            _aoes.RemoveAt(0);
            if (_aoes.Count == 0)
            {
                initialRotation = default;
                nextDirection = NextSafeDirection.None;
            }
        }
    }

    private static Angle CalculateRotation(NextSafeDirection direction, Angle baseAngle, bool isFirstAoe)
        => direction switch
        {
            NextSafeDirection.Back => baseAngle,
            NextSafeDirection.Front => isFirstAoe ? baseAngle + 180.Degrees() : baseAngle,
            NextSafeDirection.Left => isFirstAoe ? baseAngle - 90.Degrees() : baseAngle,
            NextSafeDirection.Right => isFirstAoe ? baseAngle + 90.Degrees() : baseAngle,
            _ => default
        };
}
