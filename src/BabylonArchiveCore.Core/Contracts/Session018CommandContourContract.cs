namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S018: правила командного контура.
/// </summary>
public sealed class Session018CommandContourContract
{
    public required string CommandType { get; init; }

    public required string[] AllowedModes { get; init; }

    public required bool RequiresInitializedWorld { get; init; }

    public required bool IsIdempotent { get; init; }

    public string? QueueName { get; init; }
}
