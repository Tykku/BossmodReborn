namespace BossMod.Dawntrail.Alliance.A14ShadowLord;

class GigaSlash(BossModule module) : Components.GenericAOEs(module)
{
    public readonly List<AOEInstance> AOEs = [];
    private static readonly HashSet<AID> castEnds = [AID.GigaSlashLAOE1, AID.GigaSlashRAOE2, AID.GigaSlashRAOE1, AID.GigaSlashLAOE2, AID.GigaSlashNightfallFAOE3,
    AID.GigaSlashNightfallBAOE3, AID.GigaSlashNightfallLAOE1, AID.GigaSlashNightfallRAOE2, AID.GigaSlashNightfallRAOE1, AID.GigaSlashNightfallLAOE2];
    private static readonly AOEShapeCone[] _shapes = [new(60, 112.5f.Degrees()), new(60, 135.Degrees()), new(60, 105.Degrees())];

    public override IEnumerable<AOEInstance> ActiveAOEs(int slot, Actor actor)
    {
        if (AOEs.Count > 0)
            yield return AOEs[0] with { Risky = Module.FindComponent<DarkNebula>()?.Casters.Count == 0 };
    }

    public override void AddAIHints(int slot, Actor actor, PartyRolesConfig.Assignment assignment, AIHints hints)
    {
        if (AOEs.Count == 0)
            return;
        base.AddAIHints(slot, actor, assignment, hints);
        // stay close to the middle if there is next imminent aoe from same origin
        if (Module.FindComponent<DarkNebula>()?.Casters.Count == 0 && AOEs.Count > 1 && AOEs[0].Origin == AOEs[1].Origin)
            hints.AddForbiddenZone(ShapeDistance.InvertedCircle(AOEs[0].Origin, 3), AOEs[0].Activation);
    }

    public override void OnCastStarted(Actor caster, ActorCastInfo spell)
    {
        void AddAOE(AOEShapeCone shape, float rotationOffset, float finishOffset)
        => AOEs.Add(new(shape, caster.Position, spell.Rotation + rotationOffset.Degrees(), Module.CastFinishAt(spell, finishOffset)));

        switch ((AID)spell.Action.ID)
        {
            case AID.GigaSlashL:
                AddAOE(_shapes[0], 67.5f, 1);
                AddAOE(_shapes[1], -90, 3.1f);
                break;
            case AID.GigaSlashR:
                AddAOE(_shapes[0], -67.5f, 1);
                AddAOE(_shapes[1], 90, 3.1f);
                break;
            case AID.GigaSlashNightfallLRF:
                AddAOE(_shapes[0], 67.5f, 1);
                AddAOE(_shapes[1], -90, 3.1f);
                AddAOE(_shapes[2], 0, 5.2f);
                break;
            case AID.GigaSlashNightfallLRB:
                AddAOE(_shapes[0], 67.5f, 1);
                AddAOE(_shapes[1], -90, 3.1f);
                AddAOE(_shapes[2], 180, 5.2f);
                break;
            case AID.GigaSlashNightfallRLF:
                AddAOE(_shapes[0], -67.5f, 1);
                AddAOE(_shapes[1], 90, 3.1f);
                AddAOE(_shapes[2], 0, 5.2f);
                break;
            case AID.GigaSlashNightfallRLB:
                AddAOE(_shapes[0], -67.5f, 1);
                AddAOE(_shapes[1], 90, 3.1f);
                AddAOE(_shapes[2], 180, 5.2f);
                break;
        }
    }

    public override void OnEventCast(Actor caster, ActorCastEvent spell)
    {
        if (castEnds.Contains((AID)spell.Action.ID))
        {
            ++NumCasts;
            if (AOEs.Count != 0)
                AOEs.RemoveAt(0);
        }
    }
}
