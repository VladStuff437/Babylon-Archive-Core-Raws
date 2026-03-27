using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S040: runtime-контракт миссии.
/// </summary>
public sealed class Migration_040
{
    public Session040MissionRuntimeContract Migrate(object? legacyState)
    {
        if (legacyState is Session040MissionRuntimeContract existing)
        {
            return existing;
        }

        return new Session040MissionRuntimeContract
        {
            ContractVersion = 40,
            MissionId = "mission-040",
            CurrentNodeId = "start",
            IsCompleted = false,
            StepCount = 0,
            MaxStepCount = 128
        };
    }
}
