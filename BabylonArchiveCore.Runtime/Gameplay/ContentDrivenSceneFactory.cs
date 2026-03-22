using BabylonArchiveCore.Content.Pipeline;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain;
using BabylonArchiveCore.Domain.Player;
using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Runtime.Scene;

namespace BabylonArchiveCore.Runtime.Gameplay;

/// <summary>
/// Builds a fully wired GameplaySession from JSON content,
/// replacing the manual inline setup from Sessions 4-5.
/// </summary>
public sealed class ContentDrivenSceneFactory
{
    private readonly A0ContentProvider _contentProvider;
    private readonly ILogger _logger;

    public ContentDrivenSceneFactory(A0ContentProvider contentProvider, ILogger logger)
    {
        _contentProvider = contentProvider;
        _logger = logger;
    }

    /// <summary>
    /// All runtime components built by <see cref="Build"/>.
    /// </summary>
    public sealed class SceneBundle
    {
        public required GameplaySession Session { get; init; }
        public required HubA0Runtime HubRuntime { get; init; }
        public required InteractionHandler InteractionHandler { get; init; }
        public required PrologueTracker PrologueTracker { get; init; }
        public required SceneGeometry Geometry { get; init; }
        public required CollisionSystem3D Collision { get; init; }
        public required DialoguePlayer DialoguePlayer { get; init; }
        public required ObjectiveTracker ObjectiveTracker { get; init; }
        public required TriggerSystem TriggerSystem { get; init; }
        public required PlayerInventory Inventory { get; init; }
        public required EventJournal Journal { get; init; }
        public required PlayerEntity Player { get; init; }
    }

    /// <summary>
    /// Wire all systems from JSON content and return a ready-to-run bundle.
    /// </summary>
    public SceneBundle Build(PlayerEntity? player = null, CameraProfile? cameraProfile = null)
    {
        // 1. Load content from JSON
        var zones = _contentProvider.LoadZones();
        var dialogues = _contentProvider.LoadDialogues();
        var triggers = _contentProvider.LoadTriggers();
        var objectInteractionTriggers = _contentProvider.LoadObjectInteractionTriggers();
        var objectives = _contentProvider.LoadObjectives();

        _logger.Info($"ContentDrivenSceneFactory: loaded {zones.Count} zones, " +
            $"{dialogues.Count} dialogues, {triggers.Count} triggers, {objectives.Count} objectives");

        // 2. Geometry
        var floor = HubA0SceneBuilder.BuildFloor();
        var geometry = new SceneGeometry(floor, zones);
        var collision = new CollisionSystem3D(geometry);

        // 3. Journal & hub runtime
        var journal = new EventJournal();
        var hubRuntime = new HubA0Runtime(zones, journal, _logger);

        // 4. Dialogue player
        var dialoguePlayer = new DialoguePlayer();
        foreach (var d in dialogues) dialoguePlayer.Register(d);

        // 5. Objective tracker
        var objectiveTracker = new ObjectiveTracker();
        foreach (var o in objectives) objectiveTracker.Register(o);
        objectiveTracker.SetActive("UI_OBJ_EXIT_CAPSULE");

        // 6. Trigger system
        var triggerSystem = new TriggerSystem(_logger);
        foreach (var t in triggers) triggerSystem.Register(t);

        // 7. Inventory
        var inventory = new PlayerInventory();

        // 8. Interaction handler
        var handler = new InteractionHandler(dialoguePlayer, objectiveTracker, triggerSystem, inventory, journal, _logger);
        handler.RegisterItemGrant("supply_terminal",
        [
            new InventoryItem { ItemId = "ITEM_ARCHIVE_BADGE", Name = "Жетон Архива", Type = ItemType.KeyItem },
            new InventoryItem { ItemId = "ITEM_BASIC_SCANNER", Name = "Базовый сканер", Type = ItemType.Tool },
            new InventoryItem { ItemId = "ITEM_FIELD_RATION", Name = "Полевой рацион", Type = ItemType.Consumable },
        ]);

        handler.RegisterTerminalScreen(new TerminalScreen
        {
            TerminalId = "bio_scanner",
            Title = "БИОМЕТРИЯ",
            Lines = ["Сканирование...", "Личность подтверждена.", "Оператор: Алан Арквейн."],
        });

        handler.RegisterTerminalScreen(new TerminalScreen
        {
            TerminalId = "supply_terminal",
            Title = "СНАБЖЕНИЕ",
            Lines = ["Выдан комплект:", "- Жетон Архива", "- Базовый сканер", "- Полевой рацион"],
        });

        handler.RegisterTerminalScreen(new TerminalScreen
        {
            TerminalId = "core_console",
            Title = "C.O.R.E.",
            Lines = ["Оператор идентифицирован.", "Контур архива: нестабилен.", "Доступ к операциям открыт."],
        });

        handler.RegisterTerminalScreen(new TerminalScreen
        {
            TerminalId = "op_terminal",
            Title = "ТЕРМИНАЛ ОПЕРАЦИЙ",
            Lines = ["P01 // Эхо в стеке 9 // INDEXED", "P02 // Несовместимый индекс // LOCKED", "P03 // Пустой свидетель // SHADOWED"],
        });

        handler.RegisterTerminalScreen(new TerminalScreen
        {
            TerminalId = "research_terminal",
            Title = "ТЕРМИНАЛ ИССЛЕДОВАНИЙ",
            Lines = ["Анализ архивных структур...", "Обнаружена нестыковка индекса.", "Рекомендация: проверить последовательность данных."],
        });

        foreach (var kv in objectInteractionTriggers)
            handler.RegisterObjectInteractionActions(kv.Key, kv.Value);

        // 9. Prologue tracker
        var prologueTracker = new PrologueTracker(_logger);

        // 10. Player
        var playerEntity = player ?? new PlayerEntity { Position = new Vec3(-7f, 0f, -6f) };

        // 11. Gameplay session
        var session = new GameplaySession(
            playerEntity, geometry, collision, hubRuntime,
            cameraProfile ?? CameraProfile.Default3D, _logger, handler);

        _logger.Info("ContentDrivenSceneFactory: build complete.");

        return new SceneBundle
        {
            Session = session,
            HubRuntime = hubRuntime,
            InteractionHandler = handler,
            PrologueTracker = prologueTracker,
            Geometry = geometry,
            Collision = collision,
            DialoguePlayer = dialoguePlayer,
            ObjectiveTracker = objectiveTracker,
            TriggerSystem = triggerSystem,
            Inventory = inventory,
            Journal = journal,
            Player = playerEntity,
        };
    }
}
