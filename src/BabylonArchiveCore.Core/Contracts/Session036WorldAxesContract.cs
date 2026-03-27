namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S036: оси мира и влияние на контур миссий.
/// </summary>
public sealed class Session036WorldAxesContract
{
    public float MoralAxis { get; init; }

    public float TechnoArcaneAxis { get; init; }

    public int WorldAxisVersion { get; init; }

    public required IReadOnlyDictionary<string, int> FactionReputations { get; init; }
}
