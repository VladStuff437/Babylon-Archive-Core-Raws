using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Core.Input;

/// <summary>
/// Immutable snapshot of input state for a single frame.
/// Built from raw input polling and consumed by PlayerController / GameplaySession.
/// </summary>
public readonly record struct InputSnapshot
{
    /// <summary>Normalised movement direction in XZ plane (length 0–1).</summary>
    public Vec3 MoveDirection { get; init; }

    /// <summary>True if the interact key was pressed this frame.</summary>
    public bool InteractPressed { get; init; }

    /// <summary>True if camera toggle was pressed this frame.</summary>
    public bool CameraTogglePressed { get; init; }

    /// <summary>When true, PlayerController applies FacingDirection instead of deriving facing from MoveDirection.</summary>
    public bool ApplyFacingDirection { get; init; }

    /// <summary>Optional explicit facing for profile-driven schemes.</summary>
    public Vec3 FacingDirection { get; init; }

    /// <summary>Create a snapshot from WASD-style held flags.</summary>
    public static InputSnapshot FromActions(bool up, bool down, bool left, bool right,
        bool interact = false, bool cameraToggle = false)
    {
        var dx = (right ? 1f : 0f) - (left ? 1f : 0f);
        var dz = (up ? 1f : 0f) - (down ? 1f : 0f);
        var dir = new Vec3(dx, 0f, dz).Normalized();
        return new InputSnapshot
        {
            MoveDirection = dir,
            InteractPressed = interact,
            CameraTogglePressed = cameraToggle,
            ApplyFacingDirection = false,
            FacingDirection = Vec3.Zero,
        };
    }

    /// <summary>No input this frame.</summary>
    public static InputSnapshot None => default;
}
