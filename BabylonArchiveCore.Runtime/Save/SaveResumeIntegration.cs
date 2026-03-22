using BabylonArchiveCore.Content.Pipeline;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain;
using BabylonArchiveCore.Domain.Player;
using BabylonArchiveCore.Domain.Scene;

namespace BabylonArchiveCore.Runtime.Save;

/// <summary>
/// Integrates save/resume into the content-driven pipeline.
/// Creates a scene from JSON content and restores runtime state from a SaveGame.
/// </summary>
public sealed class SaveResumeIntegration
{
    private readonly A0ContentProvider _contentProvider;
    private readonly ILogger _logger;

    public SaveResumeIntegration(A0ContentProvider contentProvider, ILogger logger)
    {
        _contentProvider = contentProvider;
        _logger = logger;
    }

    /// <summary>
    /// Build a fresh scene and optionally restore from a save.
    /// If <paramref name="save"/> specifies a phase beyond Awakening,
    /// the runtime state is restored to that point.
    /// </summary>
    public Gameplay.ContentDrivenSceneFactory.SceneBundle BuildAndResume(
        SaveGame? save = null,
        PlayerEntity? player = null,
        CameraProfile? cameraProfile = null)
    {
        var factory = new Gameplay.ContentDrivenSceneFactory(_contentProvider, _logger);
        var bundle = factory.Build(player, cameraProfile);

        if (save is null || save.CurrentPhase == "Awakening")
        {
            _logger.Info("SaveResumeIntegration: fresh start (no save to restore).");
            return bundle;
        }

        _logger.Info($"SaveResumeIntegration: restoring from save — phase={save.CurrentPhase}, " +
            $"visited={save.VisitedObjects.Count}, items={save.InventoryItemIds.Count}");

        SaveGameMapper.Apply(save, bundle.HubRuntime, bundle.PrologueTracker,
            bundle.ObjectiveTracker, bundle.Inventory);

        _logger.Info($"SaveResumeIntegration: restored. Phase={bundle.HubRuntime.CurrentPhase}, " +
            $"inventory={bundle.Inventory.Items.Count}, P0={bundle.PrologueTracker.IsProtocolZeroUnlocked}");

        return bundle;
    }
}
