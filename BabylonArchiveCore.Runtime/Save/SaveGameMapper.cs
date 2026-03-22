using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain;
using BabylonArchiveCore.Domain.Player;
using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Runtime.Gameplay;
using BabylonArchiveCore.Runtime.Scene;

namespace BabylonArchiveCore.Runtime.Save;

/// <summary>
/// Bidirectional mapper: runtime state ↔ SaveGame record.
/// Extract captures current play state; Apply restores it.
/// </summary>
public static class SaveGameMapper
{
    /// <summary>
    /// Extract the current runtime state into a SaveGame record.
    /// </summary>
    public static SaveGame Extract(
        HubA0Runtime hubRuntime,
        PrologueTracker prologueTracker,
        ObjectiveTracker objectiveTracker,
        PlayerInventory inventory,
        int operatorXp,
        int operatorLevel = 1,
        SaveGame? baseSave = null)
    {
        return new SaveGame
        {
            Version = baseSave?.Version ?? 1,
            OperatorName = baseSave?.OperatorName ?? "Alan Arcwain",
            LastScene = baseSave?.LastScene ?? SceneId.Boot,
            WorldSeed = baseSave?.WorldSeed ?? 42,
            WorldSeedAddress = baseSave?.WorldSeedAddress ?? "S00.H00.M00.SH00.C00.T000.P000",
            CurrentPhase = hubRuntime.CurrentPhase.ToString(),
            VisitedObjects = [.. prologueTracker.VisitedObjects],
            InventoryItemIds = [.. inventory.Items.Select(i => i.ItemId)],
            CompletedObjectiveIds = [.. objectiveTracker.GetCompleted().Select(o => o.ObjectiveId)],
            ActiveObjectiveId = objectiveTracker.GetActive()?.ObjectiveId,
            OperatorLevel = operatorLevel,
            OperatorXp = operatorXp,
            ProtocolZeroUnlocked = prologueTracker.IsProtocolZeroUnlocked,
        };
    }

    /// <summary>
    /// Apply a SaveGame to restore runtime state after loading.
    /// Replays visited objects into the prologue tracker and advances
    /// the hub runtime to the saved phase.
    /// </summary>
    public static void Apply(
        SaveGame save,
        HubA0Runtime hubRuntime,
        PrologueTracker prologueTracker,
        ObjectiveTracker objectiveTracker,
        PlayerInventory inventory)
    {
        // Restore visited objects
        foreach (var objId in save.VisitedObjects)
            prologueTracker.RecordVisit(objId);

        // Restore inventory
        foreach (var itemId in save.InventoryItemIds)
            inventory.Add(new InventoryItem { ItemId = itemId, Name = itemId, Type = ItemType.Misc });

        // Restore objectives: complete visited ones, set active
        foreach (var completedId in save.CompletedObjectiveIds)
            objectiveTracker.Complete(completedId);

        if (save.ActiveObjectiveId is not null)
            objectiveTracker.SetActive(save.ActiveObjectiveId);

        // Advance hub to saved phase by replaying rhythm interactions
        if (Enum.TryParse<HubRhythmPhase>(save.CurrentPhase, out var targetPhase))
            AdvanceHubToPhase(hubRuntime, targetPhase);
    }

    /// <summary>
    /// Advance hub runtime to a target phase by interacting with
    /// the canonical object sequence.
    /// </summary>
    private static void AdvanceHubToPhase(HubA0Runtime hubRuntime, HubRhythmPhase targetPhase)
    {
        // Phase advancement sequence
        var sequence = new (string ObjectId, HubRhythmPhase ResultPhase)[]
        {
            ("capsule_exit", HubRhythmPhase.Identification),
            ("bio_scanner", HubRhythmPhase.Provisioning),
            ("supply_terminal", HubRhythmPhase.DroneContact),
            ("drone_dock", HubRhythmPhase.Activation),
            ("core_console", HubRhythmPhase.OperationAccess),
        };

        foreach (var (objId, resultPhase) in sequence)
        {
            if (hubRuntime.CurrentPhase >= targetPhase)
                break;
            hubRuntime.Interact(objId);
        }
    }
}
