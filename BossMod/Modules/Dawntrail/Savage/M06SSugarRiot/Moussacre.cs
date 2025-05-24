namespace BossMod.Dawntrail.Savage.M06SSugarRiot;

class Moussacre(BossModule module) : Components.GenericBaitAway(module)
{
    private DateTime _activation;

    private static readonly AOEShapeCone cone = new(60f, 22.5f.Degrees());

    public override void Update()
    {
        CurrentBaits.Clear();
        if (_activation != default)
        {
            var party = Raid.WithoutSlot(false, true, true);
            var source = Module.PrimaryActor.Position;
            Array.Sort(party, (a, b) =>
                {
                    var distA = (a.Position - source).LengthSq();
                    var distB = (b.Position - source).LengthSq();
                    return distA.CompareTo(distB);
                });
            var len = party.Length;
            var max = len > 4 ? 4 : len;

            for (var i = 0; i < max; ++i)
            {
                ref readonly var p = ref party[i];
                CurrentBaits.Add(new(Module.PrimaryActor, p, cone, _activation));
            }
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if (spell.Action.ID == (uint)AID.MoussacreVisual)
        {
            _activation = Module.CastFinishAt(spell, 0.7f);
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (spell.Action.ID == (uint)AID.Moussacre)
        {
            ++NumCasts;
            CurrentBaits.Clear();
            _activation = default;
        }
    }
}
