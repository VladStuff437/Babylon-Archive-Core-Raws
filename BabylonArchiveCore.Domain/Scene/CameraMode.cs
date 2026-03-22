namespace BabylonArchiveCore.Domain.Scene;

/// <summary>
/// Camera viewing mode. V1 supports two switchable profiles.
/// </summary>
public enum CameraMode
{
    /// <summary>Third-person 3D camera behind and above the player.</summary>
    ThirdPerson3D = 0,

    /// <summary>Isometric 2.5D top-down with fixed 45° angle.</summary>
    Isometric25D = 1,
}
