using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Core.Camera;

/// <summary>
/// Camera controller with smooth follow and 3D ↔ 2.5D mode switching.
/// Computes world-space camera position from the active CameraProfile and player position.
/// </summary>
public sealed class CameraController
{
    private CameraProfile _activeProfile;
    private Vec3 _currentPosition;

    public CameraMode ActiveMode => _activeProfile.Mode;
    public CameraProfile ActiveProfile => _activeProfile;
    public Vec3 Position => _currentPosition;

    public CameraController(CameraProfile initialProfile, Vec3 initialPlayerPos)
    {
        _activeProfile = initialProfile;
        _currentPosition = ComputeTarget(initialPlayerPos);
    }

    /// <summary>
    /// Switch between 3D and 2.5D camera modes.
    /// </summary>
    public void ToggleMode()
    {
        _activeProfile = _activeProfile.Mode == CameraMode.ThirdPerson3D
            ? CameraProfile.Default25D
            : CameraProfile.Default3D;
    }

    /// <summary>
    /// Set a specific camera profile.
    /// </summary>
    public void SetProfile(CameraProfile profile) => _activeProfile = profile;

    /// <summary>
    /// Update camera position with smooth interpolation toward the player.
    /// </summary>
    public void Update(Vec3 playerPos, float deltaTime)
    {
        var target = ComputeTarget(playerPos);
        var t = MathF.Min(1f, _activeProfile.SmoothSpeed * deltaTime);
        _currentPosition = _currentPosition.Lerp(target, t);
    }

    /// <summary>
    /// Snap camera to target position instantly (no interpolation).
    /// </summary>
    public void SnapTo(Vec3 playerPos)
    {
        _currentPosition = ComputeTarget(playerPos);
    }

    /// <summary>
    /// Compute the ideal camera position for the given player position.
    /// </summary>
    public Vec3 ComputeTarget(Vec3 playerPos) => playerPos + _activeProfile.Offset;
}
