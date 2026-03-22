using BabylonArchiveCore.Content.Pipeline;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Runtime.Gameplay;
using BabylonArchiveCore.Runtime.QA;
using BabylonArchiveCore.Runtime.Save;

namespace BabylonArchiveCore.Runtime.QA;

/// <summary>
/// A single end-to-end smoke test that validates the complete player journey
/// from game launch to Protocol Zero in one deterministic run.
/// Designed to catch regressions across all subsystems.
/// </summary>
public sealed class EndToEndSmokeTest
{
    private readonly A0ContentProvider _contentProvider;
    private readonly ILogger _logger;

    public EndToEndSmokeTest(A0ContentProvider contentProvider, ILogger logger)
    {
        _contentProvider = contentProvider;
        _logger = logger;
    }

    /// <summary>
    /// Execute the full smoke test. Returns a result summary.
    /// Throws if any critical assertion fails.
    /// </summary>
    public SmokeTestResult Run()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var checks = new List<string>();

        // 1. Build scene
        var runner = new PrologueRunner(_contentProvider, _logger);
        checks.Add("Scene built");

        // 2. Run full prologue
        var steps = runner.RunToCompletion();
        checks.Add($"Prologue completed: {steps.Count} steps");

        var bundle = runner.Bundle;

        // 3. Verify final state
        if (runner.State != PrologueState.Completed)
            throw new InvalidOperationException("Prologue did not reach Completed state");
        checks.Add("State = Completed");

        if (bundle.HubRuntime.CurrentPhase != HubRhythmPhase.OperationAccess)
            throw new InvalidOperationException($"Phase is {bundle.HubRuntime.CurrentPhase}, expected OperationAccess");
        checks.Add("Phase = OperationAccess");

        if (!bundle.PrologueTracker.IsProtocolZeroUnlocked)
            throw new InvalidOperationException("Protocol Zero not unlocked");
        checks.Add("Protocol Zero unlocked");

        if (bundle.Inventory.Items.Count < 3)
            throw new InvalidOperationException($"Inventory has {bundle.Inventory.Items.Count} items, expected ≥3");
        checks.Add($"Inventory: {bundle.Inventory.Items.Count} items");

        // 4. Verify all 7 prologue steps were visited
        foreach (var objId in PrologueRunner.PrologueSequence)
        {
            if (!bundle.PrologueTracker.HasVisited(objId))
                throw new InvalidOperationException($"Object {objId} was not visited");
        }
        checks.Add("All 7 prologue objects visited");

        // 5. Verify cutscene frames existed
        var cutsceneSteps = steps.Count(s => s.State == PrologueState.Cutscene);
        if (cutsceneSteps < 2)
            throw new InvalidOperationException($"Only {cutsceneSteps} cutscene frames");
        checks.Add($"Cutscene: {cutsceneSteps} frames");

        // 6. Verify hints were shown
        var hintSteps = steps.Count(s => s.HintText is not null);
        if (hintSteps == 0)
            throw new InvalidOperationException("No hints were shown during prologue");
        checks.Add($"Hints shown: {hintSteps} steps");

        // 7. Verify objectives were tracked
        var objSteps = steps.Count(s => s.ObjectiveText is not null && s.ObjectiveText.Length > 0);
        checks.Add($"Objectives shown: {objSteps} steps");

        // 8. Save → Resume round-trip
        var save = SaveGameMapper.Extract(bundle.HubRuntime, bundle.PrologueTracker,
            bundle.ObjectiveTracker, bundle.Inventory, 145);
        var resume = new SaveResumeIntegration(_contentProvider, _logger);
        var restored = resume.BuildAndResume(save);
        if (restored.HubRuntime.CurrentPhase != HubRhythmPhase.OperationAccess)
            throw new InvalidOperationException("Save/Resume lost phase");
        checks.Add("Save/Resume round-trip OK");

        // 9. Journal entries
        if (bundle.Journal.Entries.Count == 0)
            throw new InvalidOperationException("Journal has no entries");
        checks.Add($"Journal: {bundle.Journal.Entries.Count} entries");

        sw.Stop();

        return new SmokeTestResult(
            Passed: true,
            TotalSteps: steps.Count,
            CutsceneFrames: cutsceneSteps,
            InteractionSteps: steps.Count(s => s.InteractedObjectId is not null),
            InventoryCount: bundle.Inventory.Items.Count,
            JournalEntries: bundle.Journal.Entries.Count,
            ElapsedMs: sw.ElapsedMilliseconds,
            Checks: checks);
    }
}

/// <summary>Result of the end-to-end smoke test.</summary>
public sealed record SmokeTestResult(
    bool Passed,
    int TotalSteps,
    int CutsceneFrames,
    int InteractionSteps,
    int InventoryCount,
    int JournalEntries,
    long ElapsedMs,
    List<string> Checks);
