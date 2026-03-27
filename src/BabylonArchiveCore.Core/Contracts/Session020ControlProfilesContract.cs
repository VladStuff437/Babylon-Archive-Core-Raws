namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S020: консолидация профилей управления.
/// </summary>
public sealed class Session020ControlProfilesContract
{
    public required string ActiveProfileId { get; init; }

    public required string FallbackProfileId { get; init; }

    public required string[] ProfileChain { get; init; }

    public required bool StrictResolution { get; init; }

    public string? DeviceTag { get; init; }
}
