namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// S046 contract: canonical ArchiveAddress model and seed coupling.
/// </summary>
public sealed class Session046ArchiveAddressContract
{
    public int ContractVersion { get; init; } = 46;

    public required string CanonicalAddress { get; init; }

    public required int[] Path { get; init; }

    public int Seed { get; init; }
}
