namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S024: атрибуты персонажа.
/// </summary>
public sealed class Session024CharacterAttributesContract
{
    public required string PlayerId { get; init; }
    public int Strength { get; init; }
    public int Agility { get; init; }
    public int Intellect { get; init; }
    public int Vitality { get; init; }
    public int UnspentPoints { get; init; }
}
