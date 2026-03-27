using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S035: репутации и оси мира.
/// </summary>
public sealed class Migration_035
{
    public Session035FactionReputationContract Migrate(object? legacyState)
    {
        if (legacyState is Session035FactionReputationContract existing)
        {
            return existing;
        }

        return new Session035FactionReputationContract
        {
            MoralAxis = 0f,
            TechnoArcaneAxis = 0f,
            FactionReputations = new Dictionary<string, int>(StringComparer.Ordinal)
        };
    }
}
