namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// S047 contract: deterministic seed composition chain.
/// </summary>
public sealed class Session047SeedCompositionContract
{
    public int ContractVersion { get; init; } = 47;

    public int BaseSeed { get; init; }

    public required string[] Scopes { get; init; }

    public int ComposedSeed { get; init; }
}
