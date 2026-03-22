using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Domain.Scene.Geometry;

namespace BabylonArchiveCore.Core.Scene;

/// <summary>
/// Complete spatial definition of scene A-0.
/// Owns the floor polygon, zone AABBs, and provides spatial queries.
/// </summary>
public sealed class SceneGeometry
{
    public OctagonalFloor Floor { get; }
    private readonly List<HubZone> _zones;

    public IReadOnlyList<HubZone> Zones => _zones;

    public SceneGeometry(OctagonalFloor floor, List<HubZone> zones)
    {
        Floor = floor;
        _zones = zones;
    }

    /// <summary>True if the position is inside the walkable area (floor polygon + height).</summary>
    public bool IsWalkable(Vec3 position) => Floor.IsWalkable(position);

    /// <summary>Returns the zone containing the given position, or null.</summary>
    public HubZone? GetZoneAt(Vec3 position) =>
        _zones.FirstOrDefault(z => z.Bounds.Contains(position));

    /// <summary>Returns all zones whose AABB contains the given position.</summary>
    public List<HubZone> GetZonesAt(Vec3 position) =>
        _zones.Where(z => z.Bounds.Contains(position)).ToList();

    /// <summary>
    /// Returns interactable objects within the given radius of <paramref name="position"/>,
    /// filtered by current rhythm phase.
    /// </summary>
    public List<InteractableObject> GetInteractablesInRange(
        Vec3 position, float radius, HubRhythmPhase currentPhase)
    {
        var radiusSq = radius * radius;
        return _zones
            .SelectMany(z => z.Objects)
            .Where(o => o.IsActiveIn(currentPhase) &&
                        (o.Position - position).LengthSquared() <= radiusSq)
            .ToList();
    }

    /// <summary>
    /// Returns the closest active interactable to <paramref name="position"/>,
    /// within <paramref name="maxDistance"/>, or null.
    /// </summary>
    public InteractableObject? GetNearestInteractable(
        Vec3 position, float maxDistance, HubRhythmPhase currentPhase)
    {
        var maxSq = maxDistance * maxDistance;
        InteractableObject? best = null;
        var bestDistSq = float.MaxValue;

        foreach (var obj in _zones.SelectMany(z => z.Objects))
        {
            if (!obj.IsActiveIn(currentPhase)) continue;
            var distSq = (obj.Position - position).LengthSquared();
            if (distSq <= maxSq && distSq < bestDistSq)
            {
                best = obj;
                bestDistSq = distSq;
            }
        }
        return best;
    }

    /// <summary>
    /// Minimum distance from the point to any wall segment (XZ).
    /// </summary>
    public float DistanceToNearestWall(Vec3 position)
    {
        var min = float.MaxValue;
        foreach (var wall in Floor.Walls)
        {
            var d = wall.DistanceXZ(position);
            if (d < min) min = d;
        }
        return min;
    }

    /// <summary>
    /// Clamp a proposed move so the player stays inside the floor polygon,
    /// maintaining <paramref name="wallMargin"/> metres from walls.
    /// Returns the clamped position. Y is unchanged.
    /// </summary>
    public Vec3 ClampToWalkable(Vec3 proposed, float wallMargin = 0.3f)
    {
        if (Floor.ContainsXZ(proposed) && DistanceToNearestWall(proposed) >= wallMargin)
            return proposed;

        // Push away from nearest wall
        var bestDist = float.MaxValue;
        Vec3 best = proposed;
        foreach (var wall in Floor.Walls)
        {
            var d = wall.DistanceXZ(proposed);
            if (d < bestDist)
            {
                bestDist = d;
                best = proposed; // keep Y
            }
        }

        // If still outside, project back to center
        if (!Floor.ContainsXZ(proposed))
        {
            // Binary search toward center
            var center = new Vec3(0, proposed.Y, 0);
            var lo = 0f;
            var hi = 1f;
            for (var i = 0; i < 16; i++)
            {
                var mid = (lo + hi) / 2f;
                var test = Lerp(proposed, center, mid);
                if (Floor.ContainsXZ(test) && DistanceToNearestWall(test) >= wallMargin)
                    hi = mid;
                else
                    lo = mid;
            }
            return Lerp(proposed, center, hi);
        }

        return proposed;
    }

    private static Vec3 Lerp(Vec3 a, Vec3 b, float t) =>
        new(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t, a.Z + (b.Z - a.Z) * t);
}
