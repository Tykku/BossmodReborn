namespace BossMod.Dawntrail.Ultimate.FRU;

// boss can spawn either N or S from center
class P4Preposition(BossModule module) : BossComponent(module)
{
    private readonly List<Actor> _boss = module.Enemies(OID.UsurperOfFrostP4);

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        for (var i = 0; i < _boss.Count; ++i)
            hints.AddForbiddenZone(ShapeDistance.InvertedCircle(_boss[i].Position, 8), DateTime.MaxValue);
    }

    public override void DrawArenaForeground(int pcSlot, Actor pc)
    {
        for (var i = 0; i < _boss.Count; ++i)
            Arena.AddCircle(_boss[i].Position, 1, Colors.Safe);
    }
}
