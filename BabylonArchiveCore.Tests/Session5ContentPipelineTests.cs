using BabylonArchiveCore.Content.DataContracts;
using BabylonArchiveCore.Content.Pipeline;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain;
using BabylonArchiveCore.Domain.Player;
using BabylonArchiveCore.Domain.Progression;
using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Runtime.Gameplay;
using BabylonArchiveCore.Runtime.Scene;
using System.Reflection;

namespace BabylonArchiveCore.Tests;

public class Session5ContentPipelineTests
{
    // ── Helpers ──

    private sealed class NullLogger : ILogger
    {
        public void Info(string message) { }
        public void Warn(string message) { }
        public void Error(string message) { }
    }

    private static NullLogger Logger => new();

    // ═══════════════════════════════════════════════════
    // A0ContentMapper — Zone ID mapping
    // ═══════════════════════════════════════════════════

    [Theory]
    [InlineData("ZONE_CAPSULE", HubZoneId.Capsule)]
    [InlineData("ZONE_BIOMETRIC", HubZoneId.Biometrics)]
    [InlineData("ZONE_LOGISTICS", HubZoneId.Logistics)]
    [InlineData("ZONE_DRONE_NICHE", HubZoneId.DroneNiche)]
    [InlineData("ZONE_CORE", HubZoneId.Core)]
    [InlineData("ZONE_MISSION_TERMINAL", HubZoneId.MissionTerminal)]
    [InlineData("ZONE_RESEARCH", HubZoneId.Research)]
    [InlineData("ZONE_OBSERVATION_GALLERY", HubZoneId.ObservationGallery)]
    [InlineData("ZONE_HARD_ARCHIVE_ENTRY_LOCKED", HubZoneId.HardArchiveEntrance)]
    [InlineData("ZONE_COMMERCE_GATE_LOCKED", HubZoneId.CommerceGateLocked)]
    [InlineData("ZONE_TECH_GATE_LOCKED", HubZoneId.TechGateLocked)]
    public void MapZoneId_AllZones_Match(string jsonId, HubZoneId expected)
    {
        Assert.Equal(expected, A0ContentMapper.MapZoneId(jsonId));
    }

    [Fact]
    public void MapZoneId_UnknownZone_Throws()
    {
        Assert.Throws<ArgumentException>(() => A0ContentMapper.MapZoneId("ZONE_UNKNOWN"));
    }

    // ═══════════════════════════════════════════════════
    // A0ContentMapper — Phase mapping
    // ═══════════════════════════════════════════════════

    [Theory]
    [InlineData("Awakening", HubRhythmPhase.Awakening)]
    [InlineData("Identification", HubRhythmPhase.Identification)]
    [InlineData("Provisioning", HubRhythmPhase.Provisioning)]
    [InlineData("DroneContact", HubRhythmPhase.DroneContact)]
    [InlineData("Activation", HubRhythmPhase.Activation)]
    [InlineData("OperationAccess", HubRhythmPhase.OperationAccess)]
    public void MapPhase_AllPhases_Match(string phaseName, HubRhythmPhase expected)
    {
        Assert.Equal(expected, A0ContentMapper.MapPhase(phaseName));
    }

    [Fact]
    public void MapPhase_Unknown_Throws()
    {
        Assert.Throws<ArgumentException>(() => A0ContentMapper.MapPhase("InvalidPhase"));
    }

    // ═══════════════════════════════════════════════════
    // A0ContentMapper — InteractiveType mapping
    // ═══════════════════════════════════════════════════

    [Theory]
    [InlineData("trigger", InteractiveType.Trigger)]
    [InlineData("terminal", InteractiveType.Terminal)]
    [InlineData("npc", InteractiveType.Npc)]
    [InlineData("gate", InteractiveType.Gate)]
    public void MapType_AllTypes_Match(string typeName, InteractiveType expected)
    {
        Assert.Equal(expected, A0ContentMapper.MapType(typeName));
    }

    // ═══════════════════════════════════════════════════
    // A0ContentMapper — Object ID mapping
    // ═══════════════════════════════════════════════════

    [Theory]
    [InlineData("OBJ_CAPSULE_EXIT", "capsule_exit")]
    [InlineData("OBJ_BIO_SCANNER", "bio_scanner")]
    [InlineData("OBJ_SUPPLY_TERMINAL", "supply_terminal")]
    [InlineData("OBJ_DRONE_DOCK", "drone_dock")]
    [InlineData("OBJ_CORE_CONSOLE", "core_console")]
    [InlineData("OBJ_OP_TERMINAL", "op_terminal")]
    [InlineData("OBJ_RESEARCH_TERMINAL", "research_terminal")]
    [InlineData("OBJ_GALLERY_OVERLOOK", "gallery_overlook")]
    [InlineData("OBJ_ARCHIVE_GATE", "archive_gate")]
    public void MapObjectId_AllObjects_Match(string jsonId, string expected)
    {
        Assert.Equal(expected, A0ContentMapper.MapObjectId(jsonId));
    }

    // ═══════════════════════════════════════════════════
    // A0ContentMapper — Vec3 mapping
    // ═══════════════════════════════════════════════════

    [Fact]
    public void MapVec3_ConvertsCorrectly()
    {
        var v = A0ContentMapper.MapVec3(new Vec3Data(1.5f, 2.0f, -3.0f));
        Assert.Equal(1.5f, v.X);
        Assert.Equal(2.0f, v.Y);
        Assert.Equal(-3.0f, v.Z);
    }

    // ═══════════════════════════════════════════════════
    // A0ContentMapper — MapObject
    // ═══════════════════════════════════════════════════

    [Fact]
    public void MapObject_SetsAllProperties()
    {
        var data = new InteractableObjectData
        {
            ObjectId = "OBJ_SUPPLY_TERMINAL",
            DisplayName = "Терминал снабжения",
            ZoneId = "ZONE_LOGISTICS",
            Type = "terminal",
            Position = new Vec3Data(8f, 1f, -6f),
            InteractionRadius = 1.5f,
            HintText = "Получи снаряжение",
            RequiredPhase = "Provisioning",
            DialogueId = "DIA_SUPPLY_GRANTED",
            GrantsItems = ["ITEM_ARCHIVE_BADGE", "ITEM_BASIC_SCANNER"],
        };

        var obj = A0ContentMapper.MapObject(data, HubZoneId.Logistics);

        Assert.Equal("supply_terminal", obj.Id);
        Assert.Equal("Терминал снабжения", obj.DisplayName);
        Assert.Equal(InteractiveType.Terminal, obj.InteractiveType);
        Assert.Equal(HubRhythmPhase.Provisioning, obj.RequiredPhase);
        Assert.Equal("DIA_SUPPLY_GRANTED", obj.DialogueId);
        Assert.NotNull(obj.GrantsItems);
        Assert.Equal(2, obj.GrantsItems!.Count);
        Assert.Equal(1.5f, obj.InteractionRadius);
    }

    [Fact]
    public void MapObject_ForwardsModelId()
    {
        var data = new InteractableObjectData
        {
            ObjectId = "OBJ_CORE_CONSOLE",
            DisplayName = "Консоль C.O.R.E.",
            ZoneId = "ZONE_CORE",
            Type = "terminal",
            Position = new Vec3Data(0f, 1.5f, 0f),
            InteractionRadius = 2.5f,
            HintText = "Доступ к системам Архива",
            RequiredPhase = "Activation",
            ModelId = ContourModelIds.CoreConsole,
        };

        var obj = A0ContentMapper.MapObject(data, HubZoneId.Core);

        Assert.Equal(ContourModelIds.CoreConsole, obj.ModelId);
    }

    [Fact]
    public void MapObject_Gate_SetsLockedMessage()
    {
        var data = new InteractableObjectData
        {
            ObjectId = "OBJ_ARCHIVE_GATE",
            DisplayName = "Врата",
            ZoneId = "ZONE_HARD_ARCHIVE_ENTRY_LOCKED",
            Type = "gate",
            Position = new Vec3Data(0f, 0f, 11f),
            InteractionRadius = 2f,
            HintText = "Доступ заблокирован",
            RequiredPhase = "OperationAccess",
            LockedMessage = "Врата откроются после завершения первой операции.",
        };

        var obj = A0ContentMapper.MapObject(data, HubZoneId.HardArchiveEntrance);

        Assert.Equal(InteractiveType.Gate, obj.InteractiveType);
        Assert.NotNull(obj.LockedMessage);
        Assert.Contains("первой операции", obj.LockedMessage);
    }

    // ═══════════════════════════════════════════════════
    // A0ContentMapper — MapDialogues
    // ═══════════════════════════════════════════════════

    [Fact]
    public void MapDialogues_MapsAllDialogues()
    {
        var data = new DialogueFileData
        {
            SceneId = "SCN_A0_INITIATION",
            Dialogues =
            [
                new DialogueData
                {
                    DialogueId = "DIA_DRONE_GREETING",
                    Speaker = "NPC_ARCHIVE_DRONE",
                    SpeakerDisplayName = "Архивный дрон",
                    Lines =
                    [
                        new DialogueLineData { LineId = "L1", Text = "Line 1", Delay = 1.5f },
                        new DialogueLineData { LineId = "L2", Text = "Line 2", Delay = 2.0f },
                    ],
                },
                new DialogueData
                {
                    DialogueId = "DIA_BIO_CONFIRM",
                    Speaker = "SYSTEM",
                    SpeakerDisplayName = "Система",
                    Lines = [new DialogueLineData { LineId = "B1", Text = "Bio text" }],
                },
            ],
        };

        var dialogues = A0ContentMapper.MapDialogues(data);

        Assert.Equal(2, dialogues.Count);
        Assert.Equal("DIA_DRONE_GREETING", dialogues[0].DialogueId);
        Assert.Equal("Архивный дрон", dialogues[0].SpeakerDisplayName);
        Assert.Equal(2, dialogues[0].Lines.Count);
        Assert.Equal("NPC_ARCHIVE_DRONE", dialogues[0].Lines[0].Speaker);
        Assert.Equal("DIA_BIO_CONFIRM", dialogues[1].DialogueId);
    }

    // ═══════════════════════════════════════════════════
    // A0ContentMapper — MapTriggers
    // ═══════════════════════════════════════════════════

    [Fact]
    public void MapTriggers_FiltersPhaseChangeOnly()
    {
        var data = new TriggerFileData
        {
            SceneId = "SCN_A0_INITIATION",
            Triggers =
            [
                new TriggerData
                {
                    TriggerId = "TRG_PHASE_IDENTIFICATION",
                    Type = "phase_change",
                    Condition = "phase == Identification",
                    Actions =
                    [
                        new TriggerActionData { Type = "set_objective", ObjectiveId = "UI_OBJ_BIOMETRIC" },
                        new TriggerActionData { Type = "journal_entry", Text = "Капсула открыта." },
                    ],
                },
                new TriggerData
                {
                    TriggerId = "TRG_GALLERY_LOOK",
                    Type = "object_interact",
                    Condition = "objectId == OBJ_GALLERY_OVERLOOK",
                    Actions = [new TriggerActionData { Type = "journal_entry", Text = "Gallery" }],
                },
            ],
        };

        var triggers = A0ContentMapper.MapTriggers(data);

        Assert.Single(triggers);
        Assert.Equal("TRG_PHASE_IDENTIFICATION", triggers[0].TriggerId);
        Assert.Equal(HubRhythmPhase.Identification, triggers[0].Phase);
        Assert.Equal(2, triggers[0].Actions.Count);
        Assert.Equal(TriggerActionType.SetObjective, triggers[0].Actions[0].Type);
        Assert.Equal(TriggerActionType.JournalEntry, triggers[0].Actions[1].Type);
    }

    [Fact]
    public void MapTriggers_PlayDialogue_SkipsNullDialogueId()
    {
        var data = new TriggerFileData
        {
            SceneId = "test",
            Triggers =
            [
                new TriggerData
                {
                    TriggerId = "TRG_START",
                    Type = "phase_change",
                    Condition = "phase == Awakening",
                    Actions =
                    [
                        new TriggerActionData { Type = "play_dialogue", DialogueId = null },
                        new TriggerActionData { Type = "set_objective", ObjectiveId = "OBJ_1" },
                    ],
                },
            ],
        };

        var triggers = A0ContentMapper.MapTriggers(data);

        Assert.Single(triggers);
        Assert.Single(triggers[0].Actions); // play_dialogue with null skipped
        Assert.Equal(TriggerActionType.SetObjective, triggers[0].Actions[0].Type);
    }

    // ═══════════════════════════════════════════════════
    // A0ContentMapper — MapObjectives
    // ═══════════════════════════════════════════════════

    [Fact]
    public void MapObjectives_MapsAllFields()
    {
        var data = new ObjectiveFileData
        {
            SceneId = "SCN_A0_INITIATION",
            Objectives =
            [
                new ObjectiveData { ObjectiveId = "UI_OBJ_EXIT_CAPSULE", Text = "Выйти из капсулы", Phase = "Awakening", Order = 1, TargetObject = "OBJ_CAPSULE_EXIT" },
                new ObjectiveData { ObjectiveId = "UI_OBJ_BIOMETRIC", Text = "Пройти идентификацию", Phase = "Identification", Order = 2 },
            ],
        };

        var objectives = A0ContentMapper.MapObjectives(data);

        Assert.Equal(2, objectives.Count);
        Assert.Equal("UI_OBJ_EXIT_CAPSULE", objectives[0].ObjectiveId);
        Assert.Equal("Выйти из капсулы", objectives[0].Text);
        Assert.Equal(1, objectives[0].Order);
        Assert.Equal(ObjectiveStatus.Locked, objectives[0].Status);
    }

    // ═══════════════════════════════════════════════════
    // A0ContentMapper — MapZones with objects
    // ═══════════════════════════════════════════════════

    [Fact]
    public void MapZones_AssociatesObjectsWithCorrectZones()
    {
        var zoneData = new ZoneFileData
        {
            SceneId = "SCN_A0_INITIATION",
            Zones =
            [
                new ZoneData { ZoneId = "ZONE_CAPSULE", DisplayName = "Капсула", Position = new Vec3Data(-8, 0, -6), Size = new SizeData(4, 3, 3), RequiredPhase = "Awakening" },
                new ZoneData { ZoneId = "ZONE_BIOMETRIC", DisplayName = "Биометрия", Position = new Vec3Data(-8, 0, 0), Size = new SizeData(4, 3, 3), RequiredPhase = "Identification" },
            ],
        };

        var objData = new ObjectFileData
        {
            SceneId = "SCN_A0_INITIATION",
            Objects =
            [
                new InteractableObjectData
                {
                    ObjectId = "OBJ_CAPSULE_EXIT", DisplayName = "Выход", ZoneId = "ZONE_CAPSULE",
                    Type = "trigger", Position = new Vec3Data(-7, 0.5f, -6), InteractionRadius = 1.5f,
                    HintText = "Выйти", RequiredPhase = "Awakening",
                },
            ],
        };

        var zones = A0ContentMapper.MapZones(zoneData, objData);

        Assert.Equal(2, zones.Count);
        Assert.Single(zones[0].Objects); // ZONE_CAPSULE has capsule_exit
        Assert.Empty(zones[1].Objects);  // ZONE_BIOMETRIC has no objects in this data
        Assert.Equal("capsule_exit", zones[0].Objects[0].Id);
    }

    [Fact]
    public void MapZones_SetsCorrectBounds()
    {
        var zoneData = new ZoneFileData
        {
            SceneId = "test",
            Zones = [new ZoneData { ZoneId = "ZONE_CORE", DisplayName = "C.O.R.E.", Position = new Vec3Data(0, 0, 0), Size = new SizeData(6, 6, 6), RequiredPhase = "Activation" }],
        };
        var objData = new ObjectFileData { SceneId = "test", Objects = [] };

        var zones = A0ContentMapper.MapZones(zoneData, objData);

        var bounds = zones[0].Bounds;
        Assert.Equal(-3f, bounds.X);
        Assert.Equal(0f, bounds.Y);
        Assert.Equal(-3f, bounds.Z);
        Assert.Equal(6f, bounds.Width);
        Assert.Equal(6f, bounds.Height);
        Assert.Equal(6f, bounds.Depth);
    }

    // ═══════════════════════════════════════════════════
    // PrologueTracker — visit tracking
    // ═══════════════════════════════════════════════════

    [Fact]
    public void PrologueTracker_StartsEmpty()
    {
        var tracker = new PrologueTracker(Logger);

        Assert.Empty(tracker.VisitedObjects);
        Assert.Equal(0, tracker.VisitCount);
        Assert.False(tracker.IsProtocolZeroUnlocked);
        Assert.Equal(0f, tracker.CompletionRatio);
    }

    [Fact]
    public void PrologueTracker_RecordsVisit()
    {
        var tracker = new PrologueTracker(Logger);

        tracker.RecordVisit("capsule_exit");

        Assert.True(tracker.HasVisited("capsule_exit"));
        Assert.False(tracker.HasVisited("bio_scanner"));
        Assert.Equal(1, tracker.VisitCount);
    }

    [Fact]
    public void PrologueTracker_IgnoresDuplicateVisit()
    {
        var tracker = new PrologueTracker(Logger);

        tracker.RecordVisit("capsule_exit");
        tracker.RecordVisit("capsule_exit");

        Assert.Equal(1, tracker.VisitCount);
    }

    [Fact]
    public void PrologueTracker_MandatoryCompleted_TracksCorrectly()
    {
        var tracker = new PrologueTracker(Logger);

        tracker.RecordVisit("capsule_exit");
        tracker.RecordVisit("bio_scanner");
        tracker.RecordVisit("supply_terminal");

        Assert.Equal(3, tracker.MandatoryCompleted);
        Assert.Equal(0, tracker.ExplorationCompleted);
    }

    [Fact]
    public void PrologueTracker_ProtocolZero_NotUnlockedWithOnlyMandatory()
    {
        var tracker = new PrologueTracker(Logger);

        foreach (var obj in new[] { "capsule_exit", "bio_scanner", "supply_terminal", "drone_dock", "core_console" })
            tracker.RecordVisit(obj);

        Assert.Equal(5, tracker.MandatoryCompleted);
        Assert.False(tracker.IsProtocolZeroUnlocked);
    }

    [Fact]
    public void PrologueTracker_ProtocolZero_UnlockedWithAllVisits()
    {
        var tracker = new PrologueTracker(Logger);

        foreach (var obj in new[] { "capsule_exit", "bio_scanner", "supply_terminal", "drone_dock", "core_console", "op_terminal", "gallery_overlook" })
            tracker.RecordVisit(obj);

        Assert.True(tracker.IsProtocolZeroUnlocked);
        Assert.Equal(1.0f, tracker.CompletionRatio);
    }

    [Fact]
    public void PrologueTracker_CompletionRatio_Partial()
    {
        var tracker = new PrologueTracker(Logger);

        // 5 mandatory + 2 exploration = 7 total
        tracker.RecordVisit("capsule_exit");
        tracker.RecordVisit("bio_scanner");

        Assert.Equal(2f / 7f, tracker.CompletionRatio, 0.01f);
    }

    // ═══════════════════════════════════════════════════
    // OperatorIdentity
    // ═══════════════════════════════════════════════════

    [Fact]
    public void OperatorIdentity_Defaults()
    {
        var identity = new OperatorIdentity();

        Assert.Equal("Алан Арквейн", identity.Name);
        Assert.Equal("Alan Arcwain", identity.NameLatin);
        Assert.Equal(0, identity.ClearanceLevel);
        Assert.NotNull(identity.Profile);
        Assert.Equal(1, identity.Profile.Level);
        Assert.Equal(10, identity.Profile.GetStat(StatType.Analysis));
    }

    [Fact]
    public void OperatorIdentity_InteractionXp_ContainsAllObjects()
    {
        Assert.Equal(9, OperatorIdentity.InteractionXp.Count);
        Assert.Equal(10, OperatorIdentity.InteractionXp["capsule_exit"]);
        Assert.Equal(50, OperatorIdentity.InteractionXp["core_console"]);
        Assert.Equal(5, OperatorIdentity.InteractionXp["archive_gate"]);
    }

    // ═══════════════════════════════════════════════════
    // SaveGame — new prologue fields
    // ═══════════════════════════════════════════════════

    [Fact]
    public void SaveGame_DefaultPrologueState()
    {
        var save = new SaveGame();

        Assert.Equal("Awakening", save.CurrentPhase);
        Assert.Empty(save.VisitedObjects);
        Assert.Empty(save.InventoryItemIds);
        Assert.Empty(save.CompletedObjectiveIds);
        Assert.Null(save.ActiveObjectiveId);
        Assert.Equal(1, save.OperatorLevel);
        Assert.Equal(0, save.OperatorXp);
        Assert.False(save.ProtocolZeroUnlocked);
    }

    [Fact]
    public void SaveGame_WithPrologueState()
    {
        var save = new SaveGame
        {
            CurrentPhase = "OperationAccess",
            VisitedObjects = ["capsule_exit", "bio_scanner"],
            InventoryItemIds = ["ITEM_ARCHIVE_BADGE"],
            CompletedObjectiveIds = ["UI_OBJ_EXIT_CAPSULE"],
            ActiveObjectiveId = "UI_OBJ_BIOMETRIC",
            OperatorLevel = 2,
            OperatorXp = 120,
            ProtocolZeroUnlocked = true,
        };

        Assert.Equal("OperationAccess", save.CurrentPhase);
        Assert.Equal(2, save.VisitedObjects.Count);
        Assert.Single(save.InventoryItemIds);
        Assert.Equal(2, save.OperatorLevel);
        Assert.True(save.ProtocolZeroUnlocked);
    }

    // ═══════════════════════════════════════════════════
    // A0ContentProvider — integration with JSON files
    // ═══════════════════════════════════════════════════

    private static string? FindContentRoot()
    {
        var dir = Directory.GetCurrentDirectory();
        while (dir is not null)
        {
            var contentDir = Path.Combine(dir, "Content");
            if (Directory.Exists(contentDir) && File.Exists(Path.Combine(contentDir, "Zones", "A0_Zones.json")))
                return contentDir;
            dir = Directory.GetParent(dir)?.FullName;
        }
        return null;
    }

    [Fact]
    public void A0ContentProvider_LoadZones_Returns11Zones()
    {
        var contentRoot = FindContentRoot();
        if (contentRoot is null) return; // skip if running outside workspace

        var provider = new A0ContentProvider(contentRoot);
        var zones = provider.LoadZones();

        Assert.Equal(11, zones.Count);
        Assert.Contains(zones, z => z.Id == HubZoneId.Capsule);
        Assert.Contains(zones, z => z.Id == HubZoneId.Core);
        Assert.Contains(zones, z => z.Id == HubZoneId.HardArchiveEntrance);
    }

    [Fact]
    public void A0ContentProvider_LoadZones_HasCorrectObjectCount()
    {
        var contentRoot = FindContentRoot();
        if (contentRoot is null) return;

        var provider = new A0ContentProvider(contentRoot);
        var zones = provider.LoadZones();

        var totalObjects = zones.Sum(z => z.Objects.Count);
        Assert.Equal(9, totalObjects);
    }

    [Fact]
    public void A0ContentProvider_LoadZones_AllObjectsHaveModelId()
    {
        var contentRoot = FindContentRoot();
        if (contentRoot is null) return;

        var provider = new A0ContentProvider(contentRoot);
        var zones = provider.LoadZones();

        var missing = zones
            .SelectMany(z => z.Objects)
            .Where(o => string.IsNullOrWhiteSpace(o.ModelId))
            .Select(o => o.Id)
            .ToList();

        Assert.Empty(missing);
    }

    [Fact]
    public void A0ContentProvider_LoadZones_ModelIdsExistInRegistryConstants()
    {
        var contentRoot = FindContentRoot();
        if (contentRoot is null) return;

        var provider = new A0ContentProvider(contentRoot);
        var zones = provider.LoadZones();

        var knownModelIds = typeof(ContourModelIds)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
            .Select(f => (string?)f.GetRawConstantValue())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Cast<string>()
            .ToHashSet(StringComparer.Ordinal);

        var unknown = zones
            .SelectMany(z => z.Objects)
            .Where(o => !string.IsNullOrWhiteSpace(o.ModelId) && !knownModelIds.Contains(o.ModelId!))
            .Select(o => $"{o.Id}:{o.ModelId}")
            .ToList();

        Assert.Empty(unknown);
    }

    [Fact]
    public void ContourModelIds_AllConstants_AreUnique()
    {
        var modelIds = typeof(ContourModelIds)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
            .Select(f => (string?)f.GetRawConstantValue())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Cast<string>()
            .ToList();

        Assert.Equal(modelIds.Count, modelIds.Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void ContourModelIds_ContainsRequiredExtraNodeEntries()
    {
        var required = new[]
        {
            ContourModelIds.CommerceHall,
            ContourModelIds.TechHall,
            ContourModelIds.ArchivePreview,
            ContourModelIds.ArchiveCorridor,
            ContourModelIds.EntryOctagon,
            ContourModelIds.IndexVestibule,
            ContourModelIds.ResearchRoom01,
            ContourModelIds.StackRingPreview,
            ContourModelIds.CommerceDesk,
            ContourModelIds.ToolBench,
            ContourModelIds.ResearchLab,
            ContourModelIds.ArchiveControl,
            ContourModelIds.MissionBoard,
        };

        Assert.All(required, modelId => Assert.False(string.IsNullOrWhiteSpace(modelId)));
    }

    [Fact]
    public void A0ContentProvider_LoadDialogues_Returns6Dialogues()
    {
        var contentRoot = FindContentRoot();
        if (contentRoot is null) return;

        var provider = new A0ContentProvider(contentRoot);
        var dialogues = provider.LoadDialogues();

        Assert.Equal(6, dialogues.Count);
        Assert.Contains(dialogues, d => d.DialogueId == "DIA_DRONE_GREETING");
        Assert.Contains(dialogues, d => d.DialogueId == "DIA_CORE_ACTIVATION");
    }

    [Fact]
    public void A0ContentProvider_LoadDialogues_DroneHas3Lines()
    {
        var contentRoot = FindContentRoot();
        if (contentRoot is null) return;

        var provider = new A0ContentProvider(contentRoot);
        var dialogues = provider.LoadDialogues();
        var drone = dialogues.First(d => d.DialogueId == "DIA_DRONE_GREETING");

        Assert.Equal(3, drone.Lines.Count);
        Assert.Equal("NPC_ARCHIVE_DRONE", drone.Lines[0].Speaker);
    }

    [Fact]
    public void A0ContentProvider_LoadTriggers_ReturnsPhaseChangeTriggers()
    {
        var contentRoot = FindContentRoot();
        if (contentRoot is null) return;

        var provider = new A0ContentProvider(contentRoot);
        var triggers = provider.LoadTriggers();

        // 5 phase_change triggers: Identification, Provisioning, DroneContact, Activation, OperationAccess
        Assert.Equal(5, triggers.Count);
        Assert.Contains(triggers, t => t.Phase == HubRhythmPhase.Identification);
        Assert.Contains(triggers, t => t.Phase == HubRhythmPhase.OperationAccess);
    }

    [Fact]
    public void A0ContentProvider_LoadObjectives_Returns7Objectives()
    {
        var contentRoot = FindContentRoot();
        if (contentRoot is null) return;

        var provider = new A0ContentProvider(contentRoot);
        var objectives = provider.LoadObjectives();

        Assert.Equal(7, objectives.Count);
        Assert.Equal(1, objectives[0].Order);
        Assert.Equal(7, objectives[^1].Order);
    }

    [Fact]
    public void A0ContentProvider_LoadScene_ReturnsCorrectScene()
    {
        var contentRoot = FindContentRoot();
        if (contentRoot is null) return;

        var provider = new A0ContentProvider(contentRoot);
        var scene = provider.LoadScene();

        Assert.Equal("SCN_A0_INITIATION", scene.SceneId);
        Assert.Equal("octagonal", scene.Shape);
        Assert.Equal(22f, scene.Dimensions.Width);
        Assert.Equal(11f, scene.Dimensions.Height);
        Assert.Equal(2, scene.CameraProfiles.Count);
    }

    // ═══════════════════════════════════════════════════
    // Integration: Content → Runtime pipeline
    // ═══════════════════════════════════════════════════

    [Fact]
    public void ContentPipeline_ZonesFromJSON_WorkWithHubRuntime()
    {
        var contentRoot = FindContentRoot();
        if (contentRoot is null) return;

        var provider = new A0ContentProvider(contentRoot);
        var zones = provider.LoadZones();
        var journal = new EventJournal();
        var runtime = new HubA0Runtime(zones, journal, Logger);

        Assert.Equal(HubRhythmPhase.Awakening, runtime.CurrentPhase);
        var available = runtime.GetAvailableInteractables();
        Assert.Single(available);
        Assert.Equal("capsule_exit", available[0].Id);
    }

    [Fact]
    public void ContentPipeline_DialoguesFromJSON_WorkWithDialoguePlayer()
    {
        var contentRoot = FindContentRoot();
        if (contentRoot is null) return;

        var provider = new A0ContentProvider(contentRoot);
        var dialogues = provider.LoadDialogues();
        var player = new DialoguePlayer();

        foreach (var d in dialogues)
            player.Register(d);

        player.Start("DIA_DRONE_GREETING");
        Assert.True(player.IsPlaying);
        Assert.NotNull(player.CurrentLine);
        Assert.Contains("дрон", player.CurrentLine!.Text, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ContentPipeline_TriggersFromJSON_WorkWithTriggerSystem()
    {
        var contentRoot = FindContentRoot();
        if (contentRoot is null) return;

        var provider = new A0ContentProvider(contentRoot);
        var triggers = provider.LoadTriggers();
        var system = new TriggerSystem(Logger);

        foreach (var t in triggers)
            system.Register(t);

        var result = system.OnPhaseChanged(HubRhythmPhase.Identification);
        Assert.True(result.FiredActions.Count > 0);
    }

    [Fact]
    public void ContentPipeline_ObjectivesFromJSON_WorkWithObjectiveTracker()
    {
        var contentRoot = FindContentRoot();
        if (contentRoot is null) return;

        var provider = new A0ContentProvider(contentRoot);
        var objectives = provider.LoadObjectives();
        var tracker = new ObjectiveTracker();

        foreach (var o in objectives)
            tracker.Register(o);

        tracker.SetActive("UI_OBJ_EXIT_CAPSULE");
        var active = tracker.GetActive();
        Assert.NotNull(active);
        Assert.Equal("UI_OBJ_EXIT_CAPSULE", active!.ObjectiveId);
    }

    [Fact]
    public void ContentPipeline_FullPrologue_WorksEndToEnd()
    {
        var contentRoot = FindContentRoot();
        if (contentRoot is null) return;

        var provider = new A0ContentProvider(contentRoot);
        var zones = provider.LoadZones();
        var dialogues = provider.LoadDialogues();
        var triggers = provider.LoadTriggers();
        var objectives = provider.LoadObjectives();

        var journal = new EventJournal();
        var hubRuntime = new HubA0Runtime(zones, journal, Logger);
        var dialoguePlayer = new DialoguePlayer();
        var objectiveTracker = new ObjectiveTracker();
        var triggerSystem = new TriggerSystem(Logger);
        var inventory = new PlayerInventory();
        var prologueTracker = new PrologueTracker(Logger);

        foreach (var d in dialogues) dialoguePlayer.Register(d);
        foreach (var t in triggers) triggerSystem.Register(t);
        foreach (var o in objectives) objectiveTracker.Register(o);
        objectiveTracker.SetActive("UI_OBJ_EXIT_CAPSULE");

        var handler = new InteractionHandler(dialoguePlayer, objectiveTracker, triggerSystem, inventory, journal, Logger);
        handler.RegisterItemGrant("supply_terminal",
        [
            new InventoryItem { ItemId = "ITEM_ARCHIVE_BADGE", Name = "Жетон Архива", Type = ItemType.KeyItem },
            new InventoryItem { ItemId = "ITEM_BASIC_SCANNER", Name = "Базовый сканер", Type = ItemType.Tool },
            new InventoryItem { ItemId = "ITEM_FIELD_RATION", Name = "Полевой рацион", Type = ItemType.Consumable },
        ]);

        // Walk through mandatory sequence
        foreach (var objId in new[] { "capsule_exit", "bio_scanner", "supply_terminal", "drone_dock", "core_console" })
        {
            var obj = zones.SelectMany(z => z.Objects).First(o => o.Id == objId);
            var baseResult = hubRuntime.Interact(objId);
            handler.Handle(obj, baseResult);
            prologueTracker.RecordVisit(objId);
        }

        Assert.Equal(HubRhythmPhase.OperationAccess, hubRuntime.CurrentPhase);
        Assert.Equal(3, inventory.Items.Count);
        Assert.False(prologueTracker.IsProtocolZeroUnlocked);

        // Visit exploration objects
        foreach (var objId in new[] { "op_terminal", "gallery_overlook" })
        {
            var obj = zones.SelectMany(z => z.Objects).First(o => o.Id == objId);
            var baseResult = hubRuntime.Interact(objId);
            handler.Handle(obj, baseResult);
            prologueTracker.RecordVisit(objId);
        }

        Assert.True(prologueTracker.IsProtocolZeroUnlocked);
        Assert.Equal(1.0f, prologueTracker.CompletionRatio);
    }
}
