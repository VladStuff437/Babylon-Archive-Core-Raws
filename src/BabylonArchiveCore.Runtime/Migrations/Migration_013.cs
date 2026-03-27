using System.Text.Json.Nodes;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция данных к версии схемы 13.
/// </summary>
public static class Migration_013
{
    public const int TargetVersion = 13;

    public static JsonObject Apply(JsonObject source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = (JsonObject?)source.DeepClone() ?? new JsonObject();
        result["schemaVersion"] = TargetVersion;

        if (result["schemaChannel"] is null)
        {
            result["schemaChannel"] = "stable";
        }

        return result;
    }
}
