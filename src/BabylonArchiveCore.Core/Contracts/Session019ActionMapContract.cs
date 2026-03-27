namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S019: action map профиля ввода.
/// </summary>
public sealed class Session019ActionMapContract
{
    public required string ProfileId { get; init; }

    public required string DefaultActionSet { get; init; }

    public required Dictionary<string, string> Bindings { get; init; }

    public required bool IsComposite { get; init; }

    public string? FallbackProfileId { get; init; }
}
