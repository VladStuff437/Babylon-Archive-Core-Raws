using BabylonArchiveCore.Core.Events;

namespace BabylonArchiveCore.Runtime.Progression;

public sealed class LevelUpEvent : IDomainEvent
{
    public required int OldLevel { get; init; }
    public required int NewLevel { get; init; }
    public required int StatPointsGranted { get; init; }
    public DateTime OccurredUtc { get; init; } = DateTime.UtcNow;
}
