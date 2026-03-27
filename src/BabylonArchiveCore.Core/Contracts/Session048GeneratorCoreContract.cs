namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// S048 contract: generator-core layout build metadata.
/// </summary>
public sealed class Session048GeneratorCoreContract
{
    public int ContractVersion { get; init; } = 48;

    public required string Address { get; init; }

    public int WorldSeed { get; init; }

    public int RoomCount { get; init; }

    public required string Strategy { get; init; }
}
