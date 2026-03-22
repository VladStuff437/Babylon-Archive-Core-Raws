using BabylonArchiveCore.Core.Events;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain.Narrative;

namespace BabylonArchiveCore.Runtime.Narrative;

/// <summary>
/// Tracks collected intervention evidence, manages signatures,
/// and computes the player's intervention certainty level.
/// Publishes events when certainty changes.
/// </summary>
public sealed class InterventionTracker
{
    private readonly ILogger _logger;
    private readonly EventBus _eventBus;

    private readonly List<InterventionEvidence> _evidence = new();
    private readonly Dictionary<string, InterventionSignature> _signatures = new();

    // Severity score thresholds for certainty levels
    private const int SuspiciousThreshold = 3;
    private const int InvestigatingThreshold = 8;
    private const int ConvincedThreshold = 15;
    private const int ProvenThreshold = 25;

    public InterventionCertainty Certainty { get; private set; } = InterventionCertainty.Unaware;
    public IReadOnlyList<InterventionEvidence> Evidence => _evidence;
    public IReadOnlyDictionary<string, InterventionSignature> Signatures => _signatures;
    public int TotalSeverityScore => _evidence.Sum(e => e.Severity);

    public InterventionTracker(ILogger logger, EventBus eventBus)
    {
        _logger = logger;
        _eventBus = eventBus;
    }

    /// <summary>
    /// Register a signature pattern to watch for.
    /// </summary>
    public void RegisterSignature(InterventionSignature signature)
    {
        _signatures[signature.PatternId] = signature;
        _logger.Info($"Intervention: registered signature '{signature.PatternId}' (threshold={signature.ConfirmationThreshold}).");
    }

    /// <summary>
    /// Add a piece of evidence. Updates certainty level and checks signatures.
    /// Optionally associates with a signature pattern.
    /// </summary>
    public void AddEvidence(InterventionEvidence evidence, string? signaturePatternId = null)
    {
        _evidence.Add(evidence);
        _logger.Info($"Intervention evidence: '{evidence.Id}' at {evidence.Address}, severity={evidence.Severity}. " +
                     $"Terminal='{evidence.TerminalData}' vs Archive='{evidence.ArchiveData}'.");

        // Associate with signature if specified
        if (signaturePatternId is not null && _signatures.TryGetValue(signaturePatternId, out var sig))
        {
            if (!sig.DetectedAtAddresses.Contains(evidence.Address))
            {
                sig.DetectedAtAddresses.Add(evidence.Address);
                _logger.Info($"Intervention: signature '{signaturePatternId}' detected at {evidence.Address} " +
                             $"({sig.DetectedAtAddresses.Count}/{sig.ConfirmationThreshold}).");

                if (sig.IsConfirmed)
                    _logger.Info($"Intervention: signature '{signaturePatternId}' CONFIRMED.");
            }
        }

        // Recalculate certainty
        var oldCertainty = Certainty;
        Certainty = ComputeCertainty();

        if (Certainty != oldCertainty)
        {
            _logger.Info($"Intervention certainty: {oldCertainty} → {Certainty} (score={TotalSeverityScore}).");
            _eventBus.Publish(new InterventionCertaintyChangedEvent
            {
                OldLevel = oldCertainty.ToString(),
                NewLevel = Certainty.ToString(),
                TotalEvidenceScore = TotalSeverityScore,
            });
        }
    }

    /// <summary>
    /// Get all confirmed signature patterns.
    /// </summary>
    public List<InterventionSignature> GetConfirmedSignatures() =>
        _signatures.Values.Where(s => s.IsConfirmed).ToList();

    private InterventionCertainty ComputeCertainty()
    {
        var score = TotalSeverityScore;
        return score switch
        {
            >= ProvenThreshold => InterventionCertainty.Proven,
            >= ConvincedThreshold => InterventionCertainty.Convinced,
            >= InvestigatingThreshold => InterventionCertainty.Investigating,
            >= SuspiciousThreshold => InterventionCertainty.Suspicious,
            _ => InterventionCertainty.Unaware,
        };
    }
}
