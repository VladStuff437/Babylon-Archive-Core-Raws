using BabylonArchiveCore.Core.Archive;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Migration S048: generator-core contract.
/// </summary>
public sealed class Migration_048
{
    public Session048GeneratorCoreContract Migrate(object? legacyState)
    {
        if (legacyState is Session048GeneratorCoreContract existing)
        {
            return existing;
        }

        var address = new ArchiveAddress(4, 8, 0, 0, 0, 1);
        return new Session048GeneratorCoreContract
        {
            ContractVersion = 48,
            Address = address.ToCanonicalString(),
            WorldSeed = 48,
            RoomCount = 6,
            Strategy = "generator-core"
        };
    }
}
