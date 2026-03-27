namespace BabylonArchiveCore.Core.Invariants;

/// <summary>
/// Контракт проверки архитектурного инварианта.
/// Каждый INV-xxx реализуется отдельным чекером (ADR-005).
/// </summary>
public interface IInvariantChecker
{
    /// <summary>Идентификатор инварианта (INV-001, INV-002, ...).</summary>
    string InvariantId { get; }

    /// <summary>Описание инварианта.</summary>
    string Description { get; }

    /// <summary>Критичность нарушения.</summary>
    InvariantSeverity Severity { get; }

    /// <summary>Выполнить проверку инварианта.</summary>
    InvariantResult Check();
}
