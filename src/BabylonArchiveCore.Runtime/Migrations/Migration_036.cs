using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S036: оси мира.
/// </summary>
public sealed class Migration_036
{
    public Session036WorldAxesContract Migrate(object? legacyState)
    {
        if (legacyState is Session036WorldAxesContract existing)
        {
            return existing;
        }

        return new Session036WorldAxesContract
        {
            MoralAxis = 0f,
            TechnoArcaneAxis = 0f,
            WorldAxisVersion = 36,
            FactionReputations = new Dictionary<string, int>(StringComparer.Ordinal)
        };
    }
}
