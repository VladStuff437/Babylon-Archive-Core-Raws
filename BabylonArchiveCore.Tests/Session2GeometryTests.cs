using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Domain.Scene.Geometry;
using BabylonArchiveCore.Runtime.Scene;

namespace BabylonArchiveCore.Tests;

public class Session2GeometryTests
{
    // ── OctagonalFloor ──

    [Fact]
    public void OctagonalFloor_Has8Vertices()
    {
        var floor = new OctagonalFloor(22f, 11f);
        Assert.Equal(8, floor.Vertices.Length);
    }

    [Fact]
    public void OctagonalFloor_Has8Walls()
    {
        var floor = new OctagonalFloor(22f, 11f);
        Assert.Equal(8, floor.Walls.Length);
    }

    [Fact]
    public void OctagonalFloor_CentreIsWalkable()
    {
        var floor = new OctagonalFloor(22f, 11f);
        Assert.True(floor.ContainsXZ(0f, 0f));
        Assert.True(floor.IsWalkable(new Vec3(0f, 0f, 0f)));
    }

    [Fact]
    public void OctagonalFloor_OutsideIsNotWalkable()
    {
        var floor = new OctagonalFloor(22f, 11f);
        // Well outside the 22×22 bounding square
        Assert.False(floor.ContainsXZ(20f, 20f));
        Assert.False(floor.IsWalkable(new Vec3(20f, 0f, 20f)));
    }

    [Fact]
    public void OctagonalFloor_CornersCutOff()
    {
        var floor = new OctagonalFloor(22f, 11f);
        // Exact corner of the bounding square should be outside the octagon
        Assert.False(floor.ContainsXZ(11f, 11f));
        Assert.False(floor.ContainsXZ(-11f, -11f));
    }

    [Fact]
    public void OctagonalFloor_EdgeMidpointsAreInside()
    {
        var floor = new OctagonalFloor(22f, 11f);
        // Midpoint of east edge (x=11, z=0) should be right on/near boundary
        // Slightly inside should be true
        Assert.True(floor.ContainsXZ(10.5f, 0f));
        Assert.True(floor.ContainsXZ(0f, 10.5f));
        Assert.True(floor.ContainsXZ(-10.5f, 0f));
        Assert.True(floor.ContainsXZ(0f, -10.5f));
    }

    [Fact]
    public void OctagonalFloor_AboveCeilingNotWalkable()
    {
        var floor = new OctagonalFloor(22f, 11f);
        Assert.False(floor.IsWalkable(new Vec3(0f, 12f, 0f)));
    }

    [Fact]
    public void OctagonalFloor_BelowFloorNotWalkable()
    {
        var floor = new OctagonalFloor(22f, 11f);
        Assert.False(floor.IsWalkable(new Vec3(0f, -1f, 0f)));
    }

    [Fact]
    public void OctagonalFloor_BoundsEnclosesFloor()
    {
        var floor = new OctagonalFloor(22f, 11f);
        var bounds = floor.GetBounds();
        Assert.Equal(-11f, bounds.X);
        Assert.Equal(-11f, bounds.Z);
        Assert.Equal(22f, bounds.Width);
        Assert.Equal(22f, bounds.Depth);
        Assert.Equal(11f, bounds.Height);
    }

    // ── WallSegment ──

    [Fact]
    public void WallSegment_DistanceFromCentreIsPositive()
    {
        var floor = new OctagonalFloor(22f, 11f);
        var centre = new Vec3(0, 0, 0);
        foreach (var wall in floor.Walls)
        {
            Assert.True(wall.DistanceXZ(centre) > 5f);
        }
    }

    // ── SceneGeometry ──

    [Fact]
    public void SceneGeometry_CentreIsWalkable()
    {
        var geo = BuildGeometry();
        Assert.True(geo.IsWalkable(new Vec3(0, 0, 0)));
    }

    [Fact]
    public void SceneGeometry_OutsideNotWalkable()
    {
        var geo = BuildGeometry();
        Assert.False(geo.IsWalkable(new Vec3(50, 0, 50)));
    }

    [Fact]
    public void SceneGeometry_GetZoneAtCore()
    {
        var geo = BuildGeometry();
        // CORE is at (0,0,0) with bounds centred there, 6×6×6
        var zone = geo.GetZoneAt(new Vec3(0, 1, 0));
        Assert.NotNull(zone);
        Assert.Equal(HubZoneId.Core, zone.Id);
    }

    [Fact]
    public void SceneGeometry_GetZoneAtOutside_ReturnsNull()
    {
        var geo = BuildGeometry();
        var zone = geo.GetZoneAt(new Vec3(50, 0, 50));
        Assert.Null(zone);
    }

    [Fact]
    public void SceneGeometry_FindInteractablesInRange()
    {
        var geo = BuildGeometry();
        // Standing right next to CORE console at (0, 1.5, 0)
        var found = geo.GetInteractablesInRange(
            new Vec3(0, 0, 0), 3f, HubRhythmPhase.Activation);
        Assert.Contains(found, o => o.Id == "core_console");
    }

    [Fact]
    public void SceneGeometry_NearestInteractable()
    {
        var geo = BuildGeometry();
        var nearest = geo.GetNearestInteractable(
            new Vec3(-7f, 0f, -6f), 5f, HubRhythmPhase.Awakening);
        Assert.NotNull(nearest);
        Assert.Equal("capsule_exit", nearest.Id);
    }

    [Fact]
    public void SceneGeometry_DistanceToWall_CentreIsFar()
    {
        var geo = BuildGeometry();
        var dist = geo.DistanceToNearestWall(new Vec3(0, 0, 0));
        // Centre of 22m octagon — should be about 11m from any wall
        Assert.True(dist > 8f);
    }

    [Fact]
    public void SceneGeometry_ClampToWalkable_InternalUnchanged()
    {
        var geo = BuildGeometry();
        var pos = new Vec3(2, 0, 2);
        var clamped = geo.ClampToWalkable(pos);
        Assert.Equal(pos, clamped);
    }

    [Fact]
    public void SceneGeometry_ClampToWalkable_OutsidePushesIn()
    {
        var geo = BuildGeometry();
        var outside = new Vec3(50, 0, 0);
        var clamped = geo.ClampToWalkable(outside);
        Assert.True(geo.Floor.ContainsXZ(clamped));
    }

    // ── CollisionSystem3D ──

    [Fact]
    public void Collision_MoveInsideFloor_NotClamped()
    {
        var collision = BuildCollision();
        var result = collision.TryMove(new Vec3(0, 0, 0), new Vec3(2, 0, 3));
        Assert.False(result.WasClamped);
        Assert.False(result.WasBlocked);
    }

    [Fact]
    public void Collision_MoveOutsideFloor_Clamped()
    {
        var collision = BuildCollision();
        var result = collision.TryMove(new Vec3(0, 0, 0), new Vec3(50, 0, 0));
        Assert.True(result.WasClamped);
        Assert.NotEqual(50f, result.FinalPosition.X);
    }

    [Fact]
    public void Collision_CanInteract_WithinRadius()
    {
        var collision = BuildCollision();
        var zones = HubA0SceneBuilder.Build();
        var capsuleExit = zones.SelectMany(z => z.Objects).First(o => o.Id == "capsule_exit");

        // Standing right at the object
        var canInteract = collision.CanInteract(
            capsuleExit.Position, capsuleExit, HubRhythmPhase.Awakening);
        Assert.True(canInteract);
    }

    [Fact]
    public void Collision_CannotInteract_TooFar()
    {
        var collision = BuildCollision();
        var zones = HubA0SceneBuilder.Build();
        var capsuleExit = zones.SelectMany(z => z.Objects).First(o => o.Id == "capsule_exit");

        // Way too far
        var canInteract = collision.CanInteract(
            new Vec3(50, 0, 50), capsuleExit, HubRhythmPhase.Awakening);
        Assert.False(canInteract);
    }

    [Fact]
    public void Collision_CannotInteract_WrongPhase()
    {
        var collision = BuildCollision();
        var zones = HubA0SceneBuilder.Build();
        var bioScanner = zones.SelectMany(z => z.Objects).First(o => o.Id == "bio_scanner");

        // Right next to it but wrong phase
        var canInteract = collision.CanInteract(
            bioScanner.Position, bioScanner, HubRhythmPhase.Awakening);
        Assert.False(canInteract);
    }

    // ── Vec3 / Bounds3D ──

    [Fact]
    public void Vec3_ToVec2_ProjectsXZ()
    {
        var v = new Vec3(3f, 5f, 7f);
        var v2 = v.ToVec2();
        Assert.Equal(3f, v2.X);
        Assert.Equal(7f, v2.Y);
    }

    [Fact]
    public void Bounds3D_Contains()
    {
        var b = new Bounds3D(-3, 0, -3, 6, 6, 6);
        Assert.True(b.Contains(new Vec3(0, 3, 0)));
        Assert.False(b.Contains(new Vec3(10, 0, 0)));
    }

    // ── SceneBuilder 3D ──

    [Fact]
    public void SceneBuilder_AllZonesUseRealCoordinates()
    {
        var zones = HubA0SceneBuilder.Build();
        var core = zones.First(z => z.Id == HubZoneId.Core);
        // CORE at origin
        Assert.Equal(0f, core.Position.X);
        Assert.Equal(0f, core.Position.Y);
        Assert.Equal(0f, core.Position.Z);
    }

    [Fact]
    public void SceneBuilder_CapsuleAtCorrectPosition()
    {
        var zones = HubA0SceneBuilder.Build();
        var capsule = zones.First(z => z.Id == HubZoneId.Capsule);
        Assert.Equal(-8f, capsule.Position.X);
        Assert.Equal(-6f, capsule.Position.Z);
    }

    [Fact]
    public void SceneBuilder_AllActiveZonesInsideFloor()
    {
        var floor = HubA0SceneBuilder.BuildFloor();
        var zones = HubA0SceneBuilder.Build();
        var activeZones = zones.Where(z =>
            z.Id != HubZoneId.CommerceGateLocked &&
            z.Id != HubZoneId.TechGateLocked &&
            z.Id != HubZoneId.HardArchiveEntrance); // below floor

        foreach (var zone in activeZones)
        {
            Assert.True(floor.ContainsXZ(zone.Position),
                $"Zone {zone.Id} at ({zone.Position.X},{zone.Position.Z}) is outside the octagonal floor");
        }
    }

    [Fact]
    public void SceneBuilder_BuildFloor_22x11()
    {
        var floor = HubA0SceneBuilder.BuildFloor();
        Assert.Equal(22f, floor.Size);
        Assert.Equal(11f, floor.CeilingHeight);
    }

    // ── CameraOcclusion 3D ──

    [Fact]
    public void CameraOcclusion3D_DetectsBlockingObject()
    {
        var system = new CameraOcclusionSystem { CameraOffset = new Vec2(100, -100) };
        var playerPos = new Vec3(0, 0, 0);

        var obstacles = new List<Bounds3D>
        {
            new(40, 0, -60, 30, 3, 30), // on the camera→player line (projected to XZ)
        };

        var hits = system.ComputeOcclusions(playerPos, obstacles);
        Assert.Single(hits);
    }

    // ── Helpers ──

    private static SceneGeometry BuildGeometry()
    {
        var floor = HubA0SceneBuilder.BuildFloor();
        var zones = HubA0SceneBuilder.Build();
        return new SceneGeometry(floor, zones);
    }

    private static CollisionSystem3D BuildCollision()
    {
        var geo = BuildGeometry();
        return new CollisionSystem3D(geo);
    }
}
