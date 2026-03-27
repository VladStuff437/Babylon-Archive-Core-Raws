using BabylonArchiveCore.Core.Archive;
using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Migration S046: archive address model contract.
/// </summary>
public sealed class Migration_046
{
    public Session046ArchiveAddressContract Migrate(object? legacyState)
    {
        if (legacyState is Session046ArchiveAddressContract existing)
        {
            return existing;
        }

        var address = new ArchiveAddress(1, 0, 0, 0, 0, 0);
        return new Session046ArchiveAddressContract
        {
            ContractVersion = 46,
            CanonicalAddress = address.ToCanonicalString(),
            Path = address.ToPath().ToArray(),
            Seed = ArchiveSeed.ToSeed(address)
        };
    }
}
