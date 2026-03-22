using BabylonArchiveCore.Content.Pipeline;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain;
using BabylonArchiveCore.Domain.Player;
using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Runtime.Debug;
using BabylonArchiveCore.Runtime.Gameplay;
using BabylonArchiveCore.Runtime.Save;
using BabylonArchiveCore.Runtime.Scene;

namespace BabylonArchiveCore.Tests;

public class Session6IntegrationTests
{
    private sealed class NullLogger : ILogger
    {
        public void Info(string message) { }
        public void Warn(string message) { }
        public void Error(string message) { }
    }

    private static NullLogger Logger => new();

    private static string ContentRoot =>
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Content"));

    private static A0ContentProvider Provider => new(ContentRoot);

    // ════════════════════════════════════════════════════
    // ContentDrivenSceneFactory
    // ════════════════════════════════════════════════════

    [Fact]
    public void Factory_Build_CreatesAllComponents()
    {
        var factory = new ContentDrivenSceneFactory(Provider, Logger);
        var bundle = factory.Build();

        Assert.NotNull(bundle.Session);
        Assert.NotNull(bundle.HubRuntime);
        Assert.NotNull(bundle.InteractionHandler);
        Assert.NotNull(bundle.PrologueTracker);
        Assert.NotNull(bundle.Geometry);
        Assert.NotNull(bundle.Collision);
        Assert.NotNull(bundle.DialoguePlayer);
        Assert.NotNull(bundle.ObjectiveTracker);
        Assert.NotNull(bundle.TriggerSystem);
        Assert.NotNull(bundle.Inventory);
        Assert.NotNull(bundle.Journal);
        Assert.NotNull(bundle.Player);
    }

    [Fact]
    public void Factory_Build_LoadsZonesFromJson()
    {
        var bundle = new ContentDrivenSceneFactory(Provider, Logger).Build();
        Assert.True(bundle.HubRuntime.Zones.Count >= 9);
    }

    [Fact]
    public void Factory_Build_StartsAtAwakening()
    {
        var bundle = new ContentDrivenSceneFactory(Provider, Logger).Build();
        Assert.Equal(HubRhythmPhase.Awakening, bundle.HubRuntime.CurrentPhase);
    }

    [Fact]
    public void Factory_Build_PlayerAtCapsule()
    {
        var bundle = new ContentDrivenSceneFactory(Provider, Logger).Build();
        Assert.Equal(-7f, bundle.Player.Position.X);
        Assert.Equal(-6f, bundle.Player.Position.Z);
    }

    [Fact]
    public void Factory_Build_CustomPlayer()
    {
        var custom = new PlayerEntity { Position = new Vec3(1, 0, 2) };
        var bundle = new ContentDrivenSceneFactory(Provider, Logger).Build(player: custom);
        Assert.Same(custom, bundle.Player);
    }

    [Fact]
    public void Factory_FullPrologue_WorksEndToEnd()
    {
        var bundle = new ContentDrivenSceneFactory(Provider, Logger).Build();
        var sequence = new[] { "capsule_exit", "bio_scanner", "supply_terminal", "drone_dock", "core_console", "op_terminal", "gallery_overlook" };

        foreach (var objId in sequence)
        {
            var obj = bundle.HubRuntime.Zones.SelectMany(z => z.Objects).First(o => o.Id == objId);
            var baseResult = bundle.HubRuntime.Interact(objId);
            bundle.InteractionHandler.Handle(obj, baseResult);
            bundle.PrologueTracker.RecordVisit(objId);
        }

        Assert.Equal(HubRhythmPhase.OperationAccess, bundle.HubRuntime.CurrentPhase);
        Assert.True(bundle.PrologueTracker.IsProtocolZeroUnlocked);
        Assert.Equal(3, bundle.Inventory.Items.Count);
    }

    // ════════════════════════════════════════════════════
    // DebugOverlay
    // ════════════════════════════════════════════════════

    [Fact]
    public void DebugOverlay_Disabled_ReturnsNull()
    {
        var overlay = new DebugOverlay(DebugConfig.Production());
        var snap = overlay.Update(1f / 60f, HubZoneId.Core, HubRhythmPhase.Awakening, null, 0, Vec3.Zero);
        Assert.Null(snap);
    }

    [Fact]
    public void DebugOverlay_Enabled_ReturnsSnapshot()
    {
        var overlay = new DebugOverlay(DebugConfig.Development());
        var snap = overlay.Update(1f / 60f, HubZoneId.Core, HubRhythmPhase.Identification, "bio_scanner", 2, new Vec3(1, 0, 2));
        Assert.NotNull(snap);
        Assert.Equal(HubZoneId.Core, snap.Zone);
        Assert.Equal(HubRhythmPhase.Identification, snap.Phase);
        Assert.Equal("bio_scanner", snap.FocusObject);
        Assert.Equal(2, snap.TriggersFired);
    }

    [Fact]
    public void DebugOverlay_Format_ContainsAllFields()
    {
        var overlay = new DebugOverlay(DebugConfig.Development());
        var snap = overlay.Update(1f / 60f, HubZoneId.Capsule, HubRhythmPhase.Awakening, "capsule_exit", 0, new Vec3(-7, 0, -6));
        Assert.NotNull(snap);
        var text = snap.Format();
        Assert.Contains("Zone: Capsule", text);
        Assert.Contains("Phase: Awakening", text);
        Assert.Contains("Focus: capsule_exit", text);
    }

    [Fact]
    public void DebugOverlay_FpsCalculated_After60Frames()
    {
        var config = DebugConfig.Development();
        var overlay = new DebugOverlay(config);
        for (int i = 0; i < 60; i++)
            overlay.Update(1f / 60f, null, HubRhythmPhase.Awakening, null, 0, Vec3.Zero);

        Assert.True(overlay.CurrentFps > 0);
    }

    // ════════════════════════════════════════════════════
    // DebugTeleport
    // ════════════════════════════════════════════════════

    [Fact]
    public void DebugTeleport_Disabled_ReturnsNull()
    {
        var teleport = new DebugTeleport(DebugConfig.Production(), Logger);
        var player = new PlayerEntity();
        var hub = new HubA0Runtime(HubA0SceneBuilder.Build(), new EventJournal(), Logger);
        var result = teleport.TeleportTo(HubZoneId.Core, player, hub);
        Assert.Null(result);
    }

    [Fact]
    public void DebugTeleport_Enabled_MovesPlayer()
    {
        var teleport = new DebugTeleport(DebugConfig.Development(), Logger);
        var player = new PlayerEntity { Position = new Vec3(0, 0, 0) };
        var hub = new HubA0Runtime(HubA0SceneBuilder.Build(), new EventJournal(), Logger);
        var result = teleport.TeleportTo(HubZoneId.Core, player, hub);
        Assert.NotNull(result);
        Assert.Equal(0f, result.Value.X);
        Assert.Equal(0f, result.Value.Z);
        Assert.Equal(result.Value, player.Position);
    }

    [Fact]
    public void DebugTeleport_ToCapsule_CorrectPosition()
    {
        var teleport = new DebugTeleport(DebugConfig.Development(), Logger);
        var player = new PlayerEntity();
        var hub = new HubA0Runtime(HubA0SceneBuilder.Build(), new EventJournal(), Logger);
        var result = teleport.TeleportTo(HubZoneId.Capsule, player, hub);
        Assert.NotNull(result);
        Assert.Equal(-8f, result.Value.X);
        Assert.Equal(-6f, result.Value.Z);
    }

    [Fact]
    public void DebugTeleport_ListDestinations_ReturnsAll()
    {
        var teleport = new DebugTeleport(DebugConfig.Development(), Logger);
        var hub = new HubA0Runtime(HubA0SceneBuilder.Build(), new EventJournal(), Logger);
        var dests = teleport.ListDestinations(hub);
        Assert.True(dests.Count >= 9);
    }

    // ════════════════════════════════════════════════════
    // SaveGameMapper — Extract
    // ════════════════════════════════════════════════════

    [Fact]
    public void Mapper_Extract_CapturesPhase()
    {
        var bundle = new ContentDrivenSceneFactory(Provider, Logger).Build();
        bundle.HubRuntime.Interact("capsule_exit");

        var save = SaveGameMapper.Extract(bundle.HubRuntime, bundle.PrologueTracker,
            bundle.ObjectiveTracker, bundle.Inventory, 0);

        Assert.Equal("Identification", save.CurrentPhase);
    }

    [Fact]
    public void Mapper_Extract_CapturesVisited()
    {
        var bundle = new ContentDrivenSceneFactory(Provider, Logger).Build();
        bundle.PrologueTracker.RecordVisit("capsule_exit");
        bundle.PrologueTracker.RecordVisit("bio_scanner");

        var save = SaveGameMapper.Extract(bundle.HubRuntime, bundle.PrologueTracker,
            bundle.ObjectiveTracker, bundle.Inventory, 25);

        Assert.Equal(2, save.VisitedObjects.Count);
        Assert.Contains("capsule_exit", save.VisitedObjects);
        Assert.Contains("bio_scanner", save.VisitedObjects);
        Assert.Equal(25, save.OperatorXp);
    }

    [Fact]
    public void Mapper_Extract_CapturesInventory()
    {
        var bundle = new ContentDrivenSceneFactory(Provider, Logger).Build();
        bundle.Inventory.Add(new InventoryItem { ItemId = "ITEM_TEST", Name = "Test", Type = ItemType.KeyItem });

        var save = SaveGameMapper.Extract(bundle.HubRuntime, bundle.PrologueTracker,
            bundle.ObjectiveTracker, bundle.Inventory, 0);

        Assert.Single(save.InventoryItemIds);
        Assert.Equal("ITEM_TEST", save.InventoryItemIds[0]);
    }

    // ════════════════════════════════════════════════════
    // SaveGameMapper — Apply (round-trip)
    // ════════════════════════════════════════════════════

    [Fact]
    public void Mapper_RoundTrip_PreservesPhase()
    {
        // Build initial, advance to Provisioning
        var b1 = new ContentDrivenSceneFactory(Provider, Logger).Build();
        b1.HubRuntime.Interact("capsule_exit");
        b1.HubRuntime.Interact("bio_scanner");
        b1.PrologueTracker.RecordVisit("capsule_exit");
        b1.PrologueTracker.RecordVisit("bio_scanner");

        var save = SaveGameMapper.Extract(b1.HubRuntime, b1.PrologueTracker,
            b1.ObjectiveTracker, b1.Inventory, 50);

        // Build fresh and apply
        var b2 = new ContentDrivenSceneFactory(Provider, Logger).Build();
        SaveGameMapper.Apply(save, b2.HubRuntime, b2.PrologueTracker,
            b2.ObjectiveTracker, b2.Inventory);

        Assert.Equal(HubRhythmPhase.Provisioning, b2.HubRuntime.CurrentPhase);
        Assert.True(b2.PrologueTracker.HasVisited("capsule_exit"));
        Assert.True(b2.PrologueTracker.HasVisited("bio_scanner"));
    }

    [Fact]
    public void Mapper_RoundTrip_PreservesInventory()
    {
        var b1 = new ContentDrivenSceneFactory(Provider, Logger).Build();
        b1.Inventory.Add(new InventoryItem { ItemId = "ITEM_X", Name = "X", Type = ItemType.Tool });

        var save = SaveGameMapper.Extract(b1.HubRuntime, b1.PrologueTracker,
            b1.ObjectiveTracker, b1.Inventory, 10);

        var b2 = new ContentDrivenSceneFactory(Provider, Logger).Build();
        SaveGameMapper.Apply(save, b2.HubRuntime, b2.PrologueTracker,
            b2.ObjectiveTracker, b2.Inventory);

        Assert.Single(b2.Inventory.Items);
        Assert.Equal("ITEM_X", b2.Inventory.Items[0].ItemId);
    }

    [Fact]
    public void Mapper_RoundTrip_ProtocolZero()
    {
        var b1 = new ContentDrivenSceneFactory(Provider, Logger).Build();
        foreach (var id in new[] { "capsule_exit", "bio_scanner", "supply_terminal", "drone_dock", "core_console", "op_terminal", "gallery_overlook" })
        {
            b1.HubRuntime.Interact(id);
            b1.PrologueTracker.RecordVisit(id);
        }

        var save = SaveGameMapper.Extract(b1.HubRuntime, b1.PrologueTracker,
            b1.ObjectiveTracker, b1.Inventory, 145);

        Assert.True(save.ProtocolZeroUnlocked);

        var b2 = new ContentDrivenSceneFactory(Provider, Logger).Build();
        SaveGameMapper.Apply(save, b2.HubRuntime, b2.PrologueTracker,
            b2.ObjectiveTracker, b2.Inventory);

        Assert.True(b2.PrologueTracker.IsProtocolZeroUnlocked);
        Assert.Equal(HubRhythmPhase.OperationAccess, b2.HubRuntime.CurrentPhase);
    }

    // ════════════════════════════════════════════════════
    // SaveResumeIntegration
    // ════════════════════════════════════════════════════

    [Fact]
    public void Resume_FreshStart_NoRestore()
    {
        var resume = new SaveResumeIntegration(Provider, Logger);
        var bundle = resume.BuildAndResume();
        Assert.Equal(HubRhythmPhase.Awakening, bundle.HubRuntime.CurrentPhase);
    }

    [Fact]
    public void Resume_MidPrologue_RestoresPhase()
    {
        var save = new SaveGame
        {
            CurrentPhase = "DroneContact",
            VisitedObjects = ["capsule_exit", "bio_scanner", "supply_terminal"],
            InventoryItemIds = ["ITEM_ARCHIVE_BADGE"],
        };

        var resume = new SaveResumeIntegration(Provider, Logger);
        var bundle = resume.BuildAndResume(save);

        Assert.Equal(HubRhythmPhase.DroneContact, bundle.HubRuntime.CurrentPhase);
        Assert.True(bundle.PrologueTracker.HasVisited("capsule_exit"));
        Assert.True(bundle.PrologueTracker.HasVisited("bio_scanner"));
        Assert.True(bundle.PrologueTracker.HasVisited("supply_terminal"));
        Assert.Single(bundle.Inventory.Items);
    }

    [Fact]
    public void Resume_OperationAccess_FullRestore()
    {
        var save = new SaveGame
        {
            CurrentPhase = "OperationAccess",
            VisitedObjects = ["capsule_exit", "bio_scanner", "supply_terminal", "drone_dock", "core_console", "op_terminal", "gallery_overlook"],
            InventoryItemIds = ["ITEM_ARCHIVE_BADGE", "ITEM_BASIC_SCANNER", "ITEM_FIELD_RATION"],
            ProtocolZeroUnlocked = true,
        };

        var resume = new SaveResumeIntegration(Provider, Logger);
        var bundle = resume.BuildAndResume(save);

        Assert.Equal(HubRhythmPhase.OperationAccess, bundle.HubRuntime.CurrentPhase);
        Assert.True(bundle.PrologueTracker.IsProtocolZeroUnlocked);
        Assert.Equal(3, bundle.Inventory.Items.Count);
    }

    [Fact]
    public void Resume_AwakeningPhase_NoAdvancement()
    {
        var save = new SaveGame { CurrentPhase = "Awakening" };

        var resume = new SaveResumeIntegration(Provider, Logger);
        var bundle = resume.BuildAndResume(save);

        Assert.Equal(HubRhythmPhase.Awakening, bundle.HubRuntime.CurrentPhase);
        Assert.False(bundle.PrologueTracker.IsProtocolZeroUnlocked);
    }

    // ════════════════════════════════════════════════════
    // Integration: Factory → Session → Save → Resume
    // ════════════════════════════════════════════════════

    [Fact]
    public void FullCycle_Play_Save_Resume_Continues()
    {
        // 1. Build and play partial prologue
        var b1 = new ContentDrivenSceneFactory(Provider, Logger).Build();
        foreach (var id in new[] { "capsule_exit", "bio_scanner", "supply_terminal" })
        {
            var obj = b1.HubRuntime.Zones.SelectMany(z => z.Objects).First(o => o.Id == id);
            var br = b1.HubRuntime.Interact(id);
            b1.InteractionHandler.Handle(obj, br);
            b1.PrologueTracker.RecordVisit(id);
        }

        // 2. Save
        var save = SaveGameMapper.Extract(b1.HubRuntime, b1.PrologueTracker,
            b1.ObjectiveTracker, b1.Inventory, 60);
        Assert.Equal("DroneContact", save.CurrentPhase);
        Assert.Equal(3, save.InventoryItemIds.Count); // from supply_terminal

        // 3. Resume in new session
        var resume = new SaveResumeIntegration(Provider, Logger);
        var b2 = resume.BuildAndResume(save);
        Assert.Equal(HubRhythmPhase.DroneContact, b2.HubRuntime.CurrentPhase);

        // 4. Continue playing
        var droneObj = b2.HubRuntime.Zones.SelectMany(z => z.Objects).First(o => o.Id == "drone_dock");
        var droneResult = b2.HubRuntime.Interact("drone_dock");
        b2.InteractionHandler.Handle(droneObj, droneResult);
        b2.PrologueTracker.RecordVisit("drone_dock");

        Assert.Equal(HubRhythmPhase.Activation, b2.HubRuntime.CurrentPhase);
    }
}
