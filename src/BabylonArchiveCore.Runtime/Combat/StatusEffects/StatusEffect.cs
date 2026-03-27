namespace BabylonArchiveCore.Runtime.Combat.StatusEffects;

/// <summary>
/// Один статусный эффект с тиком и автоматическим снятием.
/// </summary>
public sealed class StatusEffect
{
    public required string EffectId { get; init; }
    public required string Name { get; init; }
    public string Category { get; init; } = "generic";
    public int RemainingTicks { get; set; }
    public int DamagePerTick { get; init; }
    public int StackCount { get; private set; } = 1;
    public int MaxStacks { get; init; } = 1;
    public bool IsDebuff { get; init; } = true;
    public bool IsExpired => RemainingTicks <= 0;

    public int Tick()
    {
        if (IsExpired) return 0;
        RemainingTicks--;
        return DamagePerTick * StackCount;
    }

    public bool TryAddStack()
    {
        if (StackCount >= MaxStacks)
        {
            return false;
        }

        StackCount++;
        return true;
    }
}
