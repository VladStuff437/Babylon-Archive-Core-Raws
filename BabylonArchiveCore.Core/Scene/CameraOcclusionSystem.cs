using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Core.Scene;

/// <summary>
/// Camera occlusion system. Given a fixed camera angle, determines which
/// scene objects block the player and should receive transparency.
/// Session 2: supports both 2D and 3D inputs (3D projects to XZ).
/// </summary>
public sealed class CameraOcclusionSystem
{
    /// <summary>
    /// Fixed camera offset from the player in 2D top-down space.
    /// The camera sits at PlayerPos + CameraOffset.
    /// </summary>
    public Vec2 CameraOffset { get; init; } = new(200f, -300f);

    /// <summary>
    /// 3D overload: projects positions to XZ and object bounds to 2D for occlusion.
    /// </summary>
    public List<OcclusionHit> ComputeOcclusions(Vec3 playerPos3D, IReadOnlyList<Bounds3D> obstacles3D)
    {
        var playerPos = playerPos3D.ToVec2();
        var obstacles2D = new List<Bounds2D>(obstacles3D.Count);
        foreach (var b in obstacles3D)
            obstacles2D.Add(new Bounds2D(b.X, b.Z, b.Width, b.Depth));
        return ComputeOcclusions(playerPos, obstacles2D);
    }

    /// <summary>
    /// Determines which objects occlude the player and should be rendered translucent.
    /// Uses AABB intersection with the camera→player line segment.
    /// </summary>
    public List<OcclusionHit> ComputeOcclusions(Vec2 playerPos, IReadOnlyList<Bounds2D> obstacles)
    {
        var cameraPos = playerPos + CameraOffset;
        var hits = new List<OcclusionHit>();

        for (var i = 0; i < obstacles.Count; i++)
        {
            var box = obstacles[i];
            if (SegmentIntersectsAABB(cameraPos, playerPos, box))
            {
                var center = new Vec2(box.X + box.Width / 2f, box.Y + box.Height / 2f);
                var dist = (center - playerPos).LengthSquared();
                hits.Add(new OcclusionHit(i, dist));
            }
        }

        return hits;
    }

    /// <summary>
    /// Cohen–Sutherland-style segment vs AABB test.
    /// </summary>
    internal static bool SegmentIntersectsAABB(Vec2 p0, Vec2 p1, Bounds2D box)
    {
        var dx = p1.X - p0.X;
        var dy = p1.Y - p0.Y;

        float tMin = 0f, tMax = 1f;

        if (!SlabTest(p0.X, dx, box.X, box.Right, ref tMin, ref tMax)) return false;
        if (!SlabTest(p0.Y, dy, box.Y, box.Bottom, ref tMin, ref tMax)) return false;

        return tMin <= tMax;
    }

    private static bool SlabTest(float origin, float dir, float min, float max,
        ref float tMin, ref float tMax)
    {
        if (MathF.Abs(dir) < 1e-8f)
            return origin >= min && origin <= max;

        var t1 = (min - origin) / dir;
        var t2 = (max - origin) / dir;
        if (t1 > t2) (t1, t2) = (t2, t1);

        tMin = MathF.Max(tMin, t1);
        tMax = MathF.Min(tMax, t2);
        return tMin <= tMax;
    }
}

/// <summary>
/// Result of an occlusion check: obstacle index + squared distance to player.
/// </summary>
public readonly record struct OcclusionHit(int ObstacleIndex, float DistanceSquaredToPlayer);
