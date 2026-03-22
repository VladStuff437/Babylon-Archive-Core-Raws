namespace BabylonArchiveCore.Domain;

public sealed class SaveGame
{
    public int Version { get; init; } = 1;

    public string OperatorName { get; init; } = "Alan Arcwain";

    public SceneId LastScene { get; init; } = SceneId.Boot;

    public int WorldSeed { get; init; } = 42;

    public string WorldSeedAddress { get; init; } = "S00.H00.M00.SH00.C00.T000.P000";

    public DateTime LastUpdatedUtc { get; init; } = DateTime.UtcNow;

    // Session 5: prologue state
    public string CurrentPhase { get; init; } = "Awakening";
    public List<string> VisitedObjects { get; init; } = [];
    public List<string> InventoryItemIds { get; init; } = [];
    public List<string> CompletedObjectiveIds { get; init; } = [];
    public string? ActiveObjectiveId { get; init; }
    public int OperatorLevel { get; init; } = 1;
    public int OperatorXp { get; init; }
    public bool ProtocolZeroUnlocked { get; init; }
}
