using System.Text.Json;
using BabylonArchiveCore.Domain;

namespace BabylonArchiveCore.Infrastructure.Save;

public sealed class SaveGameStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public SaveGame LoadOrCreate(string filePath, int expectedVersion)
    {
        if (File.Exists(filePath))
        {
            var content = File.ReadAllText(filePath);
            var save = JsonSerializer.Deserialize<SaveGame>(content, JsonOptions);
            if (save is not null && save.Version == expectedVersion)
            {
                return save;
            }
        }

        var created = new SaveGame
        {
            Version = expectedVersion,
            LastUpdatedUtc = DateTime.UtcNow,
        };

        Save(filePath, created);
        return created;
    }

    public void Save(string filePath, SaveGame saveGame)
    {
        ArgumentNullException.ThrowIfNull(saveGame);

        var updated = new SaveGame
        {
            Version = saveGame.Version,
            OperatorName = saveGame.OperatorName,
            LastScene = saveGame.LastScene,
            WorldSeed = saveGame.WorldSeed,
            WorldSeedAddress = saveGame.WorldSeedAddress,
            LastUpdatedUtc = DateTime.UtcNow,
            // Session 5: prologue state
            CurrentPhase = saveGame.CurrentPhase,
            VisitedObjects = [.. saveGame.VisitedObjects],
            InventoryItemIds = [.. saveGame.InventoryItemIds],
            CompletedObjectiveIds = [.. saveGame.CompletedObjectiveIds],
            ActiveObjectiveId = saveGame.ActiveObjectiveId,
            OperatorLevel = saveGame.OperatorLevel,
            OperatorXp = saveGame.OperatorXp,
            ProtocolZeroUnlocked = saveGame.ProtocolZeroUnlocked,
        };

        var json = JsonSerializer.Serialize(updated, JsonOptions);
        File.WriteAllText(filePath, json);
    }
}
