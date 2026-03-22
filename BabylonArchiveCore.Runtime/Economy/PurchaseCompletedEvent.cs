using BabylonArchiveCore.Core.Events;
using BabylonArchiveCore.Domain.Economy;

namespace BabylonArchiveCore.Runtime.Economy;

public sealed class PurchaseCompletedEvent : IDomainEvent
{
    public required string ItemId { get; init; }
    public required CurrencyType CurrencyUsed { get; init; }
    public required int AmountCharged { get; init; }
    public DateTime OccurredUtc { get; init; } = DateTime.UtcNow;
}
