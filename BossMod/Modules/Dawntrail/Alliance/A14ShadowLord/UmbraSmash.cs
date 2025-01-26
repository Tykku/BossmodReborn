namespace BossMod.Dawntrail.Alliance.A14ShadowLord;

class UmbraSmash(BossModule module) : Components.Exaflare(module, new AOEShapeRect(5, 30))
{
    private static readonly HashSet<AID> casts = [AID.UmbraSmashAOE1, AID.UmbraSmashAOE2, AID.UmbraSmashAOE3, AID.UmbraSmashAOE4, AID.UmbraSmashAOEClone];
    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        var linesCount = Lines.Count;
        if (linesCount == 0)
            return;

        var imminentAOEs = ImminentAOEs(linesCount);

        // use only imminent aoes for hints
        for (var i = 0; i < imminentAOEs.Length; ++i)
        {
            var aoe = imminentAOEs[i];
            hints.AddForbiddenZone(Shape, aoe.Item1, aoe.Item3, aoe.Item2);
        }
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        if ((AID)spell.Action.ID is AID.UmbraSmashAOE1 or AID.UmbraSmashAOE2 or AID.UmbraSmashAOE3 or AID.UmbraSmashAOE4 or AID.UmbraSmashAOEClone)
        {
            var dir = spell.Rotation.ToDirection();
            var origin = spell.LocXZ + 30 * dir;
            Lines.Add(new() { Next = origin, Advance = 5 * dir.OrthoL(), Rotation = spell.Rotation + 90.Degrees(), NextExplosion = Module.CastFinishAt(spell), TimeToMove = 2.3f, ExplosionsLeft = 6, MaxShownExplosions = 2 });
            Lines.Add(new() { Next = origin, Advance = 5 * dir.OrthoR(), Rotation = spell.Rotation - 90.Degrees(), NextExplosion = Module.CastFinishAt(spell), TimeToMove = 2.3f, ExplosionsLeft = 6, MaxShownExplosions = 2 });
        }
    }

    public override void OnCastFinished(Actor caster, ActorCastInfo spell)
    {
        if (casts.Contains((AID)spell.Action.ID))
        {
            ++NumCasts;
            var origin = spell.LocXZ + 30 * spell.Rotation.ToDirection();
            foreach (var l in Lines.Where(l => l.Next.AlmostEqual(origin, 1)))
            {
                l.Next = origin + l.Advance;
                l.TimeToMove = 1;
                l.NextExplosion = WorldState.FutureTime(l.TimeToMove);
                --l.ExplosionsLeft;
            }
        }
        else if ((AID)spell.Action.ID == AID.UmbraWave)
        {
            ++NumCasts;
            var index = Lines.FindIndex(item => item.Next.AlmostEqual(caster.Position, 1));
            if (index == -1)
            {
                ReportError($"Failed to find entry for {caster.InstanceID:X}");
                return;
            }

            AdvanceLine(Lines[index], caster.Position);
            if (Lines[index].ExplosionsLeft == 0)
                Lines.RemoveAt(index);
        }
    }
}
