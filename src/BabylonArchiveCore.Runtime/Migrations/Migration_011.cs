using System.Text.Json.Nodes;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция данных к версии схемы 11.
/// </summary>
public static class Migration_011
{
    public const int TargetVersion = 11;

    public static JsonObject Apply(JsonObject source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = (JsonObject?)source.DeepClone() ?? new JsonObject();
        result["schemaVersion"] = TargetVersion;

        if (result["metadata"] is null)
        {
            result["metadata"] = new JsonObject();
        }

        return result;
    }
}
