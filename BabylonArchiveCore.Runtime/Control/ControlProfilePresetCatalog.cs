namespace BabylonArchiveCore.Runtime.Control;

public sealed record ControlProfilePreset(string Id, string DisplayName, ControlV2Profile Profile, bool IsCustom);

public static class ControlProfilePresetCatalog
{
    public const string ModernThirdPersonId = "modern-third-person";
    public const string ClassicTankId = "classic-tank";
    public const string ActionStrafeId = "action-strafe";
    public const string MmoFreeLookId = "mmo-free-look";
    public const string TacticalLockedOrbitId = "tactical-locked-orbit";

    public static IReadOnlyList<ControlProfilePreset> BuiltInPresets { get; } =
    [
        new(
            ModernThirdPersonId,
            "Modern Third-Person",
            ControlV2Profile.Default with
            {
                SchemeId = ControlSchemeId.ModernThirdPerson,
                MoveSpeedForwardMultiplier = 1.0f,
                MoveSpeedBackwardMultiplier = 0.82f,
                MoveSpeedStrafeMultiplier = 1.0f,
                TurnSpeed = 2.8f,
                RotationSmoothing = 12.0f,
                UseTankMovement = false,
                UseTankTurning = false,
                FaceMovementDirection = true,
                FaceCameraDirectionWhenMoving = false,
                FaceCameraDirectionWhenIdle = false,
                DecoupleCameraYaw = false,
                OrbitSensitivity = 0.0075f,
                OrbitSensitivityVertical = 0.0040f,
                MaxOrbitOffsetYaw = 1.05f,
                CameraDistance = 6.0f,
                CameraHeight = 4.8f,
                CameraSmoothing = 7.5f,
                OrbitRecenteringEnabled = true,
                OrbitRecenteringMovingSpeed = 9.5f,
                OrbitRecenteringIdleSpeed = 5.5f,
                Deadzone = 0.02f,
                NormalizeDiagonalMovement = true,
            },
            IsCustom: false),
        new(
            ClassicTankId,
            "Classic Tank",
            ControlV2Profile.Default with
            {
                SchemeId = ControlSchemeId.ClassicTank,
                MoveSpeedForwardMultiplier = 1.0f,
                MoveSpeedBackwardMultiplier = 0.70f,
                MoveSpeedStrafeMultiplier = 0.0f,
                TurnSpeed = 2.6f,
                RotationSmoothing = 8.5f,
                UseTankMovement = true,
                UseTankTurning = true,
                FaceMovementDirection = false,
                FaceCameraDirectionWhenMoving = false,
                FaceCameraDirectionWhenIdle = false,
                DecoupleCameraYaw = false,
                OrbitSensitivity = 0.0060f,
                OrbitSensitivityVertical = 0.0035f,
                MaxOrbitOffsetYaw = 0.85f,
                CameraDistance = 6.2f,
                CameraHeight = 4.8f,
                CameraSmoothing = 6.0f,
                OrbitRecenteringEnabled = true,
                OrbitRecenteringMovingSpeed = 12.0f,
                OrbitRecenteringIdleSpeed = 8.0f,
                Deadzone = 0.03f,
                NormalizeDiagonalMovement = true,
            },
            IsCustom: false),
        new(
            ActionStrafeId,
            "Action Strafe",
            ControlV2Profile.Default with
            {
                SchemeId = ControlSchemeId.ActionStrafe,
                MoveSpeedForwardMultiplier = 1.05f,
                MoveSpeedBackwardMultiplier = 0.86f,
                MoveSpeedStrafeMultiplier = 1.08f,
                TurnSpeed = 3.2f,
                RotationSmoothing = 16.0f,
                UseTankMovement = false,
                UseTankTurning = false,
                FaceMovementDirection = true,
                FaceCameraDirectionWhenMoving = true,
                FaceCameraDirectionWhenIdle = false,
                DecoupleCameraYaw = false,
                OrbitSensitivity = 0.0090f,
                OrbitSensitivityVertical = 0.0045f,
                MaxOrbitOffsetYaw = 1.15f,
                CameraDistance = 5.4f,
                CameraHeight = 4.4f,
                CameraSmoothing = 9.0f,
                OrbitRecenteringEnabled = true,
                OrbitRecenteringMovingSpeed = 11.0f,
                OrbitRecenteringIdleSpeed = 6.0f,
                Deadzone = 0.02f,
                NormalizeDiagonalMovement = true,
            },
            IsCustom: false),
        new(
            MmoFreeLookId,
            "MMO Free-Look",
            ControlV2Profile.Default with
            {
                SchemeId = ControlSchemeId.MmoFreeLook,
                MoveSpeedForwardMultiplier = 1.0f,
                MoveSpeedBackwardMultiplier = 0.82f,
                MoveSpeedStrafeMultiplier = 1.0f,
                TurnSpeed = 2.2f,
                RotationSmoothing = 7.5f,
                UseTankMovement = false,
                UseTankTurning = false,
                FaceMovementDirection = true,
                FaceCameraDirectionWhenMoving = false,
                FaceCameraDirectionWhenIdle = false,
                DecoupleCameraYaw = true,
                OrbitSensitivity = 0.0070f,
                OrbitSensitivityVertical = 0.0038f,
                MaxOrbitOffsetYaw = 1.30f,
                CameraDistance = 6.4f,
                CameraHeight = 5.0f,
                CameraSmoothing = 5.5f,
                OrbitRecenteringEnabled = false,
                OrbitRecenteringMovingSpeed = 3.0f,
                OrbitRecenteringIdleSpeed = 1.5f,
                Deadzone = 0.02f,
                NormalizeDiagonalMovement = true,
            },
            IsCustom: false),
        new(
            TacticalLockedOrbitId,
            "Tactical Locked Orbit",
            ControlV2Profile.Default with
            {
                SchemeId = ControlSchemeId.TacticalLockedOrbit,
                MoveSpeedForwardMultiplier = 0.95f,
                MoveSpeedBackwardMultiplier = 0.75f,
                MoveSpeedStrafeMultiplier = 0.88f,
                TurnSpeed = 2.4f,
                RotationSmoothing = 10.0f,
                UseTankMovement = false,
                UseTankTurning = false,
                FaceMovementDirection = true,
                FaceCameraDirectionWhenMoving = false,
                FaceCameraDirectionWhenIdle = true,
                DecoupleCameraYaw = false,
                OrbitSensitivity = 0.0058f,
                OrbitSensitivityVertical = 0.0034f,
                MaxOrbitOffsetYaw = 0.55f,
                CameraDistance = 6.8f,
                CameraHeight = 5.4f,
                CameraSmoothing = 8.5f,
                OrbitRecenteringEnabled = true,
                OrbitRecenteringMovingSpeed = 14.0f,
                OrbitRecenteringIdleSpeed = 9.5f,
                Deadzone = 0.025f,
                NormalizeDiagonalMovement = true,
            },
            IsCustom: false),
    ];

    public static ControlProfilePreset? TryGetBuiltIn(string id)
    {
        return BuiltInPresets.FirstOrDefault(p => p.Id.Equals(id, StringComparison.Ordinal));
    }

    public static ControlV2Profile GetBuiltInProfileOrDefault(string id)
    {
        return TryGetBuiltIn(id)?.Profile ?? ControlV2Profile.Default;
    }

    public static ControlV2Profile Normalize(ControlV2Profile profile)
    {
        return profile with
        {
            MoveSpeedForwardMultiplier = Math.Clamp(profile.MoveSpeedForwardMultiplier, 0.25f, 2.50f),
            MoveSpeedBackwardMultiplier = Math.Clamp(profile.MoveSpeedBackwardMultiplier, 0.10f, 2.50f),
            MoveSpeedStrafeMultiplier = Math.Clamp(profile.MoveSpeedStrafeMultiplier, 0.00f, 2.50f),
            TurnSpeed = Math.Clamp(profile.TurnSpeed, 0.50f, 8.00f),
            RotationSmoothing = Math.Clamp(profile.RotationSmoothing, 1.00f, 30.00f),
            MousePlayerTurnSensitivity = Math.Clamp(profile.MousePlayerTurnSensitivity, 0.0010f, 0.2500f),
            MousePlayerMoveForwardSensitivity = Math.Clamp(profile.MousePlayerMoveForwardSensitivity, 0.0010f, 0.2500f),
            MousePlayerMoveLateralSensitivity = Math.Clamp(profile.MousePlayerMoveLateralSensitivity, 0.0010f, 0.2500f),
            OrbitSensitivity = Math.Clamp(profile.OrbitSensitivity, 0.0010f, 0.0500f),
            OrbitSensitivityVertical = Math.Clamp(profile.OrbitSensitivityVertical, 0.0010f, 0.0500f),
            MaxOrbitOffsetYaw = Math.Clamp(profile.MaxOrbitOffsetYaw, 0.05f, MathF.PI),
            MaxOrbitVerticalOffset = Math.Clamp(profile.MaxOrbitVerticalOffset, 0.10f, 6.00f),
            CameraDistance = Math.Clamp(profile.CameraDistance, 3.00f, 14.00f),
            CameraHeight = Math.Clamp(profile.CameraHeight, 1.50f, 9.00f),
            CameraSmoothing = Math.Clamp(profile.CameraSmoothing, 1.00f, 20.00f),
            OrbitRecenteringMovingSpeed = Math.Clamp(profile.OrbitRecenteringMovingSpeed, 0.00f, 25.00f),
            OrbitRecenteringIdleSpeed = Math.Clamp(profile.OrbitRecenteringIdleSpeed, 0.00f, 25.00f),
            Deadzone = Math.Clamp(profile.Deadzone, 0.00f, 0.25f),
        };
    }
}
