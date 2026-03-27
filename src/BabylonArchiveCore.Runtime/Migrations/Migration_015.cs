using System.Text.Json.Nodes;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция данных к версии схемы 15.
/// </summary>
public static class Migration_015
{
    public const int TargetVersion = 15;

    public static JsonObject Apply(JsonObject source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = (JsonObject?)source.DeepClone() ?? new JsonObject();
        result["schemaVersion"] = TargetVersion;

        if (result["eventTaxonomyVersion"] is null)
        {
            result["eventTaxonomyVersion"] = 1;
        }

        return result;
    }
}
