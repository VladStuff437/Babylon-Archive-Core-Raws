using System.Text.Json.Nodes;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция данных к версии схемы 19.
/// </summary>
public static class Migration_019
{
    public const int TargetVersion = 19;

    public static JsonObject Apply(JsonObject source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = (JsonObject?)source.DeepClone() ?? new JsonObject();
        result["schemaVersion"] = TargetVersion;

        if (result["actionMapProfile"] is null)
        {
            result["actionMapProfile"] = "session-019-action-map";
        }

        return result;
    }
}
