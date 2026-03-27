using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S024: атрибуты персонажа.
/// </summary>
public sealed class Migration_024
{
    public Session024CharacterAttributesContract Migrate(object? legacyState)
    {
        if (legacyState is Session024CharacterAttributesContract existing)
        {
            return existing;
        }

        return new Session024CharacterAttributesContract
        {
            PlayerId = "player-1",
            Strength = 10,
            Agility = 10,
            Intellect = 10,
            Vitality = 10,
            UnspentPoints = 0
        };
    }
}
