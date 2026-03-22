namespace BabylonArchiveCore.Domain.Scene.Geometry;

/// <summary>
/// Regular octagon inscribed in a square of given side length, centred at the origin.
/// Y = floor height (default 0). Used for walkability checks and wall segments.
/// </summary>
public sealed class OctagonalFloor
{
    /// <summary>Side of the bounding square (metres). A-0 = 22.</summary>
    public float Size { get; }

    /// <summary>Y coordinate of the floor plane.</summary>
    public float FloorY { get; }

    /// <summary>Ceiling height above floor (metres). A-0 = 11.</summary>
    public float CeilingHeight { get; }

    /// <summary>8 vertices of the octagon in XZ plane (counter-clockwise from east edge).</summary>
    public Vec3[] Vertices { get; }

    /// <summary>8 wall segments connecting consecutive vertices.</summary>
    public WallSegment[] Walls { get; }

    public OctagonalFloor(float size, float ceilingHeight, float floorY = 0f)
    {
        Size = size;
        FloorY = floorY;
        CeilingHeight = ceilingHeight;

        var half = size / 2f;
        // Cut length for a regular octagon inscribed in a square: side * (1 − 1/√2) / 2
        var cut = half * (1f - 1f / MathF.Sqrt(2f));

        // 8 vertices counter-clockwise starting from "east-north" corner
        Vertices =
        [
            new Vec3(half,        floorY, half - cut),   // 0: east-north
            new Vec3(half - cut,  floorY, half),          // 1: north-east
            new Vec3(-(half - cut), floorY, half),        // 2: north-west
            new Vec3(-half,       floorY, half - cut),    // 3: west-north
            new Vec3(-half,       floorY, -(half - cut)), // 4: west-south
            new Vec3(-(half - cut), floorY, -half),       // 5: south-west
            new Vec3(half - cut,  floorY, -half),         // 6: south-east
            new Vec3(half,        floorY, -(half - cut)), // 7: east-south
        ];

        Walls = new WallSegment[8];
        for (var i = 0; i < 8; i++)
        {
            var a = Vertices[i];
            var b = Vertices[(i + 1) % 8];
            Walls[i] = new WallSegment(a, b, ceilingHeight);
        }
    }

    /// <summary>
    /// Returns true if the point's XZ projection lies inside the octagonal floor polygon.
    /// Uses ray-casting (parity) algorithm in XZ.
    /// </summary>
    public bool ContainsXZ(Vec3 point) => ContainsXZ(point.X, point.Z);

    public bool ContainsXZ(float x, float z)
    {
        var inside = false;
        var n = Vertices.Length;
        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            var zi = Vertices[i].Z;
            var zj = Vertices[j].Z;
            var xi = Vertices[i].X;
            var xj = Vertices[j].X;

            if ((zi > z) != (zj > z) &&
                x < (xj - xi) * (z - zi) / (zj - zi) + xi)
            {
                inside = !inside;
            }
        }
        return inside;
    }

    /// <summary>
    /// Full walkability test: inside octagon AND within floor/ceiling height band.
    /// </summary>
    public bool IsWalkable(Vec3 point) =>
        point.Y >= FloorY && point.Y <= FloorY + CeilingHeight && ContainsXZ(point);

    /// <summary>Axis-aligned bounding box enclosing the entire floor.</summary>
    public Bounds3D GetBounds()
    {
        var half = Size / 2f;
        return new Bounds3D(-half, FloorY, -half, Size, CeilingHeight, Size);
    }
}

/// <summary>
/// A wall segment between two floor vertices, extending upward by wallHeight metres.
/// </summary>
public readonly record struct WallSegment(Vec3 A, Vec3 B, float WallHeight)
{
    /// <summary>
    /// Returns the closest point on the wall line segment (XZ plane) to the given point,
    /// and the distance. Used for wall collision.
    /// </summary>
    public float DistanceXZ(Vec3 point)
    {
        var ax = A.X; var az = A.Z;
        var bx = B.X; var bz = B.Z;
        var dx = bx - ax; var dz = bz - az;
        var lenSq = dx * dx + dz * dz;
        if (lenSq < 1e-8f) return MathF.Sqrt((point.X - ax) * (point.X - ax) + (point.Z - az) * (point.Z - az));

        var t = MathF.Max(0f, MathF.Min(1f, ((point.X - ax) * dx + (point.Z - az) * dz) / lenSq));
        var cx = ax + t * dx;
        var cz = az + t * dz;
        return MathF.Sqrt((point.X - cx) * (point.X - cx) + (point.Z - cz) * (point.Z - cz));
    }
}
