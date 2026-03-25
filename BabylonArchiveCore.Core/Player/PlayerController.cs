using BabylonArchiveCore.Core.Input;
using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain.Player;
using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Core.Player;

/// <summary>
/// Processes input snapshots into validated player movement and interaction attempts.
/// Uses CollisionSystem3D to ensure all movement respects scene boundaries.
/// </summary>
public sealed class PlayerController
{
    private readonly PlayerEntity _player;
    private readonly CollisionSystem3D _collision;
    private readonly SceneGeometry _geometry;

    public PlayerEntity Player => _player;

    public PlayerController(PlayerEntity player, CollisionSystem3D collision, SceneGeometry geometry)
    {
        _player = player;
        _collision = collision;
        _geometry = geometry;
    }

    /// <summary>
    /// Process one frame of movement. Applies speed × deltaTime to direction,
    /// then validates via collision system.
    /// Returns the MoveResult from the collision system.
    /// </summary>
    public MoveResult ProcessMovement(InputSnapshot input, float deltaTime)
    {
        if (input.MoveDirection.LengthSquared() < 1e-6f)
        {
            _player.IsMoving = false;

            if (input.ApplyFacingDirection && input.FacingDirection.LengthSquared() > 1e-6f)
                _player.Facing = input.FacingDirection.Normalized();

            return new MoveResult(_player.Position, false, false);
        }

        var displacement = input.MoveDirection * (_player.MoveSpeed * deltaTime);
        var desired = _player.Position + displacement;
        var result = _collision.TryMove(_player.Position, desired);

        _player.Position = result.FinalPosition;
        if (input.ApplyFacingDirection && input.FacingDirection.LengthSquared() > 1e-6f)
            _player.Facing = input.FacingDirection.Normalized();
        else
            _player.Facing = input.MoveDirection;
        _player.IsMoving = true;

        return result;
    }

    /// <summary>
    /// Try to interact with the nearest object within reach.
    /// Returns the object ID and whether it was in range, or null if nothing nearby.
    /// </summary>
    public InteractionTarget? TryGetInteractionTarget(HubRhythmPhase currentPhase)
    {
        var nearest = _geometry.GetNearestInteractable(
            _player.Position, _player.InteractReach, currentPhase);

        if (nearest is null)
            return null;

        var canInteract = _collision.CanInteract(_player.Position, nearest, currentPhase);
        return new InteractionTarget(nearest.Id, nearest.DisplayName, canInteract);
    }

    /// <summary>
    /// Get the zone the player is currently standing in (or null).
    /// </summary>
    public HubZone? GetCurrentZone() => _geometry.GetZoneAt(_player.Position);
}

/// <summary>
/// Result of an interaction target query.
/// </summary>
public readonly record struct InteractionTarget(string ObjectId, string DisplayName, bool InRange);
