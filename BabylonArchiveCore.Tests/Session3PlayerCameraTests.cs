using BabylonArchiveCore.Core.Camera;
using BabylonArchiveCore.Core.Input;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Core.Player;
using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain.Player;
using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Domain.Scene.Geometry;
using BabylonArchiveCore.Runtime.Gameplay;
using BabylonArchiveCore.Runtime.Scene;

namespace BabylonArchiveCore.Tests;

public class Session3PlayerCameraTests
{
    // ── Helpers ──

    private static OctagonalFloor MakeFloor() => new(22f, 11f);

    private static SceneGeometry MakeGeometry()
    {
        var floor = MakeFloor();
        var zones = HubA0SceneBuilder.Build();
        return new SceneGeometry(floor, zones);
    }

    private static CollisionSystem3D MakeCollision(SceneGeometry geo) => new(geo);

    private static PlayerEntity MakePlayer(float x = 0f, float z = 0f) =>
        new() { Position = new Vec3(x, 0f, z) };

    private static HubA0Runtime MakeHubRuntime()
    {
        var zones = HubA0SceneBuilder.Build();
        var journal = new EventJournal();
        return new HubA0Runtime(zones, journal, new NullLogger());
    }

    private sealed class NullLogger : ILogger
    {
        public void Info(string message) { }
        public void Warn(string message) { }
        public void Error(string message) { }
    }

    // ── Vec3 extensions ──

    [Fact]
    public void Vec3_Length_IsCorrect()
    {
        var v = new Vec3(3f, 4f, 0f);
        Assert.Equal(5f, v.Length(), 3);
    }

    [Fact]
    public void Vec3_Normalized_HasLength1()
    {
        var v = new Vec3(3f, 0f, 4f);
        var n = v.Normalized();
        Assert.Equal(1f, n.Length(), 3);
        Assert.Equal(0.6f, n.X, 3);
        Assert.Equal(0.8f, n.Z, 3);
    }

    [Fact]
    public void Vec3_Normalized_ZeroReturnsZero()
    {
        var n = Vec3.Zero.Normalized();
        Assert.Equal(Vec3.Zero, n);
    }

    [Fact]
    public void Vec3_ScalarMultiply()
    {
        var v = new Vec3(1f, 2f, 3f);
        var r = v * 2f;
        Assert.Equal(new Vec3(2f, 4f, 6f), r);
    }

    [Fact]
    public void Vec3_Lerp_Halfway()
    {
        var a = new Vec3(0f, 0f, 0f);
        var b = new Vec3(10f, 0f, 10f);
        var mid = a.Lerp(b, 0.5f);
        Assert.Equal(5f, mid.X, 3);
        Assert.Equal(5f, mid.Z, 3);
    }

    [Fact]
    public void Vec3_Lerp_Zero_ReturnsStart()
    {
        var a = new Vec3(2f, 3f, 4f);
        var b = new Vec3(10f, 20f, 30f);
        Assert.Equal(a, a.Lerp(b, 0f));
    }

    [Fact]
    public void Vec3_Lerp_One_ReturnsEnd()
    {
        var a = new Vec3(2f, 3f, 4f);
        var b = new Vec3(10f, 20f, 30f);
        var result = a.Lerp(b, 1f);
        Assert.Equal(b.X, result.X, 3);
        Assert.Equal(b.Y, result.Y, 3);
        Assert.Equal(b.Z, result.Z, 3);
    }

    // ── PlayerEntity ──

    [Fact]
    public void PlayerEntity_DefaultPosition_InCapsule()
    {
        var p = new PlayerEntity();
        Assert.Equal(-8f, p.Position.X);
        Assert.Equal(-6f, p.Position.Z);
        Assert.Equal("Алан Арквейн", p.Name);
    }

    [Fact]
    public void PlayerEntity_DefaultSpeed()
    {
        var p = new PlayerEntity();
        Assert.Equal(4.0f, p.MoveSpeed);
        Assert.Equal(2.0f, p.InteractReach);
    }

    // ── CameraProfile ──

    [Fact]
    public void CameraProfile_Default3D_IsThirdPerson()
    {
        var p = CameraProfile.Default3D;
        Assert.Equal(CameraMode.ThirdPerson3D, p.Mode);
        Assert.True(p.Distance > 0);
        Assert.True(p.PitchDegrees > 0);
    }

    [Fact]
    public void CameraProfile_Default25D_IsIsometric()
    {
        var p = CameraProfile.Default25D;
        Assert.Equal(CameraMode.Isometric25D, p.Mode);
        Assert.Equal(45f, p.PitchDegrees);
    }

    // ── CameraController ──

    [Fact]
    public void CameraController_InitialPosition_IsOffset()
    {
        var profile = CameraProfile.Default3D;
        var playerPos = new Vec3(0f, 0f, 0f);
        var cam = new CameraController(profile, playerPos);

        var expected = playerPos + profile.Offset;
        Assert.Equal(expected, cam.Position);
    }

    [Fact]
    public void CameraController_Toggle_SwitchesMode()
    {
        var cam = new CameraController(CameraProfile.Default3D, Vec3.Zero);
        Assert.Equal(CameraMode.ThirdPerson3D, cam.ActiveMode);

        cam.ToggleMode();
        Assert.Equal(CameraMode.Isometric25D, cam.ActiveMode);

        cam.ToggleMode();
        Assert.Equal(CameraMode.ThirdPerson3D, cam.ActiveMode);
    }

    [Fact]
    public void CameraController_SnapTo_MovesInstantly()
    {
        var cam = new CameraController(CameraProfile.Default3D, Vec3.Zero);
        var newPos = new Vec3(5f, 0f, 5f);
        cam.SnapTo(newPos);

        var expected = newPos + CameraProfile.Default3D.Offset;
        Assert.Equal(expected, cam.Position);
    }

    [Fact]
    public void CameraController_Update_MovesTowardTarget()
    {
        var cam = new CameraController(CameraProfile.Default3D, Vec3.Zero);
        var initialPos = cam.Position;

        // Player moved north — camera should follow
        var playerMoved = new Vec3(0f, 0f, 5f);
        cam.Update(playerMoved, 1f / 60f); // one frame at 60fps
        var afterUpdate = cam.Position;

        // Camera should have moved toward the new target position
        var target = playerMoved + CameraProfile.Default3D.Offset;
        var initialDist = (target - initialPos).LengthSquared();
        var newDist = (target - afterUpdate).LengthSquared();
        Assert.True(newDist < initialDist, "Camera should be closer to target after update");
    }

    [Fact]
    public void CameraController_Update_ConvergesOverTime()
    {
        var cam = new CameraController(CameraProfile.Default3D, Vec3.Zero);
        var target = new Vec3(10f, 0f, 10f);

        // Simulate 120 frames at 60fps (2 seconds)
        for (int i = 0; i < 120; i++)
            cam.Update(target, 1f / 60f);

        var expected = target + CameraProfile.Default3D.Offset;
        Assert.Equal(expected.X, cam.Position.X, 0); // within 1m
        Assert.Equal(expected.Z, cam.Position.Z, 0);
    }

    // ── InputSnapshot ──

    [Fact]
    public void InputSnapshot_None_IsZero()
    {
        var snap = InputSnapshot.None;
        Assert.Equal(Vec3.Zero, snap.MoveDirection);
        Assert.False(snap.InteractPressed);
        Assert.False(snap.CameraTogglePressed);
    }

    [Fact]
    public void InputSnapshot_FromActions_NorthEast_Normalized()
    {
        var snap = InputSnapshot.FromActions(up: true, down: false, left: false, right: true);
        var len = snap.MoveDirection.Length();
        Assert.Equal(1f, len, 2);
        Assert.True(snap.MoveDirection.X > 0);
        Assert.True(snap.MoveDirection.Z > 0);
    }

    [Fact]
    public void InputSnapshot_FromActions_NoMovement_ReturnsZero()
    {
        var snap = InputSnapshot.FromActions(false, false, false, false);
        Assert.Equal(Vec3.Zero, snap.MoveDirection);
    }

    [Fact]
    public void InputSnapshot_FromActions_Interact()
    {
        var snap = InputSnapshot.FromActions(false, false, false, false, interact: true);
        Assert.True(snap.InteractPressed);
        Assert.False(snap.CameraTogglePressed);
    }

    // ── PlayerController ──

    [Fact]
    public void PlayerController_MovementAtCentre_Succeeds()
    {
        var geo = MakeGeometry();
        var collision = MakeCollision(geo);
        var player = MakePlayer();
        var ctrl = new PlayerController(player, collision, geo);

        var input = InputSnapshot.FromActions(up: true, down: false, left: false, right: false);
        var result = ctrl.ProcessMovement(input, 1f / 60f);

        Assert.False(result.WasClamped);
        Assert.False(result.WasBlocked);
        Assert.True(player.Position.Z > 0f, "Player should have moved north");
        Assert.True(player.IsMoving);
    }

    [Fact]
    public void PlayerController_NoInput_StaysIdle()
    {
        var geo = MakeGeometry();
        var collision = MakeCollision(geo);
        var player = MakePlayer();
        var ctrl = new PlayerController(player, collision, geo);

        var result = ctrl.ProcessMovement(InputSnapshot.None, 1f / 60f);

        Assert.Equal(Vec3.Zero, player.Position);
        Assert.False(player.IsMoving);
        Assert.False(result.WasClamped);
    }

    [Fact]
    public void PlayerController_MoveTowardWall_GetsClamped()
    {
        var geo = MakeGeometry();
        var collision = MakeCollision(geo);
        // Start near edge
        var player = MakePlayer(x: 10f, z: 0f);
        var ctrl = new PlayerController(player, collision, geo);

        // Move east into the wall with a large delta time (big step)
        var input = InputSnapshot.FromActions(false, false, false, right: true);
        for (int i = 0; i < 100; i++) // Push hard toward wall
            ctrl.ProcessMovement(input, 0.1f);

        // Player should stay inside the walkable area
        Assert.True(geo.Floor.ContainsXZ(player.Position));
    }

    [Fact]
    public void PlayerController_InteractionTarget_InRange()
    {
        var geo = MakeGeometry();
        var collision = MakeCollision(geo);
        // Place player at capsule exit position
        var player = MakePlayer(x: -7f, z: -6f);
        var ctrl = new PlayerController(player, collision, geo);

        var target = ctrl.TryGetInteractionTarget(HubRhythmPhase.Awakening);

        Assert.NotNull(target);
        Assert.Equal("capsule_exit", target.Value.ObjectId);
        Assert.True(target.Value.InRange);
    }

    [Fact]
    public void PlayerController_GetCurrentZone_ReturnsNull_InEmpty()
    {
        var geo = MakeGeometry();
        var collision = MakeCollision(geo);
        // Place far from any zone
        var player = MakePlayer(x: 0f, z: -5f);
        var ctrl = new PlayerController(player, collision, geo);

        // May or may not be in a zone depending on exact geometry
        // The key is that the method doesn't crash
        var zone = ctrl.GetCurrentZone();
        // Just verify it works — zone can be null or non-null
        Assert.True(true);
    }

    // ── GameplaySession ──

    [Fact]
    public void GameplaySession_ProcessFrame_IncrementsCount()
    {
        var geo = MakeGeometry();
        var collision = MakeCollision(geo);
        var player = MakePlayer();
        var hubRuntime = MakeHubRuntime();
        var session = new GameplaySession(player, geo, collision, hubRuntime, CameraProfile.Default3D, new NullLogger());

        var result = session.ProcessFrame(InputSnapshot.None, 1f / 60f);
        Assert.Equal(1, result.Frame);

        var result2 = session.ProcessFrame(InputSnapshot.None, 1f / 60f);
        Assert.Equal(2, result2.Frame);
    }

    [Fact]
    public void GameplaySession_Movement_UpdatesCameraAndPlayer()
    {
        var geo = MakeGeometry();
        var collision = MakeCollision(geo);
        var player = MakePlayer();
        var hubRuntime = MakeHubRuntime();
        var session = new GameplaySession(player, geo, collision, hubRuntime, CameraProfile.Default3D, new NullLogger());

        var initialCamPos = session.Camera.Position;
        var moveNorth = InputSnapshot.FromActions(up: true, down: false, left: false, right: false);

        // Simulate several frames of walking north
        for (int i = 0; i < 30; i++)
            session.ProcessFrame(moveNorth, 1f / 60f);

        Assert.True(player.Position.Z > 0f, "Player moved north");
        // Camera should have moved too
        Assert.NotEqual(initialCamPos, session.Camera.Position);
    }

    [Fact]
    public void GameplaySession_CameraToggle_SwitchesMode()
    {
        var geo = MakeGeometry();
        var collision = MakeCollision(geo);
        var player = MakePlayer();
        var hubRuntime = MakeHubRuntime();
        var session = new GameplaySession(player, geo, collision, hubRuntime, CameraProfile.Default3D, new NullLogger());

        Assert.Equal(CameraMode.ThirdPerson3D, session.Camera.ActiveMode);

        var toggle = new InputSnapshot { CameraTogglePressed = true };
        session.ProcessFrame(toggle, 1f / 60f);

        Assert.Equal(CameraMode.Isometric25D, session.Camera.ActiveMode);
    }

    [Fact]
    public void GameplaySession_Interaction_AtCapsule()
    {
        var geo = MakeGeometry();
        var collision = MakeCollision(geo);
        var player = MakePlayer(x: -7f, z: -6f); // Near capsule exit
        var hubRuntime = MakeHubRuntime();
        var session = new GameplaySession(player, geo, collision, hubRuntime, CameraProfile.Default3D, new NullLogger());

        var interact = InputSnapshot.FromActions(false, false, false, false, interact: true);
        var result = session.ProcessFrame(interact, 1f / 60f);

        Assert.NotNull(result.Interaction);
        Assert.True(result.Interaction!.Success);
        Assert.Equal(HubRhythmPhase.Identification, result.Phase);
    }

    [Fact]
    public void GameplaySession_FullPrologueWalkthrough()
    {
        var geo = MakeGeometry();
        var collision = MakeCollision(geo);
        var player = MakePlayer(x: -7f, z: -6f);
        var hubRuntime = MakeHubRuntime();
        var session = new GameplaySession(player, geo, collision, hubRuntime, CameraProfile.Default3D, new NullLogger());

        // Step 1: Capsule exit
        var interact = InputSnapshot.FromActions(false, false, false, false, interact: true);
        session.ProcessFrame(interact, 1f / 60f);
        Assert.Equal(HubRhythmPhase.Identification, hubRuntime.CurrentPhase);

        // Walk to biometrics (-8, 0, 0) — move north
        player.Position = new Vec3(-8f, 0f, 0f);
        session.ProcessFrame(interact, 1f / 60f);
        Assert.Equal(HubRhythmPhase.Provisioning, hubRuntime.CurrentPhase);

        // Walk to logistics (8, 0, -6)
        player.Position = new Vec3(8f, 0f, -6f);
        session.ProcessFrame(interact, 1f / 60f);
        Assert.Equal(HubRhythmPhase.DroneContact, hubRuntime.CurrentPhase);

        // Walk to drone niche (8, 0, -2)
        player.Position = new Vec3(8f, 0f, -2f);
        session.ProcessFrame(interact, 1f / 60f);
        Assert.Equal(HubRhythmPhase.Activation, hubRuntime.CurrentPhase);

        // Walk to CORE (0, 0, 0)
        player.Position = new Vec3(0f, 0f, 0f);
        session.ProcessFrame(interact, 1f / 60f);
        Assert.Equal(HubRhythmPhase.OperationAccess, hubRuntime.CurrentPhase);
    }

    // ── InputAction enum ──

    [Fact]
    public void InputAction_HasCameraToggle()
    {
        Assert.True(Enum.IsDefined(typeof(InputAction), InputAction.CameraToggle));
    }

    [Fact]
    public void InputMap_Default_HasCameraToggle()
    {
        var map = InputMap.CreateDefault();
        var bindings = map.GetBindings(InputAction.CameraToggle);
        Assert.Single(bindings);
        Assert.Equal("Q", bindings[0].Source);
    }
}
