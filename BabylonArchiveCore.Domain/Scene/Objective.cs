namespace BabylonArchiveCore.Domain.Scene;

/// <summary>
/// A single trackable objective in the prologue sequence.
/// </summary>
public sealed class Objective
{
    public required string ObjectiveId { get; init; }
    public required string Text { get; init; }
    public required int Order { get; init; }
    public ObjectiveStatus Status { get; set; } = ObjectiveStatus.Locked;
}

public enum ObjectiveStatus
{
    Locked = 0,
    Active = 1,
    Completed = 2,
}
