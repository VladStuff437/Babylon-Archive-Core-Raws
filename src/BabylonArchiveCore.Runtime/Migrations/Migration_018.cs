using System.Text.Json.Nodes;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция данных к версии схемы 18.
/// </summary>
public static class Migration_018
{
    public const int TargetVersion = 18;

    public static JsonObject Apply(JsonObject source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = (JsonObject?)source.DeepClone() ?? new JsonObject();
        result["schemaVersion"] = TargetVersion;

        if (result["commandContourVersion"] is null)
        {
            result["commandContourVersion"] = 1;
        }

        return result;
    }
}
