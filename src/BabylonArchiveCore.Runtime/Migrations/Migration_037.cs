using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S037: контракт MissionDefinition.
/// </summary>
public sealed class Migration_037
{
    public Session037MissionDefinitionContract Migrate(object? legacyState)
    {
        if (legacyState is Session037MissionDefinitionContract existing)
        {
            return existing;
        }

        return new Session037MissionDefinitionContract
        {
            MissionId = "mission-037",
            Title = "Mission Definition Baseline",
            StartNodeId = "start",
            ContractVersion = 37,
            NodeIds = new[] { "start" }
        };
    }
}
