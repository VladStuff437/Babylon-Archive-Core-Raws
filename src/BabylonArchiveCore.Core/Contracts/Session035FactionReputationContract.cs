namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S035: репутации фракций и оси мира.
/// </summary>
public sealed class Session035FactionReputationContract
{
    public float MoralAxis { get; init; }

    public float TechnoArcaneAxis { get; init; }

    public required IReadOnlyDictionary<string, int> FactionReputations { get; init; }
}
