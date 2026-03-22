namespace BabylonArchiveCore.Domain.Scene;

/// <summary>
/// Immutable camera profile describing offset, pitch, and distance for a camera mode.
/// </summary>
public sealed record CameraProfile
{
    public CameraMode Mode { get; init; }

    /// <summary>Offset from the player in world-space (before pitch rotation).</summary>
    public Vec3 Offset { get; init; }

    /// <summary>Camera pitch in degrees (0 = horizontal, 90 = straight down).</summary>
    public float PitchDegrees { get; init; }

    /// <summary>Distance from the focus point in metres.</summary>
    public float Distance { get; init; }

    /// <summary>Field of view in degrees (only used in 3D mode).</summary>
    public float FieldOfView { get; init; } = 60f;

    /// <summary>Smoothing factor for follow interpolation (0 = instant, higher = slower).</summary>
    public float SmoothSpeed { get; init; } = 8f;

    /// <summary>Canon 3D third-person profile for A-0.</summary>
    public static CameraProfile Default3D => new()
    {
        Mode = CameraMode.ThirdPerson3D,
        Offset = new Vec3(0f, 8f, -10f),
        PitchDegrees = 35f,
        Distance = 12f,
        FieldOfView = 60f,
        SmoothSpeed = 8f,
    };

    /// <summary>Canon 2.5D isometric profile for A-0.</summary>
    public static CameraProfile Default25D => new()
    {
        Mode = CameraMode.Isometric25D,
        Offset = new Vec3(0f, 14f, -14f),
        PitchDegrees = 45f,
        Distance = 20f,
        FieldOfView = 45f,
        SmoothSpeed = 6f,
    };
}
