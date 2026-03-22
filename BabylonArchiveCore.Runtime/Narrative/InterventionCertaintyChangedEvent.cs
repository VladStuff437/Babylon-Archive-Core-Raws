using BabylonArchiveCore.Core.Events;

namespace BabylonArchiveCore.Runtime.Narrative;

/// <summary>Published when the intervention certainty level changes.</summary>
public sealed class InterventionCertaintyChangedEvent : IDomainEvent
{
    public DateTime OccurredUtc { get; init; } = DateTime.UtcNow;
    public required string OldLevel { get; init; }
    public required string NewLevel { get; init; }
    public required int TotalEvidenceScore { get; init; }
}
