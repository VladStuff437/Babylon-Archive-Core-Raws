using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Migration S042: dead-end contract.
/// </summary>
public sealed class Migration_042
{
    public Session042DeadEndContract Migrate(object? legacyState)
    {
        if (legacyState is Session042DeadEndContract existing)
        {
            return existing;
        }

        return new Session042DeadEndContract
        {
            ContractVersion = 42,
            MissionId = "mission-042",
            DeadEndNodeIds = Array.Empty<string>(),
            AllowsFallback = true
        };
    }
}
