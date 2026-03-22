using BabylonArchiveCore.Core.Events;
using BabylonArchiveCore.Domain;

namespace BabylonArchiveCore.Runtime.Events;

public sealed class SceneChangedEvent : IDomainEvent
{
    public required SceneId From { get; init; }

    public required SceneId To { get; init; }

    public DateTime OccurredUtc { get; init; } = DateTime.UtcNow;
}
