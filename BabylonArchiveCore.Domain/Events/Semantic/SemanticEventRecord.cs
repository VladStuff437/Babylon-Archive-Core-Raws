namespace BabylonArchiveCore.Domain.Events.Semantic;

public sealed class SemanticEventRecord
{
    public string EventId { get; init; } = string.Empty;
    public EventSemanticKind Kind { get; init; }
    public string SourceObjectId { get; init; } = string.Empty;
    public string PayloadText { get; init; } = string.Empty;
    public DateTimeOffset RaisedAt { get; init; } = DateTimeOffset.UtcNow;
}
