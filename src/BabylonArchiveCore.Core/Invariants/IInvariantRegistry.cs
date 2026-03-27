namespace BabylonArchiveCore.Core.Invariants;

/// <summary>
/// Реестр всех invariant чекеров. Позволяет запустить все проверки разом (ADR-005).
/// Определён в Core как контракт, реализация — в Runtime.
/// </summary>
public interface IInvariantRegistry
{
    /// <summary>Зарегистрировать чекер.</summary>
    void Register(IInvariantChecker checker);

    /// <summary>Запустить все зарегистрированные проверки.</summary>
    IReadOnlyList<InvariantResult> CheckAll();

    /// <summary>Запустить проверку конкретного инварианта по Id.</summary>
    InvariantResult? CheckById(string invariantId);

    /// <summary>Все зарегистрированные чекеры.</summary>
    IReadOnlyList<IInvariantChecker> Checkers { get; }
}
