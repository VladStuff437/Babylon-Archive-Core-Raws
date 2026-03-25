using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Runtime.Control;
using System.Text.Json;

namespace BabylonArchiveCore.Tests;

public sealed class ControlV2PipelineTests
{
    [Fact]
    public void ResolveMoveDirection_ForwardUsesCameraYaw()
    {
        var state = new ControllerState();
        var profile = ControlV2Profile.Default with
        {
            UseTankMovement = false,
            MoveSpeedForwardMultiplier = 1.0f,
            MoveSpeedStrafeMultiplier = 1.0f,
        };

        var move = ControlV2Pipeline.ResolveMoveDirection(
            state,
            profile,
            currentFacing: new Vec3(0f, 0f, -1f),
            cameraYaw: MathF.PI * 0.5f,
            moveForward: true,
            moveBackward: false,
            moveLeft: false,
            moveRight: false,
            blockedByOverlay: false,
            dt: 0.016f);

        Assert.True(move.X > 0.99f);
        Assert.True(MathF.Abs(move.Z) < 0.01f);
    }

    [Fact]
    public void ResolveMoveDirection_StrafeUsesCameraRightAxis()
    {
        var state = new ControllerState();
        var profile = ControlV2Profile.Default with
        {
            UseTankMovement = false,
            MoveSpeedForwardMultiplier = 1.0f,
            MoveSpeedStrafeMultiplier = 1.0f,
        };

        var left = ControlV2Pipeline.ResolveMoveDirection(
            state,
            profile,
            currentFacing: new Vec3(0f, 0f, -1f),
            cameraYaw: 0f,
            moveForward: false,
            moveBackward: false,
            moveLeft: true,
            moveRight: false,
            blockedByOverlay: false,
            dt: 0.016f);

        var right = ControlV2Pipeline.ResolveMoveDirection(
            state,
            profile,
            currentFacing: new Vec3(0f, 0f, -1f),
            cameraYaw: 0f,
            moveForward: false,
            moveBackward: false,
            moveLeft: false,
            moveRight: true,
            blockedByOverlay: false,
            dt: 0.016f);

        Assert.True(left.X < -0.99f);
        Assert.True(right.X > 0.99f);
    }

    [Fact]
    public void ResolveMoveDirection_BlockedByOverlay_IsZero()
    {
        var state = new ControllerState();
        var profile = ControlV2Profile.Default;

        var move = ControlV2Pipeline.ResolveMoveDirection(
            state,
            profile,
            currentFacing: new Vec3(0f, 0f, -1f),
            cameraYaw: 0f,
            moveForward: true,
            moveBackward: false,
            moveLeft: false,
            moveRight: false,
            blockedByOverlay: true,
            dt: 0.016f);

        Assert.Equal(Vec3.Zero, move);
    }

    [Fact]
    public void UpdateFacingFromInput_UpdatesFacingAndYawFromMovement()
    {
        var state = new ControllerState
        {
            PlayerYaw = 0f,
        };

        var profile = ControlV2Profile.Default with
        {
            UseTankTurning = false,
            FaceMovementDirection = true,
            FaceCameraDirectionWhenMoving = false,
            FaceCameraDirectionWhenIdle = false,
        };

        var facing = ControlV2Pipeline.UpdateFacingFromInput(
            state,
            profile,
            currentFacing: new Vec3(0f, 0f, -1f),
            moveDirection: new Vec3(1f, 0f, 0f),
            turnLeft: false,
            turnRight: false,
            cameraYaw: 0f,
            dt: 0.1f,
            blockedByOverlay: false);

        Assert.True(facing.X > 0.99f);
        Assert.InRange(state.PlayerYaw, 1.55f, 1.59f);
    }

    [Fact]
    public void UpdateFacingFromInput_IdleKeepsCurrentFacing()
    {
        var state = new ControllerState();
        var facing = new Vec3(0f, 0f, -1f);
        var profile = ControlV2Profile.Default with
        {
            UseTankTurning = false,
            FaceMovementDirection = true,
            FaceCameraDirectionWhenMoving = false,
            FaceCameraDirectionWhenIdle = false,
        };

        state.ResetFromFacing(facing);

        var updated = ControlV2Pipeline.UpdateFacingFromInput(
            state,
            profile,
            currentFacing: facing,
            moveDirection: Vec3.Zero,
            turnLeft: false,
            turnRight: false,
            cameraYaw: 0f,
            dt: 0.1f,
            blockedByOverlay: false);

        Assert.Equal(facing, updated);
    }

    [Fact]
    public void UpdateFacingFromInput_TankTurning_ReversedADDirection()
    {
        var state = new ControllerState();
        state.ResetFromFacing(new Vec3(0f, 0f, -1f));

        var profile = ControlV2Profile.Default with
        {
            UseTankTurning = true,
            TurnSpeed = 2.0f,
            Deadzone = 0.0f,
        };

        var updated = ControlV2Pipeline.UpdateFacingFromInput(
            state,
            profile,
            currentFacing: new Vec3(0f, 0f, -1f),
            moveDirection: Vec3.Zero,
            turnLeft: true,
            turnRight: false,
            cameraYaw: 0f,
            dt: 0.5f,
            blockedByOverlay: false);

        Assert.True(updated.X < -0.7f);
        Assert.InRange(state.PlayerYaw, -1.05f, -0.95f);
    }

    [Fact]
    public void FollowCameraYaw_InitializesFromFacing()
    {
        var state = new ControllerState();
        var profile = ControlV2Profile.Default;

        ControlV2Pipeline.UpdateFollowCameraYaw(state, new Vec3(0f, 0f, -1f), movementIntent: false, dt: 0.016f, profile);

        Assert.True(state.CameraYawInitialized);
        Assert.Equal(0f, state.CameraYawSmoothed, 3);
    }

    [Fact]
    public void FollowCameraYaw_SmoothlyApproachesFacingYaw()
    {
        var state = new ControllerState
        {
            CameraYawInitialized = true,
            CameraYawSmoothed = 0f,
        };

        var profile = ControlV2Profile.Default;
        var facing = new Vec3(1f, 0f, 0f);

        ControlV2Pipeline.UpdateFollowCameraYaw(state, facing, movementIntent: true, dt: 0.05f, profile);

        var desiredYaw = ControlV2Pipeline.ComputeFacingYaw(facing);
        Assert.True(state.CameraYawSmoothed > 0f);
        Assert.True(state.CameraYawSmoothed < desiredYaw);
    }

    [Fact]
    public void FollowCameraYaw_RecentersOrbitWhenNotOrbiting()
    {
        var state = new ControllerState
        {
            CameraYawInitialized = true,
            CameraYawSmoothed = 0f,
            OrbitOffsetYaw = 0.8f,
            OrbitVerticalOffset = 0.6f,
            IsOrbitModifierActive = false,
        };

        ControlV2Pipeline.UpdateFollowCameraYaw(
            state,
            facing: new Vec3(0f, 0f, -1f),
            movementIntent: true,
            dt: 0.1f,
            profile: ControlV2Profile.Default);

        Assert.True(state.OrbitOffsetYaw < 0.8f);
        Assert.True(state.OrbitVerticalOffset < 0.6f);
    }

    [Fact]
    public void FollowCameraYaw_DoesNotRecenterOrbitWhileOrbiting()
    {
        var state = new ControllerState
        {
            CameraYawInitialized = true,
            CameraYawSmoothed = 0f,
            OrbitOffsetYaw = 0.8f,
            OrbitVerticalOffset = 0.6f,
            IsOrbitModifierActive = true,
        };

        ControlV2Pipeline.UpdateFollowCameraYaw(
            state,
            facing: new Vec3(0f, 0f, -1f),
            movementIntent: true,
            dt: 0.1f,
            profile: ControlV2Profile.Default);

        Assert.Equal(0.8f, state.OrbitOffsetYaw, 3);
        Assert.Equal(0.6f, state.OrbitVerticalOffset, 3);
    }

    [Fact]
    public void OrbitDelta_UsesSensitivityAndDefaultDirection()
    {
        var state = new ControllerState();
        var profile = ControlV2Profile.Default with
        {
            OrbitSensitivity = 0.01f,
            OrbitSensitivityVertical = 0.01f,
            InvertOrbitHorizontal = false,
            InvertOrbitVertical = false,
        };

        ControlV2Pipeline.ApplyOrbitDelta(state, rawMouseDeltaX: 10f, rawMouseDeltaY: 5f, profile);

        Assert.Equal(-0.10f, state.OrbitOffsetYaw, 3);
        Assert.Equal(-0.05f, state.OrbitVerticalOffset, 3);
    }

    [Fact]
    public void OrbitDelta_RespectsInvertFlag()
    {
        var state = new ControllerState();
        var profile = ControlV2Profile.Default with
        {
            OrbitSensitivity = 0.01f,
            OrbitSensitivityVertical = 0.01f,
            InvertOrbitHorizontal = true,
            InvertOrbitVertical = true,
        };

        ControlV2Pipeline.ApplyOrbitDelta(state, rawMouseDeltaX: 10f, rawMouseDeltaY: 5f, profile);

        Assert.Equal(0.10f, state.OrbitOffsetYaw, 3);
        Assert.Equal(0.05f, state.OrbitVerticalOffset, 3);
    }

    [Fact]
    public void OrbitDelta_ClampsToConfiguredLimit()
    {
        var state = new ControllerState
        {
            OrbitOffsetYaw = 0.35f,
            OrbitVerticalOffset = 0.45f,
        };

        var profile = ControlV2Profile.Default with
        {
            OrbitSensitivity = 0.02f,
            OrbitSensitivityVertical = 0.02f,
            InvertOrbitHorizontal = true,
            InvertOrbitVertical = true,
            MaxOrbitOffsetYaw = 0.40f,
            MaxOrbitVerticalOffset = 0.50f,
        };

        ControlV2Pipeline.ApplyOrbitDelta(state, rawMouseDeltaX: 10f, rawMouseDeltaY: 10f, profile);

        Assert.Equal(0.40f, state.OrbitOffsetYaw, 3);
        Assert.Equal(0.50f, state.OrbitVerticalOffset, 3);
    }

    [Fact]
    public void ControlProfile_RoundTrip_PreservesExpandedFields()
    {
        var profile = ControlV2Profile.Default with
        {
            SchemeId = ControlSchemeId.ActionStrafe,
            InvertOrbitHorizontal = true,
            InvertOrbitVertical = true,
            MaxOrbitOffsetYaw = 0.75f,
            MaxOrbitVerticalOffset = 1.25f,
            OrbitSensitivity = 0.009f,
            OrbitSensitivityVertical = 0.006f,
            CameraSmoothing = 8.5f,
            Deadzone = 0.03f,
            DecoupleCameraYaw = true,
        };

        var json = JsonSerializer.Serialize(profile);
        var restored = JsonSerializer.Deserialize<ControlV2Profile>(json);

        Assert.NotNull(restored);
        Assert.Equal(profile, restored);
    }
}
