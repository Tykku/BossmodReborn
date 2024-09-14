namespace BossMod;

// helpers that can potentially be reused for different things

public static class Helpers
{
    public static readonly float sqrt3 = MathF.Sqrt(3);
    private static readonly Dictionary<object, object> cache = [];
    public static readonly Angle[] AnglesIntercardinals = [-45.003f.Degrees(), 44.998f.Degrees(), 134.999f.Degrees(), -135.005f.Degrees()];
    public static readonly Angle[] AnglesCardinals = [-90.004f.Degrees(), -0.003f.Degrees(), 180.Degrees(), 89.999f.Degrees()];

    public static WPos RotateAroundOrigin(float rotatebydegrees, WPos origin, WPos caster)
    {
        if (cache.TryGetValue((rotatebydegrees, origin, caster), out var cachedResult))
            return (WPos)cachedResult;
        var x = MathF.Cos(rotatebydegrees * Angle.DegToRad) * (caster.X - origin.X) - MathF.Sin(rotatebydegrees * Angle.DegToRad) * (caster.Z - origin.Z);
        var z = MathF.Sin(rotatebydegrees * Angle.DegToRad) * (caster.X - origin.X) + MathF.Cos(rotatebydegrees * Angle.DegToRad) * (caster.Z - origin.Z);
        var result = new WPos(origin.X + x, origin.Z + z);
        cache[(rotatebydegrees, origin, caster)] = result;
        return result;
    }
}
