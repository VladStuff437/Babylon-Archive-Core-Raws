using BabylonArchiveCore.Core.Events;

namespace BabylonArchiveCore.Runtime.Mission;

/// <summary>
/// Published when a mission changes status (started, completed, failed).
/// </summary>
public sealed class MissionStatusChangedEvent : IDomainEvent
{
    public required string MissionId { get; init; }
    public required Domain.Mission.MissionStatus OldStatus { get; init; }
    public required Domain.Mission.MissionStatus NewStatus { get; init; }
    public DateTime OccurredUtc { get; init; } = DateTime.UtcNow;
}
