using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Migration S045: mission runtime persistence contract.
/// </summary>
public sealed class Migration_045
{
    public Session045MissionPersistenceContract Migrate(object? legacyState)
    {
        if (legacyState is Session045MissionPersistenceContract existing)
        {
            return existing;
        }

        return new Session045MissionPersistenceContract
        {
            ContractVersion = 45,
            MissionId = "mission-045",
            SnapshotVersion = 45,
            StateChecksum = "UNINITIALIZED",
            StepCount = 0
        };
    }
}
