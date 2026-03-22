namespace BabylonArchiveCore.Domain.Scene;

/// <summary>
/// 3D vector for world-space positions. Framework-independent.
/// X = west/east, Y = height, Z = south/north.
/// </summary>
public readonly record struct Vec3(float X, float Y, float Z)
{
    public static readonly Vec3 Zero = new(0f, 0f, 0f);

    public static Vec3 operator +(Vec3 a, Vec3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vec3 operator -(Vec3 a, Vec3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vec3 operator *(Vec3 v, float s) => new(v.X * s, v.Y * s, v.Z * s);
    public static Vec3 operator *(float s, Vec3 v) => v * s;

    public float LengthSquared() => X * X + Y * Y + Z * Z;
    public float Length() => MathF.Sqrt(LengthSquared());

    public Vec3 Normalized()
    {
        var len = Length();
        return len < 1e-6f ? Zero : new Vec3(X / len, Y / len, Z / len);
    }

    /// <summary>Linear interpolation toward <paramref name="target"/>.</summary>
    public Vec3 Lerp(Vec3 target, float t) =>
        new(X + (target.X - X) * t, Y + (target.Y - Y) * t, Z + (target.Z - Z) * t);

    /// <summary>Project to XZ plane as Vec2 (top-down).</summary>
    public Vec2 ToVec2() => new(X, Z);
}

/// <summary>
/// Axis-aligned bounding box in 3D world space.
/// </summary>
public readonly record struct Bounds3D(float X, float Y, float Z, float Width, float Height, float Depth)
{
    public float Right => X + Width;
    public float Top => Y + Height;
    public float Far => Z + Depth;

    public bool Contains(Vec3 point) =>
        point.X >= X && point.X <= Right &&
        point.Y >= Y && point.Y <= Top &&
        point.Z >= Z && point.Z <= Far;
}
