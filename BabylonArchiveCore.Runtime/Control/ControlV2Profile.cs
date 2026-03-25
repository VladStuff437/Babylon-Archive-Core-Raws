namespace BabylonArchiveCore.Runtime.Control;

/// <summary>
/// Profile-driven movement/camera parameters.
/// </summary>
public sealed record ControlV2Profile
{
    public ControlSchemeId SchemeId { get; init; } = ControlSchemeId.ModernThirdPerson;

    // Movement
    public bool EnablePlayerMovement { get; init; } = true;
    public bool EnableKeyboardMovement { get; init; } = true;
    public bool UseCameraRelativeMovement { get; init; } = true;
    public float MoveSpeedForwardMultiplier { get; init; } = 1.0f;
    public float MoveSpeedBackwardMultiplier { get; init; } = 0.82f;
    public float MoveSpeedStrafeMultiplier { get; init; } = 1.0f;
    public bool InvertMoveForwardInput { get; init; }
    public bool InvertMoveLateralInput { get; init; }
    public bool NormalizeDiagonalMovement { get; init; } = true;

    // Rotation
    public bool EnablePlayerTurning { get; init; } = true;
    public bool EnableKeyboardTurning { get; init; } = true;
    public float TurnSpeed { get; init; } = 2.8f;
    public float RotationSmoothing { get; init; } = 12.0f;
    public bool InvertTurnInput { get; init; }
    public bool UseTankMovement { get; init; }
    public bool UseTankTurning { get; init; }
    public bool FaceMovementDirection { get; init; } = true;
    public bool FaceCameraDirectionWhenMoving { get; init; }
    public bool FaceCameraDirectionWhenIdle { get; init; }

    // Mouse player controls
    public bool EnableMousePlayerTurn { get; init; }
    public MouseControlButton MousePlayerTurnButton { get; init; } = MouseControlButton.Right;
    public float MousePlayerTurnSensitivity { get; init; } = 0.026f;
    public bool InvertMousePlayerTurn { get; init; }
    public bool EnableMousePlayerMove { get; init; }
    public MouseControlButton MousePlayerMoveButton { get; init; } = MouseControlButton.Right;
    public float MousePlayerMoveForwardSensitivity { get; init; } = 0.020f;
    public float MousePlayerMoveLateralSensitivity { get; init; } = 0.010f;
    public bool InvertMousePlayerMoveForward { get; init; }
    public bool InvertMousePlayerMoveLateral { get; init; }

    // Camera orbit/follow
    public bool EnableCameraOrbitInput { get; init; } = true;
    public MouseControlButton CameraOrbitButton { get; init; } = MouseControlButton.Right;
    public float OrbitSensitivity { get; init; } = 0.0075f;
    public float OrbitSensitivityVertical { get; init; } = 0.0040f;
    public float MaxOrbitOffsetYaw { get; init; } = 1.05f;
    public float MaxOrbitVerticalOffset { get; init; } = 1.8f;
    public float CameraDistance { get; init; } = 6.0f;
    public float CameraHeight { get; init; } = 4.8f;
    public float CameraSmoothing { get; init; } = 7.5f;
    public bool EnableCameraFollow { get; init; } = true;
    public bool EnableCameraAutoRotateWithPlayerTurn { get; init; } = true;
    public bool EnableFreeCameraMode { get; init; }
    public bool DecoupleCameraYaw { get; init; }
    public bool OrbitRecenteringEnabled { get; init; } = true;
    public float OrbitRecenteringMovingSpeed { get; init; } = 9.5f;
    public float OrbitRecenteringIdleSpeed { get; init; } = 5.5f;

    // Inversion and input shaping
    public bool InvertOrbitHorizontal { get; init; }
    public bool InvertOrbitVertical { get; init; }
    public float Deadzone { get; init; } = 0.02f;

    public static ControlV2Profile Default => new();
}
