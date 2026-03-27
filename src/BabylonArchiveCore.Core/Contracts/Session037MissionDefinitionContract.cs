namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S037: финализированная структура MissionDefinition.
/// </summary>
public sealed class Session037MissionDefinitionContract
{
    public required string MissionId { get; init; }

    public required string Title { get; init; }

    public required string StartNodeId { get; init; }

    public int ContractVersion { get; init; } = 37;

    public required string[] NodeIds { get; init; }
}
