namespace BabylonArchiveCore.Core.State;

/// <summary>
/// Базовое состояние игрока с атрибутами и валидацией диапазонов.
/// </summary>
public sealed class PlayerState
{
    public string PlayerId { get; set; } = "player-1";
    public int Health { get; private set; } = 100;
    public int MaxHealth { get; private set; } = 100;
    public int Stamina { get; private set; } = 50;
    public int MaxStamina { get; private set; } = 50;
    public int Level { get; private set; } = 1;
    public int Experience { get; private set; }
    public int Strength { get; private set; } = 10;
    public int Agility { get; private set; } = 10;
    public int Intellect { get; private set; } = 10;
    public int Vitality { get; private set; } = 10;

    public void SetHealth(int value) => Health = Math.Clamp(value, 0, MaxHealth);
    public void SetMaxHealth(int value) { MaxHealth = Math.Max(1, value); Health = Math.Min(Health, MaxHealth); }
    public void SetStamina(int value) => Stamina = Math.Clamp(value, 0, MaxStamina);
    public void SetMaxStamina(int value) { MaxStamina = Math.Max(1, value); Stamina = Math.Min(Stamina, MaxStamina); }
    public void SetLevel(int value) => Level = Math.Max(1, value);
    public void AddExperience(int value) => Experience = Math.Max(0, Experience + value);
    public void SetStrength(int value) => Strength = ClampAttribute(value);
    public void SetAgility(int value) => Agility = ClampAttribute(value);
    public void SetIntellect(int value) => Intellect = ClampAttribute(value);
    public void SetVitality(int value) => Vitality = ClampAttribute(value);

    private static int ClampAttribute(int value) => Math.Clamp(value, 1, 100);
}
