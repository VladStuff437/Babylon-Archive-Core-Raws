using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Domain.Scene.Geometry;

namespace BabylonArchiveCore.Core.Scene;

/// <summary>
/// 3D collision detection for scene movement and interaction.
/// </summary>
public sealed class CollisionSystem3D
{
    private readonly SceneGeometry _geometry;
    private readonly float _playerRadius;

    public CollisionSystem3D(SceneGeometry geometry, float playerRadius = 0.4f)
    {
        _geometry = geometry;
        _playerRadius = playerRadius;
    }

    /// <summary>
    /// Attempt to move from <paramref name="current"/> to <paramref name="desired"/>.
    /// Returns the final valid position (may be clamped or blocked).
    /// </summary>
    public MoveResult TryMove(Vec3 current, Vec3 desired)
    {
        // 1. Stay on the floor plane (Y clamped to floor)
        var target = new Vec3(desired.X, _geometry.Floor.FloorY, desired.Z);

        // 2. Check walkable area
        if (!_geometry.Floor.ContainsXZ(target))
        {
            var clamped = _geometry.ClampToWalkable(target, _playerRadius);
            return new MoveResult(clamped, true, false);
        }

        // 3. Check wall margin
        var wallDist = _geometry.DistanceToNearestWall(target);
        if (wallDist < _playerRadius)
        {
            var clamped = _geometry.ClampToWalkable(target, _playerRadius);
            return new MoveResult(clamped, true, false);
        }

        // 4. Check AABB collisions with locked zone boundaries
        foreach (var zone in _geometry.Zones)
        {
            if (!IsBlockingZone(zone)) continue;
            if (zone.Bounds.Contains(target))
            {
                return new MoveResult(current, true, true);
            }
        }

        return new MoveResult(target, false, false);
    }

    /// <summary>
    /// True if the player at <paramref name="position"/> can interact with <paramref name="obj"/>.
    /// Checks distance ≤ InteractionRadius AND same-phase requirement.
    /// </summary>
    public bool CanInteract(Vec3 position, InteractableObject obj, HubRhythmPhase currentPhase)
    {
        if (!obj.IsActiveIn(currentPhase)) return false;
        var distSq = (obj.Position - position).LengthSquared();
        return distSq <= obj.InteractionRadius * obj.InteractionRadius;
    }

    private static bool IsBlockingZone(HubZone zone) =>
        zone.Id is HubZoneId.CommerceGateLocked or HubZoneId.TechGateLocked;
}

/// <summary>
/// Result of a movement attempt.
/// </summary>
public readonly record struct MoveResult(Vec3 FinalPosition, bool WasClamped, bool WasBlocked);
