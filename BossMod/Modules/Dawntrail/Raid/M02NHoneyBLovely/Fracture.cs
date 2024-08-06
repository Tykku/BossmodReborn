namespace BossMod.Dawntrail.Raid.M02NHoneyBLovely;

class Fracture(BossModule module) : Components.GenericTowers(module)
{
    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID == AID.Fracture)
            Towers.Add(new(caster.Position, 4, 1, 1, default, Module.CastFinishAt(spell)));
    }

    public override void Update()
    {
        var updatedTowers = new List<Tower>();
        foreach (var tower in Towers)
        {
            var hearts2 = (uint)SID.HeadOverHeels;
            var hearts3 = (uint)SID.HopelessDevotion;
            var updatedTower = new Tower(tower.Position, tower.Radius, tower.MinSoakers, tower.MaxSoakers, Raid.WithSlot().WhereActor(p => p.Statuses.Where(i => i.ID == hearts2 || i.ID == hearts3).Any()).Mask(), tower.Activation);
            updatedTowers.Add(updatedTower);
        }
        Towers = updatedTowers;
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (Towers.Count > 0 && (AID)spell.Action.ID == AID.Fracture)
            Towers.RemoveAt(0);
    }
}
