using System.Text.Json.Nodes;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция данных к версии схемы 17.
/// </summary>
public static class Migration_017
{
    public const int TargetVersion = 17;

    public static JsonObject Apply(JsonObject source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var result = (JsonObject?)source.DeepClone() ?? new JsonObject();
        result["schemaVersion"] = TargetVersion;

        if (result["runtimeState"] is null)
        {
            result["runtimeState"] = new JsonObject
            {
                ["stateId"] = "default",
                ["currentMode"] = "exploration",
                ["lastAppliedTick"] = 0,
                ["isDirty"] = false,
            };
        }

        return result;
    }
}
