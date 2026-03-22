namespace BabylonArchiveCore.Domain.World.Morality;

public sealed class MoralRule
{
    public string EventId { get; init; } = string.Empty;
    public MoralDelta Delta { get; init; } = new(0, 0, 0, 0, 0, 0);
    public IReadOnlyList<string> Tags { get; init; } = [];
}
