using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Migration S043: cycle safety contract.
/// </summary>
public sealed class Migration_043
{
    public Session043CycleSafetyContract Migrate(object? legacyState)
    {
        if (legacyState is Session043CycleSafetyContract existing)
        {
            return existing;
        }

        return new Session043CycleSafetyContract
        {
            ContractVersion = 43,
            MissionId = "mission-043",
            UnsafeCycleNodeIds = Array.Empty<string>(),
            MaxAllowedCycleLength = 8
        };
    }
}
