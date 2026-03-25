using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Runtime.Control;

public static class ControlV2Pipeline
{
    public static float ComputeFacingYaw(Vec3 facing)
    {
        var f = facing.LengthSquared() > 0.0001f ? facing.Normalized() : new Vec3(0f, 0f, -1f);
        return MathF.Atan2(f.X, -f.Z);
    }

    public static Vec3 ResolveMoveDirection(
        ControllerState state,
        ControlV2Profile profile,
        Vec3 currentFacing,
        float cameraYaw,
        bool moveForward,
        bool moveBackward,
        bool moveLeft,
        bool moveRight,
        bool blockedByOverlay,
        float dt,
        float additionalForwardInput = 0f,
        float additionalLateralInput = 0f)
    {
        if (blockedByOverlay || !profile.EnablePlayerMovement)
            return Vec3.Zero;

        _ = dt;

        var keyboardForwardInput = profile.EnableKeyboardMovement
            ? (moveForward ? 1f : 0f) - (moveBackward ? 1f : 0f)
            : 0f;
        var keyboardLateralInput = profile.EnableKeyboardMovement
            ? (moveRight ? 1f : 0f) - (moveLeft ? 1f : 0f)
            : 0f;

        var forwardInput = keyboardForwardInput + additionalForwardInput;
        var lateralInput = keyboardLateralInput + additionalLateralInput;

        if (profile.InvertMoveForwardInput)
            forwardInput = -forwardInput;
        if (profile.InvertMoveLateralInput)
            lateralInput = -lateralInput;

        var minInput = MathF.Max(profile.Deadzone, 0.0001f);
        if (MathF.Abs(forwardInput) < minInput && MathF.Abs(lateralInput) < minInput)
            return Vec3.Zero;

        if (profile.NormalizeDiagonalMovement && MathF.Abs(forwardInput) > minInput && MathF.Abs(lateralInput) > minInput)
        {
            const float diagonalScale = 0.70710677f;
            forwardInput *= diagonalScale;
            lateralInput *= diagonalScale;
        }

        if (profile.UseTankMovement)
        {
            var facing = currentFacing.LengthSquared() > 0.0001f
                ? currentFacing.Normalized()
                : FacingFromYaw(state.PlayerYaw);
            var forwardScale = forwardInput >= 0f
                ? profile.MoveSpeedForwardMultiplier
                : profile.MoveSpeedBackwardMultiplier;
            return facing * (forwardInput * forwardScale);
        }

        var basisYaw = profile.UseCameraRelativeMovement ? cameraYaw : state.PlayerYaw;
        var basisForward = FacingFromYaw(basisYaw);
        var basisRight = new Vec3(-basisForward.Z, 0f, basisForward.X);

        var move = (basisForward * (forwardInput * profile.MoveSpeedForwardMultiplier))
            + (basisRight * (lateralInput * profile.MoveSpeedStrafeMultiplier));

        if (forwardInput < 0f)
        {
            move += basisForward * (forwardInput * (profile.MoveSpeedBackwardMultiplier - profile.MoveSpeedForwardMultiplier));
        }

        return move.LengthSquared() > 0.0001f ? move : Vec3.Zero;
    }

    public static Vec3 UpdateFacingFromInput(
        ControllerState state,
        ControlV2Profile profile,
        Vec3 currentFacing,
        Vec3 moveDirection,
        bool turnLeft,
        bool turnRight,
        float cameraYaw,
        float dt,
        bool blockedByOverlay,
        float additionalTurnInput = 0f)
    {
        var facing = currentFacing.LengthSquared() > 0.0001f
            ? currentFacing.Normalized()
            : FacingFromYaw(state.PlayerYaw);

        if (blockedByOverlay || !profile.EnablePlayerTurning)
            return facing;

        var keyboardTurnInput = profile.EnableKeyboardTurning
            ? (turnRight ? 1f : 0f) - (turnLeft ? 1f : 0f)
            : 0f;
        var turnInput = keyboardTurnInput + additionalTurnInput;
        if (profile.InvertTurnInput)
            turnInput = -turnInput;

        if (profile.UseTankTurning)
        {
            if (MathF.Abs(turnInput) > MathF.Max(profile.Deadzone, 0.0001f))
            {
                var yaw = NormalizeAngle(state.PlayerYaw + (turnInput * profile.TurnSpeed * dt));
                state.PlayerYaw = yaw;
                return FacingFromYaw(yaw);
            }

            return facing;
        }

        if (MathF.Abs(turnInput) > MathF.Max(profile.Deadzone, 0.0001f))
        {
            var yaw = NormalizeAngle(state.PlayerYaw + (turnInput * profile.TurnSpeed * dt));
            state.PlayerYaw = yaw;
            var desiredFromTurn = FacingFromYaw(yaw);
            var turnSmoothing = Math.Clamp(dt * MathF.Max(profile.RotationSmoothing, 0.0001f), 0f, 1f);
            return LerpVector(facing, desiredFromTurn, turnSmoothing).Normalized();
        }

        var hasMovement = moveDirection.LengthSquared() > 0.0001f;
        Vec3 desiredFacing;

        if (hasMovement && profile.FaceCameraDirectionWhenMoving)
        {
            desiredFacing = FacingFromYaw(cameraYaw);
        }
        else if (hasMovement && profile.FaceMovementDirection)
        {
            desiredFacing = moveDirection.Normalized();
        }
        else if (!hasMovement && profile.FaceCameraDirectionWhenIdle)
        {
            desiredFacing = FacingFromYaw(cameraYaw);
        }
        else
        {
            desiredFacing = facing;
        }

        var smoothing = Math.Clamp(dt * MathF.Max(profile.RotationSmoothing, 0.0001f), 0f, 1f);
        facing = LerpVector(facing, desiredFacing, smoothing).Normalized();
        state.PlayerYaw = NormalizeAngle(ComputeFacingYaw(facing));
        return facing;
    }

    public static void UpdateFollowCameraYaw(
        ControllerState state,
        Vec3 facing,
        bool movementIntent,
        float dt,
        ControlV2Profile profile,
        bool playerTurnIntent = false)
    {
        var desiredYaw = ComputeFacingYaw(facing);
        state.PlayerYaw = NormalizeAngle(desiredYaw);

        if (!state.CameraYawInitialized)
        {
            state.CameraYawSmoothed = state.PlayerYaw;
            state.CameraYawInitialized = true;
            return;
        }

        if (profile.EnableFreeCameraMode || !profile.EnableCameraFollow)
            return;

        var followTargetYaw = state.PlayerYaw;
        if (!profile.EnableCameraAutoRotateWithPlayerTurn && !movementIntent && playerTurnIntent)
            followTargetYaw = state.CameraYawSmoothed;
        if (profile.DecoupleCameraYaw && !movementIntent && !profile.FaceCameraDirectionWhenIdle)
            followTargetYaw = state.CameraYawSmoothed;

        var followSpeed = state.IsOrbitModifierActive
            ? profile.CameraSmoothing * 0.35f
            : movementIntent
                ? profile.CameraSmoothing
                : profile.CameraSmoothing * 0.7f;

        state.CameraYawSmoothed = NormalizeAngle(LerpAngle(state.CameraYawSmoothed, followTargetYaw, dt * followSpeed));

        if (!state.IsOrbitModifierActive && profile.OrbitRecenteringEnabled)
        {
            var orbitRecenteringSpeed = movementIntent
                ? profile.OrbitRecenteringMovingSpeed
                : profile.OrbitRecenteringIdleSpeed;
            state.OrbitOffsetYaw = Lerp(state.OrbitOffsetYaw, 0f, dt * orbitRecenteringSpeed);
            state.OrbitVerticalOffset = Lerp(state.OrbitVerticalOffset, 0f, dt * orbitRecenteringSpeed);
        }
    }

    public static void ApplyOrbitDelta(ControllerState state, float rawMouseDeltaX, float rawMouseDeltaY, ControlV2Profile profile)
    {
        if (!profile.EnableCameraOrbitInput || profile.EnableFreeCameraMode && profile.CameraOrbitButton == MouseControlButton.Off)
            return;

        if (MathF.Abs(rawMouseDeltaX) <= 0.0001f && MathF.Abs(rawMouseDeltaY) <= 0.0001f)
            return;

        var horizontalDir = profile.InvertOrbitHorizontal ? 1f : -1f;
        var horizontalDelta = rawMouseDeltaX * profile.OrbitSensitivity * horizontalDir;
        var verticalDir = profile.InvertOrbitVertical ? 1f : -1f;
        var verticalDelta = rawMouseDeltaY * profile.OrbitSensitivityVertical * verticalDir;

        var maxYawOffset = Math.Clamp(profile.MaxOrbitOffsetYaw, 0.05f, MathF.PI);
        var maxVerticalOffset = Math.Clamp(profile.MaxOrbitVerticalOffset, 0.10f, 6.00f);
        state.OrbitOffsetYaw = Math.Clamp(state.OrbitOffsetYaw + horizontalDelta, -maxYawOffset, maxYawOffset);
        state.OrbitVerticalOffset = Math.Clamp(state.OrbitVerticalOffset + verticalDelta, -maxVerticalOffset, maxVerticalOffset);
    }

    public static float NormalizeAngle(float angle)
    {
        while (angle > MathF.PI)
            angle -= MathF.PI * 2f;
        while (angle < -MathF.PI)
            angle += MathF.PI * 2f;
        return angle;
    }

    private static float LerpAngle(float from, float to, float t)
    {
        var delta = NormalizeAngle(to - from);
        return from + delta * Math.Clamp(t, 0f, 1f);
    }

    private static float Lerp(float from, float to, float t)
    {
        var k = Math.Clamp(t, 0f, 1f);
        return from + (to - from) * k;
    }

    private static Vec3 LerpVector(Vec3 from, Vec3 to, float t)
    {
        return new Vec3(
            Lerp(from.X, to.X, t),
            Lerp(from.Y, to.Y, t),
            Lerp(from.Z, to.Z, t));
    }

    public static Vec3 FacingFromYaw(float yaw)
    {
        var normalizedYaw = NormalizeAngle(yaw);
        return new Vec3(MathF.Sin(normalizedYaw), 0f, -MathF.Cos(normalizedYaw));
    }
}
