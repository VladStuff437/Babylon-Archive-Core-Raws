using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Migration S044: fallback mission contract.
/// </summary>
public sealed class Migration_044
{
    public Session044FallbackMissionContract Migrate(object? legacyState)
    {
        if (legacyState is Session044FallbackMissionContract existing)
        {
            return existing;
        }

        return new Session044FallbackMissionContract
        {
            ContractVersion = 44,
            SourceMissionId = "mission-044",
            FallbackMissionId = "mission-044.fallback",
            ReasonCodes = new[] { "MVAL-FALLBACK" },
            IsDeterministic = true
        };
    }
}
