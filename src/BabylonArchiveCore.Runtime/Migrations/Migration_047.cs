using BabylonArchiveCore.Core.Archive;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Migration S047: seed composition contract.
/// </summary>
public sealed class Migration_047
{
    public Session047SeedCompositionContract Migrate(object? legacyState)
    {
        if (legacyState is Session047SeedCompositionContract existing)
        {
            return existing;
        }

        var baseSeed = 47;
        var composed = ArchiveSeed.ComposeSeed(baseSeed,
            new SeedScope("sector", 1),
            new SeedScope("hall", 2));

        return new Session047SeedCompositionContract
        {
            ContractVersion = 47,
            BaseSeed = baseSeed,
            Scopes = new[] { "sector:1", "hall:2" },
            ComposedSeed = composed
        };
    }
}
