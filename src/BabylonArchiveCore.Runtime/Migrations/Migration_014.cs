using System.Text.Json.Nodes;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция данных к версии схемы 14.
/// </summary>
public static class Migration_014
{
    public const int TargetVersion = 14;

    public static JsonObject Apply(JsonObject source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = (JsonObject?)source.DeepClone() ?? new JsonObject();
        result["schemaVersion"] = TargetVersion;

        if (result["migrationId"] is null)
        {
            result["migrationId"] = "migration-014";
        }

        return result;
    }
}
