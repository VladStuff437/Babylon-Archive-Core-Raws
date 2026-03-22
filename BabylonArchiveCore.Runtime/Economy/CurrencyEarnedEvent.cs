using BabylonArchiveCore.Core.Events;
using BabylonArchiveCore.Domain.Economy;

namespace BabylonArchiveCore.Runtime.Economy;

public sealed class CurrencyEarnedEvent : IDomainEvent
{
    public required CurrencyType Currency { get; init; }
    public required int Amount { get; init; }
    public required int NewBalance { get; init; }
    public required string Source { get; init; }
    public DateTime OccurredUtc { get; init; } = DateTime.UtcNow;
}
