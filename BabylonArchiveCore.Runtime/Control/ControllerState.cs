using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Runtime.Control;

public sealed class ControllerState
{
    public float PlayerYaw { get; set; }
    public float OrbitOffsetYaw { get; set; }
    public float OrbitVerticalOffset { get; set; }
    public float CameraYawSmoothed { get; set; }
    public bool CameraYawInitialized { get; set; }
    public bool IsOrbitModifierActive { get; set; }
    public bool IsUiInputCaptured { get; set; }

    public float CurrentCameraYaw => ControlV2Pipeline.NormalizeAngle(CameraYawSmoothed + OrbitOffsetYaw);

    public void ResetFromFacing(Vec3 facing)
    {
        PlayerYaw = ControlV2Pipeline.ComputeFacingYaw(facing);
        OrbitOffsetYaw = 0f;
        OrbitVerticalOffset = 0f;
        CameraYawSmoothed = PlayerYaw;
        CameraYawInitialized = true;
        IsOrbitModifierActive = false;
    }
}
