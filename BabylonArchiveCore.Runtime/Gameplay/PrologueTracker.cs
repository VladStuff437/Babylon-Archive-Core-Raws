using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Runtime.Gameplay;

/// <summary>
/// Tracks prologue progress: which objects have been visited,
/// and determines when Protocol Zero is unlocked.
/// </summary>
public sealed class PrologueTracker
{
    private readonly HashSet<string> _visitedObjects = new();
    private readonly ILogger _logger;

    /// <summary>
    /// Mandatory interactions that advance the prologue (steps 1-5).
    /// </summary>
    private static readonly string[] MandatoryObjects =
    [
        "capsule_exit", "bio_scanner", "supply_terminal", "drone_dock", "core_console"
    ];

    /// <summary>
    /// Exploration objects that must be visited for Protocol Zero (steps 6-8).
    /// </summary>
    private static readonly string[] ExplorationObjects =
    [
        "op_terminal", "gallery_overlook"
    ];

    public IReadOnlySet<string> VisitedObjects => _visitedObjects;
    public bool IsProtocolZeroUnlocked { get; private set; }

    public PrologueTracker(ILogger logger)
    {
        _logger = logger;
    }

    public void RecordVisit(string objectId)
    {
        if (_visitedObjects.Add(objectId))
        {
            _logger.Info($"PrologueTracker: visited {objectId} ({_visitedObjects.Count} total)");
            CheckProtocolZero();
        }
    }

    public bool HasVisited(string objectId) => _visitedObjects.Contains(objectId);

    public int VisitCount => _visitedObjects.Count;

    public int MandatoryCompleted =>
        MandatoryObjects.Count(o => _visitedObjects.Contains(o));

    public int ExplorationCompleted =>
        ExplorationObjects.Count(o => _visitedObjects.Contains(o));

    public float CompletionRatio =>
        (float)(MandatoryCompleted + ExplorationCompleted) / (MandatoryObjects.Length + ExplorationObjects.Length);

    private void CheckProtocolZero()
    {
        if (IsProtocolZeroUnlocked) return;

        var allMandatory = MandatoryObjects.All(o => _visitedObjects.Contains(o));
        var allExploration = ExplorationObjects.All(o => _visitedObjects.Contains(o));

        if (allMandatory && allExploration)
        {
            IsProtocolZeroUnlocked = true;
            _logger.Info("PrologueTracker: Protocol Zero UNLOCKED!");
        }
    }
}
