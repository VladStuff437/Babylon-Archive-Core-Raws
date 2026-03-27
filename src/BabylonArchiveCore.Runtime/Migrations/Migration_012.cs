using System.Text.Json.Nodes;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция данных к версии схемы 12.
/// </summary>
public static class Migration_012
{
    public const int TargetVersion = 12;

    public static JsonObject Apply(JsonObject source)
    {
        var result = (JsonObject?)source.DeepClone() ?? new JsonObject();
        result["schemaVersion"] = TargetVersion;

        if (result["lastUpdatedUtc"] is null)
        {
            result["lastUpdatedUtc"] = DateTimeOffset.UtcNow.ToString("O");
        }

        return result;
    }
}
