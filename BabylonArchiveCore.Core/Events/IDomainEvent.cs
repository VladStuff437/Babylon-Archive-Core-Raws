namespace BabylonArchiveCore.Core.Events;

public interface IDomainEvent
{
    DateTime OccurredUtc { get; }
}
