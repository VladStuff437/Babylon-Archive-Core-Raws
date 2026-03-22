namespace BabylonArchiveCore.Domain.World.Morality;

public sealed class MoralLogEntry
{
    public string EventId { get; init; } = string.Empty;
    public string SourceObjectId { get; init; } = string.Empty;
    public MoralDelta Delta { get; init; } = new(0, 0, 0, 0, 0, 0);
    public DateTimeOffset At { get; init; } = DateTimeOffset.UtcNow;
    public IReadOnlyList<string> Tags { get; init; } = [];
}
