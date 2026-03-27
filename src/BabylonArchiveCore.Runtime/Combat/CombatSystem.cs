using BabylonArchiveCore.Runtime.Combat.StatusEffects;

namespace BabylonArchiveCore.Runtime.Combat;

/// <summary>
/// Боевой цикл: Direct → AOE → Status → Resolve.
/// </summary>
public sealed class CombatSystem
{
    private readonly StatusEffectManager _statusEffectManager;

    public CombatSystem(StatusEffectManager statusEffectManager)
    {
        ArgumentNullException.ThrowIfNull(statusEffectManager);
        _statusEffectManager = statusEffectManager;
    }

    public CombatResult ResolveTurn(int directDamage, int aoeDamage)
    {
        var context = new CombatTurnContext
        {
            DirectBaseDamage = directDamage,
            AoeBaseDamage = aoeDamage
        };

        return ResolveTurn(context);
    }

    public CombatResult ResolveTurn(CombatTurnContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Phase 1: Direct
        int totalDirect = Math.Max(0, context.DirectBaseDamage + context.DirectDamageModifier);
        // Phase 2: AOE
        int totalAoe = Math.Max(0, context.AoeBaseDamage + context.AoeDamageModifier);
        // Phase 3: Status tick
        int statusDamage = _statusEffectManager.TickAll();
        // Mitigation / crit
        int mitigatedDirect = ApplyMitigation(totalDirect, context.TargetArmor);
        int mitigatedAoe = ApplyMitigation(totalAoe, context.TargetArmor);
        if (context.IsCritical)
        {
            mitigatedDirect = (int)Math.Round(mitigatedDirect * context.CriticalMultiplier);
        }

        int total = mitigatedDirect + mitigatedAoe + statusDamage;
        // Phase 4: Resolve
        return new CombatResult
        {
            DirectDamage = mitigatedDirect,
            AoeDamage = mitigatedAoe,
            StatusDamage = statusDamage,
            TotalDamage = total
        };
    }

    private static int ApplyMitigation(int damage, int armor)
    {
        if (damage <= 0)
        {
            return 0;
        }

        int mitigated = damage - Math.Max(0, armor);
        return Math.Max(0, mitigated);
    }
}

public sealed class CombatTurnContext
{
    public int DirectBaseDamage { get; init; }

    public int AoeBaseDamage { get; init; }

    public int DirectDamageModifier { get; init; }

    public int AoeDamageModifier { get; init; }

    public int TargetArmor { get; init; }

    public bool IsCritical { get; init; }

    public double CriticalMultiplier { get; init; } = 1.5d;
}

public sealed class CombatResult
{
    public int DirectDamage { get; init; }
    public int AoeDamage { get; init; }
    public int StatusDamage { get; init; }
    public int TotalDamage { get; init; }
}
