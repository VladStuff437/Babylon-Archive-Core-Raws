namespace BabylonArchiveCore.Core.Invariants;

/// <summary>
/// Критичность нарушения инварианта (ADR-005).
/// </summary>
public enum InvariantSeverity
{
    /// <summary>Информационная проверка.</summary>
    Info = 0,

    /// <summary>Предупреждение — потенциальная проблема.</summary>
    Warning = 1,

    /// <summary>Критическое нарушение — блокирующий дефект.</summary>
    Critical = 2
}
