namespace BabylonArchiveCore.Runtime.Combat;

/// <summary>
/// Калькулятор урона S031/S032 с покрытием граничных случаев.
/// </summary>
public sealed class DamageCalculator
{
    public int CalculateDamage(DamageFormulaInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var attackPower = Math.Max(0f, input.AttackPower);
        var skillMultiplier = Math.Max(0f, input.SkillMultiplier);
        var armor = Math.Max(0f, input.TargetArmor);
        var armorPenetration = Math.Max(0f, input.ArmorPenetration);

        var effectiveArmor = Math.Max(0f, armor - armorPenetration);
        var raw = attackPower * skillMultiplier;
        var mitigated = Math.Max(0f, raw - effectiveArmor);

        if (input.ForceCritical)
        {
            mitigated *= Math.Max(1f, input.CriticalMultiplier);
        }

        mitigated *= Math.Max(0f, input.BalanceMultiplier);

        var rounded = (int)Math.Round(mitigated, MidpointRounding.AwayFromZero);
        return Math.Max(input.MinimumDamage, rounded);
    }

    public int CalculateDamageWithContext(DamageFormulaInput input, DamageContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var baseline = CalculateDamage(input);

        // Reputation and world-axis influence is intentionally low-impact for v1 economy/faction balancing.
        var factionFactor = 1f + Math.Clamp(context.ReputationDelta / 500f, -0.2f, 0.2f);
        var axisFactor = 1f + Math.Clamp((context.MoralAxis + context.TechnoArcaneAxis) / 1000f, -0.2f, 0.2f);

        var computed = baseline * factionFactor * axisFactor;
        var rounded = (int)Math.Round(computed, MidpointRounding.AwayFromZero);
        return Math.Max(input.MinimumDamage, rounded);
    }

    public int CalculateDamageFromBalance(DamageFormulaInput input, Balance.BalanceTable balanceTable, DamageContext? context = null)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(balanceTable);

        var adjustedInput = new DamageFormulaInput
        {
            AttackPower = input.AttackPower,
            SkillMultiplier = input.SkillMultiplier,
            TargetArmor = input.TargetArmor,
            ArmorPenetration = input.ArmorPenetration,
            ForceCritical = input.ForceCritical,
            CriticalMultiplier = input.CriticalMultiplier,
            BalanceMultiplier = input.BalanceMultiplier * balanceTable.GetScalar("damageMultipliers.player", 1f),
            MinimumDamage = Math.Max(input.MinimumDamage, (int)Math.Round(balanceTable.GetScalar("damage.minDamage", 0f), MidpointRounding.AwayFromZero))
        };

        return context is null
            ? CalculateDamage(adjustedInput)
            : CalculateDamageWithContext(adjustedInput, context);
    }
}

public sealed class DamageFormulaInput
{
    public float AttackPower { get; init; }

    public float SkillMultiplier { get; init; } = 1f;

    public float TargetArmor { get; init; }

    public float ArmorPenetration { get; init; }

    public bool ForceCritical { get; init; }

    public float CriticalMultiplier { get; init; } = 1.5f;

    public float BalanceMultiplier { get; init; } = 1f;

    public int MinimumDamage { get; init; } = 0;
}

public sealed class DamageContext
{
    public int ReputationDelta { get; init; }

    public float MoralAxis { get; init; }

    public float TechnoArcaneAxis { get; init; }
}
