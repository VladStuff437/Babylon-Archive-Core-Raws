namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S034: экономика v1.
/// </summary>
public sealed class Session034EconomyContract
{
    public required string EconomyProfileId { get; init; }

    public float InflationIndex { get; init; }

    public float BuyMultiplier { get; init; }

    public float SellMultiplier { get; init; }

    public float DefaultFactionModifier { get; init; }
}
