using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Runtime.Scene;

/// <summary>
/// Runs the Hub A-0 scene: manages rhythm phase progression,
/// processes player interactions, and records journal events.
/// </summary>
public sealed class HubA0Runtime
{
    private readonly List<HubZone> _zones;
    private readonly EventJournal _journal;
    private readonly ILogger _logger;

    public HubRhythmPhase CurrentPhase { get; private set; } = HubRhythmPhase.Awakening;

    public IReadOnlyList<HubZone> Zones => _zones;
    public EventJournal Journal => _journal;

    public HubA0Runtime(List<HubZone> zones, EventJournal journal, ILogger logger)
    {
        _zones = zones;
        _journal = journal;
        _logger = logger;

        _journal.Record("system", "Оператор пробуждается в капсуле.");
        _logger.Info($"Hub A-0 initialized. Phase: {CurrentPhase}");
    }

    /// <summary>
    /// Returns all interactable objects available in the current rhythm phase.
    /// </summary>
    public List<InteractableObject> GetAvailableInteractables() =>
        _zones.SelectMany(z => z.Objects)
              .Where(o => o.IsActiveIn(CurrentPhase))
              .ToList();

    /// <summary>
    /// Returns contextual hints for objects near the player's 3D position.
    /// </summary>
    public List<(InteractableObject Object, string Hint)> GetNearbyHints(Vec3 playerPos, float radius)
    {
        var radiusSq = radius * radius;
        return _zones
            .SelectMany(z => z.Objects)
            .Where(o => o.IsActiveIn(CurrentPhase) &&
                        (o.Position - playerPos).LengthSquared() <= radiusSq)
            .Select(o => (o, o.HintText))
            .ToList();
    }

    /// <summary>
    /// Process an interaction with the specified object.
    /// If the interaction advances the scene rhythm, the phase is updated.
    /// </summary>
    public InteractionResult Interact(string objectId)
    {
        var obj = _zones.SelectMany(z => z.Objects).FirstOrDefault(o => o.Id == objectId);

        if (obj is null)
            return new InteractionResult { ObjectId = objectId, Success = false, Message = "Объект не найден." };

        if (!obj.IsActiveIn(CurrentPhase))
            return new InteractionResult { ObjectId = objectId, Success = false, Message = $"Требуется фаза: {obj.RequiredPhase}." };

        var oneShot = objectId is "capsule_exit" or "bio_scanner" or "supply_terminal" or "drone_dock" or "core_console";
        if (oneShot)
            obj.ContextState = "used";

        HubRhythmPhase? newPhase = objectId switch
        {
            "capsule_exit" => AdvanceTo(HubRhythmPhase.Identification),
            "bio_scanner" => AdvanceTo(HubRhythmPhase.Provisioning),
            "supply_terminal" => AdvanceTo(HubRhythmPhase.DroneContact),
            "drone_dock" => AdvanceTo(HubRhythmPhase.Activation),
            "core_console" => AdvanceTo(HubRhythmPhase.OperationAccess),
            _ => null,
        };

        var message = objectId switch
        {
            "capsule_exit" => "Оператор покинул капсулу. Пройдите биометрическую проверку.",
            "bio_scanner" => "Идентификация подтверждена: Алан Арквейн. Получите снаряжение.",
            "supply_terminal" => "Снаряжение получено. Активируйте дрон.",
            "drone_dock" => "Дрон активирован. Обратитесь к C.O.R.E.",
            "core_console" => "C.O.R.E. активирован. Доступ к операциям открыт.",
            "op_terminal" => "Терминал операций активирован.",
            "research_terminal" => "Терминал исследований (доступ ограничен).",
            "gallery_overlook" => "Обзорная галерея. Хард-Архив виден внизу.",
            "archive_gate" => "Врата Хард-Архива заблокированы.",
            _ => $"Взаимодействие с {obj.DisplayName}.",
        };

        _journal.Record("interaction", $"[{CurrentPhase}] {message}");
        _logger.Info($"Interaction: {objectId} → {message}");

        return new InteractionResult
        {
            ObjectId = objectId,
            Success = true,
            Message = message,
            NewPhase = newPhase,
        };
    }

    private HubRhythmPhase? AdvanceTo(HubRhythmPhase target)
    {
        if (CurrentPhase >= target) return null;
        CurrentPhase = target;
        _journal.Record("rhythm", $"Фаза хаба: {target}");
        _logger.Info($"Rhythm advanced to: {target}");
        return target;
    }
}
