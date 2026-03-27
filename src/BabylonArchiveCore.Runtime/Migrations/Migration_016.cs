using System.Text.Json.Nodes;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция данных к версии схемы 16.
/// </summary>
public static class Migration_016
{
    public const int TargetVersion = 16;

    public static JsonObject Apply(JsonObject source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = (JsonObject?)source.DeepClone() ?? new JsonObject();
        result["schemaVersion"] = TargetVersion;

        if (result["gameLoopState"] is null)
        {
            result["gameLoopState"] = "running";
        }

        return result;
    }
}
