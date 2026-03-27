namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S033: лут-система и редкости.
/// </summary>
public sealed class Session033LootContract
{
    public required string BalanceProfileId { get; init; }

    public required IReadOnlyDictionary<string, int> RarityWeights { get; init; }

    public float LuckBias { get; init; }
}
