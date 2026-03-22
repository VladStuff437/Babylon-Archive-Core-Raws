namespace BabylonArchiveCore.Domain.Scene;

/// <summary>
/// Minimal 2D vector for scene layout positions (no MonoGame dependency).
/// </summary>
public readonly record struct Vec2(float X, float Y)
{
    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);

    public float LengthSquared() => X * X + Y * Y;
}

/// <summary>
/// Axis-aligned bounding box for 2.5D scene objects.
/// </summary>
public readonly record struct Bounds2D(float X, float Y, float Width, float Height)
{
    public float Right => X + Width;
    public float Bottom => Y + Height;

    public bool Contains(Vec2 point) =>
        point.X >= X && point.X <= Right && point.Y >= Y && point.Y <= Bottom;
}
