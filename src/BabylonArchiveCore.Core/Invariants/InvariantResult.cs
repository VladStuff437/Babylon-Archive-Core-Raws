namespace BabylonArchiveCore.Core.Invariants;

/// <summary>
/// Результат проверки инварианта.
/// Immutable value object (ADR-005).
/// </summary>
public sealed class InvariantResult
{
    public string InvariantId { get; }
    public bool Passed { get; }
    public string Message { get; }
    public InvariantSeverity Severity { get; }

    private InvariantResult(string invariantId, bool passed, string message, InvariantSeverity severity)
    {
        InvariantId = invariantId;
        Passed = passed;
        Message = message;
        Severity = severity;
    }

    public static InvariantResult Pass(string invariantId, string message = "OK")
        => new(invariantId, true, message, InvariantSeverity.Info);

    public static InvariantResult Fail(string invariantId, string message, InvariantSeverity severity = InvariantSeverity.Critical)
        => new(invariantId, false, message, severity);

    public override string ToString()
        => $"[{InvariantId}] {(Passed ? "PASS" : "FAIL")} ({Severity}): {Message}";
}
