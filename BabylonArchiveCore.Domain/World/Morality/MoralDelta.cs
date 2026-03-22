namespace BabylonArchiveCore.Domain.World.Morality;

public sealed record MoralDelta(
    int Care,
    int Truth,
    int Responsibility,
    int Respect,
    int ArchiveIntegrity,
    int Insight);
