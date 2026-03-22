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

public class Session4InteractivesTests
{
    // ── Helpers ──

    private sealed class NullLogger : ILogger
    {
        public void Info(string message) { }
        public void Warn(string message) { }
        public void Error(string message) { }
    }

    private static NullLogger Logger => new();

    private static HubA0Runtime MakeHubRuntime()
    {
        var zones = HubA0SceneBuilder.Build();
        var journal = new EventJournal();
        return new HubA0Runtime(zones, journal, Logger);
    }

    private static PlayerInventory MakeInventory() => new();

    private static DialoguePlayer MakeDialoguePlayer()
    {
        var dp = new DialoguePlayer();
        dp.Register(new InlineDialogue
        {
            DialogueId = "DIA_DRONE_GREETING",
            SpeakerDisplayName = "Архивный дрон",
            Lines =
            [
                new InlineDialogueLine { LineId = "L1", Speaker = "NPC_ARCHIVE_DRONE", Text = "Инициализация дрона..." },
                new InlineDialogueLine { LineId = "L2", Speaker = "NPC_ARCHIVE_DRONE", Text = "Оператор Арквейн." },
                new InlineDialogueLine { LineId = "L3", Speaker = "NPC_ARCHIVE_DRONE", Text = "Ваш допуск: начальный." },
            ],
        });
        dp.Register(new InlineDialogue
        {
            DialogueId = "DIA_CORE_ACTIVATION",
            SpeakerDisplayName = "C.O.R.E.",
            Lines =
            [
                new InlineDialogueLine { LineId = "C1", Speaker = "SYSTEM_CORE", Text = "C.O.R.E. — Центральная Операционная..." },
                new InlineDialogueLine { LineId = "C2", Speaker = "SYSTEM_CORE", Text = "Оператор идентифицирован." },
            ],
        });
        dp.Register(new InlineDialogue
        {
            DialogueId = "DIA_BIO_CONFIRM",
            SpeakerDisplayName = "Система",
            Lines =
            [
                new InlineDialogueLine { LineId = "B1", Speaker = "SYSTEM", Text = "Сканирование..." },
                new InlineDialogueLine { LineId = "B2", Speaker = "SYSTEM", Text = "Оператор: Алан Арквейн." },
            ],
        });
        dp.Register(new InlineDialogue
        {
            DialogueId = "DIA_SUPPLY_GRANTED",
            SpeakerDisplayName = "Логистика",
            Lines =
            [
                new InlineDialogueLine { LineId = "S1", Speaker = "SYSTEM", Text = "Комплект снаряжения подготовлен." },
                new InlineDialogueLine { LineId = "S2", Speaker = "SYSTEM", Text = "Заберите предметы." },
            ],
        });
        dp.Register(new InlineDialogue
        {
            DialogueId = "DIA_ARCHIVE_LOCKED",
            SpeakerDisplayName = "Система безопасности",
            Lines =
            [
                new InlineDialogueLine { LineId = "AL1", Speaker = "SYSTEM", Text = "Врата заблокированы." },
            ],
        });
        dp.Register(new InlineDialogue
        {
            DialogueId = "DIA_GALLERY_OVERLOOK",
            SpeakerDisplayName = "",
            Lines =
            [
                new InlineDialogueLine { LineId = "G1", Speaker = "NARRATOR", Text = "Перед тобой — шахта." },
            ],
        });
        return dp;
    }

    private static ObjectiveTracker MakeObjectiveTracker()
    {
        var tracker = new ObjectiveTracker();
        tracker.Register(new Objective { ObjectiveId = "UI_OBJ_EXIT_CAPSULE", Text = "Выйти из капсулы", Order = 1 });
        tracker.Register(new Objective { ObjectiveId = "UI_OBJ_BIOMETRIC", Text = "Пройти идентификацию", Order = 2 });
        tracker.Register(new Objective { ObjectiveId = "UI_OBJ_LOGISTICS", Text = "Получить снаряжение", Order = 3 });
        tracker.Register(new Objective { ObjectiveId = "UI_OBJ_DRONE", Text = "Активировать дрона", Order = 4 });
        tracker.Register(new Objective { ObjectiveId = "UI_OBJ_CORE", Text = "Обратиться к C.O.R.E.", Order = 5 });
        tracker.Register(new Objective { ObjectiveId = "UI_OBJ_EXPLORE", Text = "Осмотреть терминал и галерею", Order = 6 });
        tracker.Register(new Objective { ObjectiveId = "UI_OBJ_PROLOGUE_DONE", Text = "Пролог завершён", Order = 7 });
        return tracker;
    }

    private static TriggerSystem MakeTriggerSystem()
    {
        var ts = new TriggerSystem(Logger);
        ts.Register(new PhaseTrigger
        {
            TriggerId = "TRG_PHASE_IDENTIFICATION",
            Phase = HubRhythmPhase.Identification,
            Actions =
            [
                new TriggerAction { Type = TriggerActionType.SetObjective, ObjectiveId = "UI_OBJ_BIOMETRIC" },
                new TriggerAction { Type = TriggerActionType.JournalEntry, Text = "Капсула открыта." },
            ],
        });
        ts.Register(new PhaseTrigger
        {
            TriggerId = "TRG_PHASE_PROVISIONING",
            Phase = HubRhythmPhase.Provisioning,
            Actions =
            [
                new TriggerAction { Type = TriggerActionType.SetObjective, ObjectiveId = "UI_OBJ_LOGISTICS" },
                new TriggerAction { Type = TriggerActionType.PlayDialogue, DialogueId = "DIA_BIO_CONFIRM" },
                new TriggerAction { Type = TriggerActionType.JournalEntry, Text = "Идентификация подтверждена." },
            ],
        });
        ts.Register(new PhaseTrigger
        {
            TriggerId = "TRG_PHASE_DRONE",
            Phase = HubRhythmPhase.DroneContact,
            Actions =
            [
                new TriggerAction { Type = TriggerActionType.SetObjective, ObjectiveId = "UI_OBJ_DRONE" },
                new TriggerAction { Type = TriggerActionType.PlayDialogue, DialogueId = "DIA_SUPPLY_GRANTED" },
                new TriggerAction { Type = TriggerActionType.JournalEntry, Text = "Снаряжение получено." },
            ],
        });
        ts.Register(new PhaseTrigger
        {
            TriggerId = "TRG_PHASE_ACTIVATION",
            Phase = HubRhythmPhase.Activation,
            Actions =
            [
                new TriggerAction { Type = TriggerActionType.SetObjective, ObjectiveId = "UI_OBJ_CORE" },
                new TriggerAction { Type = TriggerActionType.JournalEntry, Text = "Дрон активирован." },
            ],
        });
        ts.Register(new PhaseTrigger
        {
            TriggerId = "TRG_PHASE_OPERATION_ACCESS",
            Phase = HubRhythmPhase.OperationAccess,
            Actions =
            [
                new TriggerAction { Type = TriggerActionType.SetObjective, ObjectiveId = "UI_OBJ_EXPLORE" },
                new TriggerAction { Type = TriggerActionType.PlayDialogue, DialogueId = "DIA_CORE_ACTIVATION" },
                new TriggerAction { Type = TriggerActionType.JournalEntry, Text = "C.O.R.E. активирован." },
            ],
        });
        return ts;
    }

    private static List<InventoryItem> StarterItems =>
    [
        new() { ItemId = "ITEM_ARCHIVE_BADGE", Name = "Жетон Архива", Type = ItemType.KeyItem },
        new() { ItemId = "ITEM_BASIC_SCANNER", Name = "Базовый сканер", Type = ItemType.Tool },
        new() { ItemId = "ITEM_FIELD_RATION", Name = "Полевой рацион", Type = ItemType.Consumable },
    ];

    private static InteractionHandler MakeHandler(
        DialoguePlayer? dp = null, ObjectiveTracker? ot = null,
        TriggerSystem? ts = null, PlayerInventory? inv = null,
        EventJournal? journal = null)
    {
        dp ??= MakeDialoguePlayer();
        ot ??= MakeObjectiveTracker();
        ts ??= MakeTriggerSystem();
        inv ??= MakeInventory();
        journal ??= new EventJournal();

        var handler = new InteractionHandler(dp, ot, ts, inv, journal, Logger);
        handler.RegisterItemGrant("supply_terminal", StarterItems);
        return handler;
    }

    // ═══════════════════════════════════════════════════════════════
    //  1. InteractiveType Enum
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void InteractiveType_HasFourValues()
    {
        var values = Enum.GetValues<InteractiveType>();
        Assert.Equal(4, values.Length);
    }

    [Theory]
    [InlineData(InteractiveType.Trigger, 0)]
    [InlineData(InteractiveType.Terminal, 1)]
    [InlineData(InteractiveType.Npc, 2)]
    [InlineData(InteractiveType.Gate, 3)]
    public void InteractiveType_ValuesMatch(InteractiveType type, int expected)
    {
        Assert.Equal(expected, (int)type);
    }

    // ═══════════════════════════════════════════════════════════════
    //  2. InventoryItem + PlayerInventory
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void InventoryItem_HasProperties()
    {
        var item = new InventoryItem { ItemId = "ITEM_ARCHIVE_BADGE", Name = "Жетон Архива", Type = ItemType.KeyItem };
        Assert.Equal("ITEM_ARCHIVE_BADGE", item.ItemId);
        Assert.Equal("Жетон Архива", item.Name);
        Assert.Equal(ItemType.KeyItem, item.Type);
    }

    [Fact]
    public void PlayerInventory_Add_And_Has()
    {
        var inv = MakeInventory();
        var item = new InventoryItem { ItemId = "ITEM_TEST", Name = "Test", Type = ItemType.Tool };
        inv.Add(item);
        Assert.True(inv.Has("ITEM_TEST"));
        Assert.False(inv.Has("ITEM_NONEXISTENT"));
    }

    [Fact]
    public void PlayerInventory_Add_Duplicate_Ignored()
    {
        var inv = MakeInventory();
        var item = new InventoryItem { ItemId = "ITEM_TEST", Name = "Test", Type = ItemType.Tool };
        inv.Add(item);
        inv.Add(item);
        Assert.Single(inv.Items);
    }

    [Fact]
    public void PlayerInventory_Remove()
    {
        var inv = MakeInventory();
        inv.Add(new InventoryItem { ItemId = "ITEM_A", Name = "A", Type = ItemType.Consumable });
        Assert.True(inv.Remove("ITEM_A"));
        Assert.False(inv.Has("ITEM_A"));
        Assert.False(inv.Remove("ITEM_A"));
    }

    [Fact]
    public void PlayerInventory_GetAll_ReturnsAll()
    {
        var inv = MakeInventory();
        foreach (var item in StarterItems) inv.Add(item);
        Assert.Equal(3, inv.GetAll().Count);
    }

    [Fact]
    public void ItemType_HasFourValues()
    {
        Assert.Equal(4, Enum.GetValues<ItemType>().Length);
    }

    // ═══════════════════════════════════════════════════════════════
    //  3. TerminalScreen
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void TerminalScreen_HasProperties()
    {
        var screen = new TerminalScreen
        {
            TerminalId = "bio_scanner",
            Title = "Биосканер",
            Lines = ["Сканирование...", "Идентификация подтверждена."],
            DialogueId = "DIA_BIO_CONFIRM",
        };
        Assert.Equal("bio_scanner", screen.TerminalId);
        Assert.Equal(2, screen.Lines.Count);
        Assert.Equal("DIA_BIO_CONFIRM", screen.DialogueId);
    }

    [Fact]
    public void TerminalScreen_DialogueId_Optional()
    {
        var screen = new TerminalScreen
        {
            TerminalId = "op_terminal",
            Title = "Терминал операций",
            Lines = ["Журнал операций"],
        };
        Assert.Null(screen.DialogueId);
    }

    // ═══════════════════════════════════════════════════════════════
    //  4. ObjectiveTracker
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void ObjectiveTracker_Register_And_GetAll()
    {
        var tracker = MakeObjectiveTracker();
        Assert.Equal(7, tracker.GetAll().Count);
    }

    [Fact]
    public void ObjectiveTracker_SetActive_Works()
    {
        var tracker = MakeObjectiveTracker();
        tracker.SetActive("UI_OBJ_EXIT_CAPSULE");
        var active = tracker.GetActive();
        Assert.NotNull(active);
        Assert.Equal("UI_OBJ_EXIT_CAPSULE", active.ObjectiveId);
        Assert.Equal(ObjectiveStatus.Active, active.Status);
    }

    [Fact]
    public void ObjectiveTracker_Complete_Works()
    {
        var tracker = MakeObjectiveTracker();
        tracker.SetActive("UI_OBJ_EXIT_CAPSULE");
        tracker.Complete("UI_OBJ_EXIT_CAPSULE");
        Assert.Null(tracker.GetActive());
        Assert.Single(tracker.GetCompleted());
    }

    [Fact]
    public void ObjectiveTracker_Cannot_Complete_Locked()
    {
        var tracker = MakeObjectiveTracker();
        tracker.Complete("UI_OBJ_EXIT_CAPSULE"); // not yet active
        Assert.Equal(ObjectiveStatus.Locked, tracker.Objectives["UI_OBJ_EXIT_CAPSULE"].Status);
    }

    [Fact]
    public void ObjectiveTracker_Cannot_Activate_Already_Active()
    {
        var tracker = MakeObjectiveTracker();
        tracker.SetActive("UI_OBJ_EXIT_CAPSULE");
        tracker.Complete("UI_OBJ_EXIT_CAPSULE");
        tracker.SetActive("UI_OBJ_EXIT_CAPSULE"); // already completed
        Assert.Equal(ObjectiveStatus.Completed, tracker.Objectives["UI_OBJ_EXIT_CAPSULE"].Status);
    }

    [Fact]
    public void ObjectiveTracker_GetAll_Ordered()
    {
        var tracker = MakeObjectiveTracker();
        var all = tracker.GetAll();
        for (int i = 1; i < all.Count; i++)
            Assert.True(all[i - 1].Order < all[i].Order);
    }

    // ═══════════════════════════════════════════════════════════════
    //  5. DialoguePlayer
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void DialoguePlayer_Start_ReturnsFirstLine()
    {
        var dp = MakeDialoguePlayer();
        Assert.True(dp.Start("DIA_DRONE_GREETING"));
        Assert.True(dp.IsPlaying);
        Assert.NotNull(dp.CurrentLine);
        Assert.Equal("L1", dp.CurrentLine.LineId);
    }

    [Fact]
    public void DialoguePlayer_Advance_ThroughAllLines()
    {
        var dp = MakeDialoguePlayer();
        dp.Start("DIA_DRONE_GREETING");

        var line2 = dp.Advance();
        Assert.NotNull(line2);
        Assert.Equal("L2", line2.LineId);

        var line3 = dp.Advance();
        Assert.NotNull(line3);
        Assert.Equal("L3", line3.LineId);

        var end = dp.Advance();
        Assert.Null(end);
        Assert.True(dp.IsComplete);
        Assert.False(dp.IsPlaying);
    }

    [Fact]
    public void DialoguePlayer_Start_UnknownId_ReturnsFalse()
    {
        var dp = MakeDialoguePlayer();
        Assert.False(dp.Start("NONEXISTENT"));
        Assert.False(dp.IsPlaying);
    }

    [Fact]
    public void DialoguePlayer_Stop_ClearsState()
    {
        var dp = MakeDialoguePlayer();
        dp.Start("DIA_DRONE_GREETING");
        dp.Stop();
        Assert.False(dp.IsPlaying);
        Assert.Null(dp.CurrentLine);
    }

    [Fact]
    public void DialoguePlayer_GetTranscript()
    {
        var dp = MakeDialoguePlayer();
        var transcript = dp.GetTranscript("DIA_DRONE_GREETING");
        Assert.Equal(3, transcript.Count);
    }

    [Fact]
    public void DialoguePlayer_CurrentDialogueId()
    {
        var dp = MakeDialoguePlayer();
        dp.Start("DIA_CORE_ACTIVATION");
        Assert.Equal("DIA_CORE_ACTIVATION", dp.CurrentDialogueId);
    }

    // ═══════════════════════════════════════════════════════════════
    //  6. TriggerSystem
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void TriggerSystem_OnPhaseChanged_FiresActions()
    {
        var ts = MakeTriggerSystem();
        var result = ts.OnPhaseChanged(HubRhythmPhase.Identification);
        Assert.Equal(2, result.FiredActions.Count);
        Assert.Equal(TriggerActionType.SetObjective, result.FiredActions[0].Type);
    }

    [Fact]
    public void TriggerSystem_OnPhaseChanged_FiresOnlyOnce()
    {
        var ts = MakeTriggerSystem();
        ts.OnPhaseChanged(HubRhythmPhase.Identification);
        var second = ts.OnPhaseChanged(HubRhythmPhase.Identification);
        Assert.Empty(second.FiredActions);
    }

    [Fact]
    public void TriggerSystem_HasFired_TracksState()
    {
        var ts = MakeTriggerSystem();
        Assert.False(ts.HasFired("TRG_PHASE_IDENTIFICATION"));
        ts.OnPhaseChanged(HubRhythmPhase.Identification);
        Assert.True(ts.HasFired("TRG_PHASE_IDENTIFICATION"));
    }

    [Fact]
    public void TriggerSystem_UnregisteredPhase_ReturnsEmpty()
    {
        var ts = new TriggerSystem(Logger);
        var result = ts.OnPhaseChanged(HubRhythmPhase.Awakening);
        Assert.Empty(result.FiredActions);
    }

    [Fact]
    public void TriggerSystem_MultiplePhases_Sequential()
    {
        var ts = MakeTriggerSystem();
        var r1 = ts.OnPhaseChanged(HubRhythmPhase.Identification);
        var r2 = ts.OnPhaseChanged(HubRhythmPhase.Provisioning);
        Assert.Equal(2, r1.FiredActions.Count);
        Assert.Equal(3, r2.FiredActions.Count); // includes PlayDialogue
    }

    // ═══════════════════════════════════════════════════════════════
    //  7. InteractableObject — new properties
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void InteractableObject_InteractiveType_Default_IsTrigger()
    {
        var obj = new InteractableObject
        {
            Id = "test", DisplayName = "Test", HintText = "Hint",
            Zone = HubZoneId.Core, Position = Vec3.Zero,
            Bounds = new Bounds3D(0, 0, 0, 1, 1, 1),
            RequiredPhase = HubRhythmPhase.Awakening,
        };
        Assert.Equal(InteractiveType.Trigger, obj.InteractiveType);
    }

    [Fact]
    public void InteractableObject_CanSetInteractiveType()
    {
        var obj = new InteractableObject
        {
            Id = "drone", DisplayName = "Drone", HintText = "Talk",
            Zone = HubZoneId.DroneNiche, Position = Vec3.Zero,
            Bounds = new Bounds3D(0, 0, 0, 1, 1, 1),
            RequiredPhase = HubRhythmPhase.Awakening,
            InteractiveType = InteractiveType.Npc,
            DialogueId = "DIA_DRONE_GREETING",
        };
        Assert.Equal(InteractiveType.Npc, obj.InteractiveType);
        Assert.Equal("DIA_DRONE_GREETING", obj.DialogueId);
    }

    [Fact]
    public void InteractableObject_GrantsItems_FromBuilder()
    {
        var zones = HubA0SceneBuilder.Build();
        var supply = zones.SelectMany(z => z.Objects).First(o => o.Id == "supply_terminal");
        Assert.NotNull(supply.GrantsItems);
        Assert.Equal(3, supply.GrantsItems.Count);
        Assert.Contains("ITEM_ARCHIVE_BADGE", supply.GrantsItems);
    }

    [Fact]
    public void InteractableObject_LockedMessage_FromBuilder()
    {
        var zones = HubA0SceneBuilder.Build();
        var gate = zones.SelectMany(z => z.Objects).First(o => o.Id == "archive_gate");
        Assert.Equal(InteractiveType.Gate, gate.InteractiveType);
        Assert.NotNull(gate.LockedMessage);
    }

    // ═══════════════════════════════════════════════════════════════
    //  8. HubA0SceneBuilder — types assigned correctly
    // ═══════════════════════════════════════════════════════════════

    [Theory]
    [InlineData("capsule_exit", InteractiveType.Trigger)]
    [InlineData("bio_scanner", InteractiveType.Terminal)]
    [InlineData("supply_terminal", InteractiveType.Terminal)]
    [InlineData("drone_dock", InteractiveType.Npc)]
    [InlineData("core_console", InteractiveType.Terminal)]
    [InlineData("op_terminal", InteractiveType.Terminal)]
    [InlineData("research_terminal", InteractiveType.Terminal)]
    [InlineData("gallery_overlook", InteractiveType.Trigger)]
    [InlineData("archive_gate", InteractiveType.Gate)]
    public void Builder_AssignsCorrectType(string objId, InteractiveType expectedType)
    {
        var zones = HubA0SceneBuilder.Build();
        var obj = zones.SelectMany(z => z.Objects).First(o => o.Id == objId);
        Assert.Equal(expectedType, obj.InteractiveType);
    }

    [Theory]
    [InlineData("drone_dock", "DIA_DRONE_GREETING")]
    [InlineData("core_console", "DIA_CORE_ACTIVATION")]
    [InlineData("bio_scanner", "DIA_BIO_CONFIRM")]
    [InlineData("supply_terminal", "DIA_SUPPLY_GRANTED")]
    [InlineData("archive_gate", "DIA_ARCHIVE_LOCKED")]
    [InlineData("gallery_overlook", "DIA_GALLERY_OVERLOOK")]
    public void Builder_AssignsDialogueId(string objId, string expectedDialogueId)
    {
        var zones = HubA0SceneBuilder.Build();
        var obj = zones.SelectMany(z => z.Objects).First(o => o.Id == objId);
        Assert.Equal(expectedDialogueId, obj.DialogueId);
    }

    // ═══════════════════════════════════════════════════════════════
    //  9. InteractionResult — enriched fields
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void InteractionResult_CarriesDialogueId()
    {
        var result = new InteractionResult
        {
            ObjectId = "drone_dock", Success = true, Message = "Test",
            DialogueId = "DIA_DRONE_GREETING",
            ObjectType = InteractiveType.Npc,
        };
        Assert.Equal("DIA_DRONE_GREETING", result.DialogueId);
        Assert.Equal(InteractiveType.Npc, result.ObjectType);
    }

    [Fact]
    public void InteractionResult_CarriesGrantedItems()
    {
        var result = new InteractionResult
        {
            ObjectId = "supply_terminal", Success = true, Message = "Test",
            GrantedItems = StarterItems,
        };
        Assert.Equal(3, result.GrantedItems!.Count);
    }

    [Fact]
    public void InteractionResult_CarriesTerminalScreen()
    {
        var screen = new TerminalScreen { TerminalId = "op_terminal", Title = "Ops", Lines = ["Line1"] };
        var result = new InteractionResult
        {
            ObjectId = "op_terminal", Success = true, Message = "Test",
            TerminalScreen = screen,
        };
        Assert.NotNull(result.TerminalScreen);
        Assert.Equal("op_terminal", result.TerminalScreen.TerminalId);
    }

    // ═══════════════════════════════════════════════════════════════
    //  10. InteractionHandler — type dispatch
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void InteractionHandler_Trigger_NoDialogue()
    {
        var handler = MakeHandler();
        var obj = new InteractableObject
        {
            Id = "capsule_exit", DisplayName = "Exit", HintText = "Exit",
            Zone = HubZoneId.Capsule, Position = Vec3.Zero,
            Bounds = new Bounds3D(0, 0, 0, 1, 1, 1),
            RequiredPhase = HubRhythmPhase.Awakening,
            InteractiveType = InteractiveType.Trigger,
        };
        var baseResult = new InteractionResult { ObjectId = "capsule_exit", Success = true, Message = "Exited" };
        var result = handler.Handle(obj, baseResult);
        Assert.Null(result.DialogueId);
        Assert.Equal(InteractiveType.Trigger, result.ObjectType);
    }

    [Fact]
    public void InteractionHandler_Npc_StartsDialogue()
    {
        var dp = MakeDialoguePlayer();
        var handler = MakeHandler(dp: dp);
        var obj = new InteractableObject
        {
            Id = "drone_dock", DisplayName = "Drone", HintText = "Talk",
            Zone = HubZoneId.DroneNiche, Position = Vec3.Zero,
            Bounds = new Bounds3D(0, 0, 0, 1, 1, 1),
            RequiredPhase = HubRhythmPhase.DroneContact,
            InteractiveType = InteractiveType.Npc,
            DialogueId = "DIA_DRONE_GREETING",
        };
        var baseResult = new InteractionResult { ObjectId = "drone_dock", Success = true, Message = "Дрон" };
        var result = handler.Handle(obj, baseResult);
        Assert.Equal("DIA_DRONE_GREETING", result.DialogueId);
        Assert.True(dp.IsPlaying);
    }

    [Fact]
    public void InteractionHandler_Terminal_GrantsItems()
    {
        var inv = MakeInventory();
        var handler = MakeHandler(inv: inv);
        var obj = new InteractableObject
        {
            Id = "supply_terminal", DisplayName = "Supply", HintText = "Get",
            Zone = HubZoneId.Logistics, Position = Vec3.Zero,
            Bounds = new Bounds3D(0, 0, 0, 1, 1, 1),
            RequiredPhase = HubRhythmPhase.Provisioning,
            InteractiveType = InteractiveType.Terminal,
            DialogueId = "DIA_SUPPLY_GRANTED",
            GrantsItems = ["ITEM_ARCHIVE_BADGE", "ITEM_BASIC_SCANNER", "ITEM_FIELD_RATION"],
        };
        var baseResult = new InteractionResult { ObjectId = "supply_terminal", Success = true, Message = "Снаряжение" };
        var result = handler.Handle(obj, baseResult);
        Assert.NotNull(result.GrantedItems);
        Assert.Equal(3, result.GrantedItems.Count);
        Assert.True(inv.Has("ITEM_ARCHIVE_BADGE"));
        Assert.True(inv.Has("ITEM_BASIC_SCANNER"));
        Assert.True(inv.Has("ITEM_FIELD_RATION"));
    }

    [Fact]
    public void InteractionHandler_Gate_ShowsDialogue()
    {
        var dp = MakeDialoguePlayer();
        var handler = MakeHandler(dp: dp);
        var obj = new InteractableObject
        {
            Id = "archive_gate", DisplayName = "Gate", HintText = "Locked",
            Zone = HubZoneId.HardArchiveEntrance, Position = Vec3.Zero,
            Bounds = new Bounds3D(0, 0, 0, 1, 1, 1),
            RequiredPhase = HubRhythmPhase.OperationAccess,
            InteractiveType = InteractiveType.Gate,
            DialogueId = "DIA_ARCHIVE_LOCKED",
            LockedMessage = "Заблокировано",
        };
        var baseResult = new InteractionResult { ObjectId = "archive_gate", Success = true, Message = "Заблокировано" };
        var result = handler.Handle(obj, baseResult);
        Assert.Equal("DIA_ARCHIVE_LOCKED", result.DialogueId);
        Assert.Equal(InteractiveType.Gate, result.ObjectType);
    }

    [Fact]
    public void InteractionHandler_PhaseAdvance_FiresTrigger()
    {
        var ot = MakeObjectiveTracker();
        ot.SetActive("UI_OBJ_EXIT_CAPSULE"); // make it active first
        var handler = MakeHandler(ot: ot);
        var obj = new InteractableObject
        {
            Id = "capsule_exit", DisplayName = "Exit", HintText = "Exit",
            Zone = HubZoneId.Capsule, Position = Vec3.Zero,
            Bounds = new Bounds3D(0, 0, 0, 1, 1, 1),
            RequiredPhase = HubRhythmPhase.Awakening,
            InteractiveType = InteractiveType.Trigger,
        };
        var baseResult = new InteractionResult
        {
            ObjectId = "capsule_exit", Success = true, Message = "Exited",
            NewPhase = HubRhythmPhase.Identification,
        };
        var result = handler.Handle(obj, baseResult);
        // Trigger should have set objective to UI_OBJ_BIOMETRIC and completed UI_OBJ_EXIT_CAPSULE
        Assert.Equal(ObjectiveStatus.Completed, ot.Objectives["UI_OBJ_EXIT_CAPSULE"].Status);
        Assert.Equal(ObjectiveStatus.Active, ot.Objectives["UI_OBJ_BIOMETRIC"].Status);
    }

    [Fact]
    public void InteractionHandler_Terminal_ShowsScreen()
    {
        var handler = MakeHandler();
        handler.RegisterTerminalScreen(new TerminalScreen
        {
            TerminalId = "op_terminal",
            Title = "Терминал операций",
            Lines = ["Нет назначенных операций."],
        });
        var obj = new InteractableObject
        {
            Id = "op_terminal", DisplayName = "Ops", HintText = "Ops",
            Zone = HubZoneId.MissionTerminal, Position = Vec3.Zero,
            Bounds = new Bounds3D(0, 0, 0, 1, 1, 1),
            RequiredPhase = HubRhythmPhase.OperationAccess,
            InteractiveType = InteractiveType.Terminal,
        };
        var baseResult = new InteractionResult { ObjectId = "op_terminal", Success = true, Message = "Терминал" };
        var result = handler.Handle(obj, baseResult);
        Assert.NotNull(result.TerminalScreen);
        Assert.Equal("Терминал операций", result.TerminalScreen.Title);
    }

    // ═══════════════════════════════════════════════════════════════
    //  11. GameplaySession with InteractionHandler
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void GameplaySession_WithHandler_EnrichesInteraction()
    {
        var zones = HubA0SceneBuilder.Build();
        var journal = new EventJournal();
        var hubRuntime = new HubA0Runtime(zones, journal, Logger);
        var geo = new SceneGeometry(new OctagonalFloor(22f, 11f), zones);
        var collision = new CollisionSystem3D(geo);
        var player = new PlayerEntity { Position = new Vec3(-7f, 0f, -6f) };
        var handler = MakeHandler(journal: journal);

        var session = new GameplaySession(player, geo, collision, hubRuntime, CameraProfile.Default3D, Logger, handler);
        var interact = InputSnapshot.FromActions(false, false, false, false, interact: true);
        var result = session.ProcessFrame(interact, 1f / 60f);

        Assert.NotNull(result.Interaction);
        Assert.True(result.Interaction.Success);
        Assert.Equal(InteractiveType.Trigger, result.Interaction.ObjectType);
    }

    [Fact]
    public void GameplaySession_WithoutHandler_StillWorks()
    {
        var zones = HubA0SceneBuilder.Build();
        var journal = new EventJournal();
        var hubRuntime = new HubA0Runtime(zones, journal, Logger);
        var geo = new SceneGeometry(new OctagonalFloor(22f, 11f), zones);
        var collision = new CollisionSystem3D(geo);
        var player = new PlayerEntity { Position = new Vec3(-7f, 0f, -6f) };

        // No handler — backward compatible
        var session = new GameplaySession(player, geo, collision, hubRuntime, CameraProfile.Default3D, Logger);
        var interact = InputSnapshot.FromActions(false, false, false, false, interact: true);
        var result = session.ProcessFrame(interact, 1f / 60f);

        Assert.NotNull(result.Interaction);
        Assert.True(result.Interaction.Success);
        Assert.Null(result.Interaction.ObjectType); // No handler → no type enrichment
    }

    // ═══════════════════════════════════════════════════════════════
    //  12. Inline Dialogue data model
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void InlineDialogueLine_HasRequiredProperties()
    {
        var line = new InlineDialogueLine
        {
            LineId = "L1", Speaker = "CORE", Text = "Hello", Delay = 2.0f,
        };
        Assert.Equal("L1", line.LineId);
        Assert.Equal(2.0f, line.Delay);
    }

    [Fact]
    public void InlineDialogue_HasLines()
    {
        var dlg = new InlineDialogue
        {
            DialogueId = "TEST", SpeakerDisplayName = "Test",
            Lines = [new InlineDialogueLine { LineId = "X", Speaker = "S", Text = "T" }],
        };
        Assert.Single(dlg.Lines);
    }

    // ═══════════════════════════════════════════════════════════════
    //  13. Objective data model
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void Objective_DefaultStatus_IsLocked()
    {
        var obj = new Objective { ObjectiveId = "OBJ1", Text = "Test", Order = 1 };
        Assert.Equal(ObjectiveStatus.Locked, obj.Status);
    }

    [Fact]
    public void ObjectiveStatus_HasThreeValues()
    {
        Assert.Equal(3, Enum.GetValues<ObjectiveStatus>().Length);
    }

    // ═══════════════════════════════════════════════════════════════
    //  14. PhaseTrigger data model
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void PhaseTrigger_HasActions()
    {
        var trigger = new PhaseTrigger
        {
            TriggerId = "TRG_TEST",
            Phase = HubRhythmPhase.Awakening,
            Actions = [new TriggerAction { Type = TriggerActionType.JournalEntry, Text = "Entry" }],
        };
        Assert.Single(trigger.Actions);
    }

    [Fact]
    public void TriggerActionType_HasThreeValues()
    {
        Assert.Equal(3, Enum.GetValues<TriggerActionType>().Length);
    }
}
