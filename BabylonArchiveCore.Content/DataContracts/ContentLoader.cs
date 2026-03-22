using System.Text.Json;
using System.Text.Json.Serialization;

namespace BabylonArchiveCore.Content.DataContracts;

/// <summary>
/// Loads content JSON files into typed data contracts.
/// All content is data-driven — no hardcoded scene logic.
/// </summary>
public static class ContentLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public static T Load<T>(string filePath) where T : class
    {
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(json, JsonOptions)
               ?? throw new InvalidOperationException($"Failed to deserialize {filePath}");
    }

    public static SceneData LoadScene(string contentRoot, string sceneId)
        => Load<SceneData>(Path.Combine(contentRoot, "Scenes", $"{sceneId}.json"));

    public static ZoneFileData LoadZones(string contentRoot, string fileName)
        => Load<ZoneFileData>(Path.Combine(contentRoot, "Zones", fileName));

    public static ObjectFileData LoadObjects(string contentRoot, string fileName)
        => Load<ObjectFileData>(Path.Combine(contentRoot, "Objects", fileName));

    public static DialogueFileData LoadDialogues(string contentRoot, string fileName)
        => Load<DialogueFileData>(Path.Combine(contentRoot, "Dialogue", fileName));

    public static TriggerFileData LoadTriggers(string contentRoot, string fileName)
        => Load<TriggerFileData>(Path.Combine(contentRoot, "Triggers", fileName));

    public static ObjectiveFileData LoadObjectives(string contentRoot, string fileName)
        => Load<ObjectiveFileData>(Path.Combine(contentRoot, "UI", fileName));
}
