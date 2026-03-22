namespace BabylonArchiveCore.Domain.Scene;

/// <summary>
/// Data model for a terminal screen shown when a Terminal-type object is activated.
/// </summary>
public sealed class TerminalScreen
{
    public required string TerminalId { get; init; }
    public required string Title { get; init; }
    public required IReadOnlyList<string> Lines { get; init; }
    public string? DialogueId { get; init; }
}
