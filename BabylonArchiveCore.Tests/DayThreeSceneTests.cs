using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Runtime.Scene;

namespace BabylonArchiveCore.Tests;

public class DayThreeSceneTests
{
    // --- Scene Builder ---

    [Fact]
    public void SceneBuilder_ProducesAllZones()
    {
        var zones = HubA0SceneBuilder.Build();

        Assert.Equal(11, zones.Count);
        Assert.Contains(zones, z => z.Id == HubZoneId.Capsule);
        Assert.Contains(zones, z => z.Id == HubZoneId.Biometrics);
        Assert.Contains(zones, z => z.Id == HubZoneId.Logistics);
        Assert.Contains(zones, z => z.Id == HubZoneId.DroneNiche);
        Assert.Contains(zones, z => z.Id == HubZoneId.Core);
        Assert.Contains(zones, z => z.Id == HubZoneId.MissionTerminal);
        Assert.Contains(zones, z => z.Id == HubZoneId.Research);
        Assert.Contains(zones, z => z.Id == HubZoneId.ObservationGallery);
        Assert.Contains(zones, z => z.Id == HubZoneId.HardArchiveEntrance);
        Assert.Contains(zones, z => z.Id == HubZoneId.CommerceGateLocked);
        Assert.Contains(zones, z => z.Id == HubZoneId.TechGateLocked);
    }

    [Fact]
    public void SceneBuilder_ActiveZonesHaveObjects()
    {
        var zones = HubA0SceneBuilder.Build();
        var activeZones = zones.Where(z =>
            z.Id != HubZoneId.CommerceGateLocked &&
            z.Id != HubZoneId.TechGateLocked);
        Assert.All(activeZones, z => Assert.NotEmpty(z.Objects));
    }

    // --- Scene Rhythm ---

    [Fact]
    public void Rhythm_StartsAtAwakening()
    {
        var runtime = CreateRuntime();
        Assert.Equal(HubRhythmPhase.Awakening, runtime.CurrentPhase);
    }

    [Fact]
    public void Rhythm_FullProgressionSequence()
    {
        var runtime = CreateRuntime();

        // In Awakening: can only use capsule_exit
        var available = runtime.GetAvailableInteractables();
        Assert.Single(available);
        Assert.Equal("capsule_exit", available[0].Id);

        // Step 1: exit capsule → Identification
        var r1 = runtime.Interact("capsule_exit");
        Assert.True(r1.Success);
        Assert.Equal(HubRhythmPhase.Identification, r1.NewPhase);
        Assert.Equal(HubRhythmPhase.Identification, runtime.CurrentPhase);

        // Step 2: bio-scan → Provisioning
        var r2 = runtime.Interact("bio_scanner");
        Assert.True(r2.Success);
        Assert.Equal(HubRhythmPhase.Provisioning, r2.NewPhase);

        // Step 3: get supplies → DroneContact
        var r3 = runtime.Interact("supply_terminal");
        Assert.True(r3.Success);
        Assert.Equal(HubRhythmPhase.DroneContact, r3.NewPhase);

        // Step 4: activate drone → Activation
        var r4 = runtime.Interact("drone_dock");
        Assert.True(r4.Success);
        Assert.Equal(HubRhythmPhase.Activation, r4.NewPhase);

        // Step 5: activate CORE → OperationAccess
        var r5 = runtime.Interact("core_console");
        Assert.True(r5.Success);
        Assert.Equal(HubRhythmPhase.OperationAccess, r5.NewPhase);

        // Now all end-zone objects are available
        var final = runtime.GetAvailableInteractables();
        Assert.Contains(final, o => o.Id == "op_terminal");
        Assert.Contains(final, o => o.Id == "research_terminal");
        Assert.Contains(final, o => o.Id == "gallery_overlook");
        Assert.Contains(final, o => o.Id == "archive_gate");
    }

    [Fact]
    public void Rhythm_CannotInteractBeforeRequiredPhase()
    {
        var runtime = CreateRuntime();

        var result = runtime.Interact("bio_scanner");
        Assert.False(result.Success);
        Assert.Contains("Требуется фаза", result.Message);
    }

    [Fact]
    public void Rhythm_UsedObjectNotAvailableAgain()
    {
        var runtime = CreateRuntime();
        runtime.Interact("capsule_exit");

        var available = runtime.GetAvailableInteractables();
        Assert.DoesNotContain(available, o => o.Id == "capsule_exit");
    }

    // --- Interaction Hints ---

    [Fact]
    public void Hints_NearbyObjectReturnsHint()
    {
        var runtime = CreateRuntime();
        // Capsule exit is at (-7, 0.5, -6)
        var hints = runtime.GetNearbyHints(new Vec3(-7f, 0f, -6f), radius: 3f);

        Assert.Single(hints);
        Assert.Equal("capsule_exit", hints[0].Object.Id);
    }

    [Fact]
    public void Hints_FarObjectReturnsNoHint()
    {
        var runtime = CreateRuntime();
        var hints = runtime.GetNearbyHints(new Vec3(50f, 0f, 50f), radius: 3f);

        Assert.Empty(hints);
    }

    // --- Event Journal ---

    [Fact]
    public void Journal_RecordsInteractions()
    {
        var runtime = CreateRuntime();
        runtime.Interact("capsule_exit");
        runtime.Interact("bio_scanner");

        var entries = runtime.Journal.GetByCategory("interaction");
        Assert.Equal(2, entries.Count);
    }

    [Fact]
    public void Journal_RecordsRhythmAdvances()
    {
        var runtime = CreateRuntime();
        runtime.Interact("capsule_exit");

        var rhythm = runtime.Journal.GetByCategory("rhythm");
        Assert.Single(rhythm);
        Assert.Contains("Identification", rhythm[0].Message);
    }

    [Fact]
    public void Journal_InitialSystemEntry()
    {
        var runtime = CreateRuntime();
        Assert.NotNull(runtime.Journal.Latest);
        Assert.Equal("system", runtime.Journal.Latest!.Category);
    }

    // --- Camera Occlusion ---

    [Fact]
    public void Occlusion_DetectsBlockingObject()
    {
        var system = new CameraOcclusionSystem { CameraOffset = new Vec2(100, -100) };
        var playerPos = new Vec2(0, 0);

        // Object between camera (100, -100) and player (0, 0)
        var obstacles = new List<Bounds2D>
        {
            new(40, -60, 30, 30), // on the camera→player line
        };

        var hits = system.ComputeOcclusions(playerPos, obstacles);
        Assert.Single(hits);
        Assert.Equal(0, hits[0].ObstacleIndex);
    }

    [Fact]
    public void Occlusion_IgnoresNonBlockingObject()
    {
        var system = new CameraOcclusionSystem { CameraOffset = new Vec2(100, -100) };
        var playerPos = new Vec2(0, 0);

        // Object far from the camera→player line
        var obstacles = new List<Bounds2D>
        {
            new(-200, 200, 30, 30),
        };

        var hits = system.ComputeOcclusions(playerPos, obstacles);
        Assert.Empty(hits);
    }

    [Fact]
    public void Occlusion_MultipleBlockingObjects()
    {
        var system = new CameraOcclusionSystem { CameraOffset = new Vec2(200, -200) };
        var playerPos = new Vec2(0, 0);

        var obstacles = new List<Bounds2D>
        {
            new(60, -80, 40, 40),  // on line
            new(120, -140, 30, 30), // on line
            new(-300, 300, 30, 30), // off line
        };

        var hits = system.ComputeOcclusions(playerPos, obstacles);
        Assert.Equal(2, hits.Count);
    }

    // --- InteractableObject ---

    [Fact]
    public void InteractableObject_IsActiveRespectsBothPhaseAndState()
    {
        var obj = new InteractableObject
        {
            Id = "test", DisplayName = "T", HintText = "H",
            Zone = HubZoneId.Capsule,
            Position = new Vec3(0, 0, 0), Bounds = new Bounds3D(0, 0, 0, 10, 10, 10),
            RequiredPhase = HubRhythmPhase.Supply,
        };

        Assert.False(obj.IsActiveIn(HubRhythmPhase.Awakening));
        Assert.False(obj.IsActiveIn(HubRhythmPhase.Identification));
        Assert.True(obj.IsActiveIn(HubRhythmPhase.Supply));
        Assert.True(obj.IsActiveIn(HubRhythmPhase.OperationAccess));

        obj.ContextState = "used";
        Assert.False(obj.IsActiveIn(HubRhythmPhase.OperationAccess));
    }

    // --- Helpers ---

    private static HubA0Runtime CreateRuntime()
    {
        var zones = HubA0SceneBuilder.Build();
        var journal = new EventJournal();
        var logger = new NullTestLogger();
        return new HubA0Runtime(zones, journal, logger);
    }

    private sealed class NullTestLogger : ILogger
    {
        public void Info(string message) { }
        public void Warn(string message) { }
        public void Error(string message) { }
    }
}
