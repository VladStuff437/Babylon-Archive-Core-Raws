using BabylonArchiveCore.Core.Events;

namespace BabylonArchiveCore.Runtime.Narrative;

/// <summary>Published when a dialogue option with a skill check is attempted.</summary>
public sealed class DialogueCheckEvent : IDomainEvent
{
    public DateTime OccurredUtc { get; init; } = DateTime.UtcNow;
    public required string DialogueId { get; init; }
    public required string OptionId { get; init; }
    public required string CheckType { get; init; }
    public required int Difficulty { get; init; }
    public required int PlayerStat { get; init; }
    public required bool Passed { get; init; }
}
