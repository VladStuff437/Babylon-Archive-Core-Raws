namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Контракт S026: ядро боевой системы.
/// </summary>
public sealed class Session026CombatCoreContract
{
    public required string AttackerId { get; init; }
    public required string TargetId { get; init; }
    public int DirectBaseDamage { get; init; }
    public int AoeBaseDamage { get; init; }
    public int TargetArmor { get; init; }
    public bool IsCritical { get; init; }
}
