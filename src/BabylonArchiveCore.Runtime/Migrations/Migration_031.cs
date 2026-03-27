using BabylonArchiveCore.Core.Contracts;

namespace BabylonArchiveCore.Runtime.Migrations;

/// <summary>
/// Миграция S031: формулы урона.
/// </summary>
public sealed class Migration_031
{
    public Session031DamageFormulaContract Migrate(object? legacyState)
    {
        if (legacyState is Session031DamageFormulaContract existing)
        {
            return existing;
        }

        return new Session031DamageFormulaContract
        {
            FormulaId = "default",
            AttackPower = 10f,
            SkillMultiplier = 1f,
            CritChance = 0.05f,
            CritMultiplier = 1.5f,
            ArmorPenetration = 0f
        };
    }
}
