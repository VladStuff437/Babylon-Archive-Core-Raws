using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Domain.Player;

/// <summary>
/// Mutable player state for the operator (Alan Arcwain).
/// Tracks 3D position, facing direction, and movement parameters.
/// </summary>
public sealed class PlayerEntity
{
    public string Name { get; init; } = "Алан Арквейн";
    public Vec3 Position { get; set; } = new(-8f, 0f, -6f); // Start in capsule
    public Vec3 Facing { get; set; } = new(1f, 0f, 0f); // Facing east initially

    /// <summary>Movement speed in metres per second.</summary>
    public float MoveSpeed { get; init; } = 4.0f;

    /// <summary>Maximum distance at which the player can interact with objects.</summary>
    public float InteractReach { get; init; } = 2.0f;

    /// <summary>True if the player is currently moving (non-zero input).</summary>
    public bool IsMoving { get; set; }
}
