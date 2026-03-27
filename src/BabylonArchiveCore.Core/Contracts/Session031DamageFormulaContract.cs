namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S031: параметры формул урона.
/// </summary>
public sealed class Session031DamageFormulaContract
{
    public required string FormulaId { get; init; }

    public float AttackPower { get; init; }

    public float SkillMultiplier { get; init; }

    public float CritChance { get; init; }

    public float CritMultiplier { get; init; }

    public float ArmorPenetration { get; init; }
}
