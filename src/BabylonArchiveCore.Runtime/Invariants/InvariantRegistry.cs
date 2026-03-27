using BabylonArchiveCore.Core.Invariants;

namespace BabylonArchiveCore.Runtime.Invariants;

/// <summary>
/// Реализация реестра инвариантов (ADR-005).
/// Runtime собирает все чекеры и позволяет запускать проверки.
/// </summary>
public sealed class InvariantRegistry : IInvariantRegistry
{
    private readonly List<IInvariantChecker> _checkers = new();

    public IReadOnlyList<IInvariantChecker> Checkers => _checkers.AsReadOnly();

    public void Register(IInvariantChecker checker)
    {
        ArgumentNullException.ThrowIfNull(checker);
        _checkers.Add(checker);
    }

    public IReadOnlyList<InvariantResult> CheckAll()
    {
        var results = new List<InvariantResult>(_checkers.Count);
        foreach (var checker in _checkers)
        {
            results.Add(checker.Check());
        }
        return results.AsReadOnly();
    }

    public InvariantResult? CheckById(string invariantId)
    {
        var checker = _checkers.Find(c => c.InvariantId == invariantId);
        return checker?.Check();
    }
}
