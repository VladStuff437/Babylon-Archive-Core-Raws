using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain;
using BabylonArchiveCore.Domain.Archive;

namespace BabylonArchiveCore.Runtime.QA;

/// <summary>
/// Save game integrity validator: checks version, address validity,
/// and seed reproducibility on load.
/// </summary>
public static class SaveIntegrityValidator
{
    public sealed class ValidationResult
    {
        public bool IsValid { get; init; }
        public List<string> Errors { get; init; } = new();
        public List<string> Warnings { get; init; } = new();
    }

    public static ValidationResult Validate(SaveGame save, int expectedVersion, ILogger logger)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        // Version check
        if (save.Version != expectedVersion)
            errors.Add($"Save version mismatch: expected {expectedVersion}, got {save.Version}.");

        // Operator name
        if (string.IsNullOrWhiteSpace(save.OperatorName))
            warnings.Add("Operator name is empty.");

        // Archive address validity
        try
        {
            var addr = ArchiveAddress.Parse(save.WorldSeedAddress);
            var seed = addr.ToSeed(save.WorldSeed);
            if (seed == 0)
                warnings.Add("Computed seed is 0 — may produce degenerate content.");
        }
        catch (Exception ex)
        {
            errors.Add($"Invalid WorldSeedAddress '{save.WorldSeedAddress}': {ex.Message}");
        }

        // World seed sanity
        if (save.WorldSeed == 0)
            warnings.Add("WorldSeed is 0 — default/uninitialized?");

        var result = new ValidationResult
        {
            IsValid = errors.Count == 0,
            Errors = errors,
            Warnings = warnings,
        };

        foreach (var e in errors) logger.Error($"SaveValidation: {e}");
        foreach (var w in warnings) logger.Warn($"SaveValidation: {w}");
        if (result.IsValid) logger.Info("SaveValidation: PASSED.");

        return result;
    }
}
