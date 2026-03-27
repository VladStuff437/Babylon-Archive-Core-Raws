using System.Text.Json.Nodes;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция данных к версии схемы 20.
/// </summary>
public static class Migration_020
{
    public const int TargetVersion = 20;

    public static JsonObject Apply(JsonObject source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = (JsonObject?)source.DeepClone() ?? new JsonObject();
        result["schemaVersion"] = TargetVersion;

        if (result["controlProfileVersion"] is null)
        {
            result["controlProfileVersion"] = 1;
        }

        if (result["activeProfile"] is null)
        {
            result["activeProfile"] = "session-020-controls";
        }

        if (result["fallbackProfile"] is null)
        {
            result["fallbackProfile"] = "fallback";
        }

        return result;
    }
}
