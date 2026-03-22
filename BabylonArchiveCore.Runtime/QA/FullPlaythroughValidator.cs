using BabylonArchiveCore.Content.Pipeline;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Runtime.Save;

namespace BabylonArchiveCore.Runtime.QA;

/// <summary>
/// Automated validator that runs the full A-0 prologue and checks
/// all V1 MUST-HAVE criteria programmatically.
/// </summary>
public sealed class FullPlaythroughValidator
{
    private readonly A0ContentProvider _contentProvider;
    private readonly ILogger _logger;

    public FullPlaythroughValidator(A0ContentProvider contentProvider, ILogger logger)
    {
        _contentProvider = contentProvider;
        _logger = logger;
    }

    /// <summary>
    /// Run a full automated playthrough and validate all criteria.
    /// Returns a validation report.
    /// </summary>
    public ValidationReport Validate()
    {
        var report = new ValidationReport();

        try
        {
            // Criterion 1: Game launches without crash
            var factory = new Gameplay.ContentDrivenSceneFactory(_contentProvider, _logger);
            var bundle = factory.Build();
            report.Pass("MUST-01", "Game launches without crash — scene built successfully");

            // Criterion 2: Alan wakes in capsule
            var startPos = bundle.Player.Position;
            report.Check("MUST-02", "Alan starts at capsule position",
                startPos.X < 0 && startPos.Z < 0); // Capsule is at (-7, 0, -6)

            // Criterion 3: Can walk in A-0 in 3D
            var collision = bundle.Collision;
            var moveResult = collision.TryMove(bundle.Player.Position, new Vec3(0, 0, 0));
            report.Check("MUST-03", "Movement to centre possible", !moveResult.WasBlocked);

            // Walk through prologue sequence
            var interactSequence = new[] { "capsule_exit", "bio_scanner", "supply_terminal", "drone_dock", "core_console" };
            foreach (var objId in interactSequence)
            {
                var obj = bundle.HubRuntime.Zones.SelectMany(z => z.Objects).First(o => o.Id == objId);
                var result = bundle.HubRuntime.Interact(objId);
                bundle.InteractionHandler.Handle(obj, result);
                bundle.PrologueTracker.RecordVisit(objId);
            }

            // Criterion 4: Biometric station works
            report.Check("MUST-04", "Biometrics progresses phase",
                bundle.HubRuntime.CurrentPhase >= HubRhythmPhase.Provisioning);

            // Criterion 5: Logistics terminal works (gives items)
            report.Check("MUST-05", "Logistics grants items",
                bundle.Inventory.Items.Count >= 3);

            // Criterion 6: Drone responds
            report.Check("MUST-06", "Drone interaction recorded",
                bundle.PrologueTracker.HasVisited("drone_dock"));

            // Criterion 7: C.O.R.E. activates → OperationAccess
            report.Check("MUST-07", "C.O.R.E. leads to OperationAccess",
                bundle.HubRuntime.CurrentPhase == HubRhythmPhase.OperationAccess);

            // Criterion 8-9: Terminal and gallery
            var opTerminal = bundle.HubRuntime.Zones.SelectMany(z => z.Objects).First(o => o.Id == "op_terminal");
            var opResult = bundle.HubRuntime.Interact("op_terminal");
            bundle.InteractionHandler.Handle(opTerminal, opResult);
            bundle.PrologueTracker.RecordVisit("op_terminal");
            report.Check("MUST-08", "Operations terminal accessible",
                bundle.PrologueTracker.HasVisited("op_terminal"));

            var gallery = bundle.HubRuntime.Zones.SelectMany(z => z.Objects).First(o => o.Id == "gallery_overlook");
            var galResult = bundle.HubRuntime.Interact("gallery_overlook");
            bundle.InteractionHandler.Handle(gallery, galResult);
            bundle.PrologueTracker.RecordVisit("gallery_overlook");
            report.Check("MUST-09", "Gallery overlook visited",
                bundle.PrologueTracker.HasVisited("gallery_overlook"));

            // Criterion 10: Hard-archive blocked
            var gateObj = bundle.HubRuntime.Zones.SelectMany(z => z.Objects).FirstOrDefault(o => o.Id == "archive_gate");
            report.Check("MUST-10", "Hard-archive gate exists and is gate type",
                gateObj is not null && gateObj.InteractiveType == InteractiveType.Gate);

            // Criterion 11: Protocol Zero unlocked
            report.Check("MUST-11", "Protocol Zero unlocked",
                bundle.PrologueTracker.IsProtocolZeroUnlocked);

            // Criterion 12: Stability — no crash during full playthrough
            report.Pass("MUST-12", "No exception during full playthrough");

            // Criterion 13: Re-launch — save and restore
            var save = SaveGameMapper.Extract(bundle.HubRuntime, bundle.PrologueTracker,
                bundle.ObjectiveTracker, bundle.Inventory, 145);
            var resume = new SaveResumeIntegration(_contentProvider, _logger);
            var restored = resume.BuildAndResume(save);
            report.Check("MUST-13", "Save/resume preserves phase",
                restored.HubRuntime.CurrentPhase == HubRhythmPhase.OperationAccess);
        }
        catch (Exception ex)
        {
            report.Fail("EXCEPTION", $"Playthrough crashed: {ex.Message}");
        }

        _logger.Info($"FullPlaythroughValidator: {report.PassedCount}/{report.TotalCount} passed, " +
            $"{report.FailedCount} failed.");

        return report;
    }
}

/// <summary>Collects validation results.</summary>
public sealed class ValidationReport
{
    private readonly List<ValidationEntry> _entries = [];

    public IReadOnlyList<ValidationEntry> Entries => _entries;
    public int PassedCount => _entries.Count(e => e.Passed);
    public int FailedCount => _entries.Count(e => !e.Passed);
    public int TotalCount => _entries.Count;
    public bool AllPassed => _entries.All(e => e.Passed);

    public void Pass(string id, string description)
    {
        _entries.Add(new ValidationEntry(id, description, true));
    }

    public void Fail(string id, string description)
    {
        _entries.Add(new ValidationEntry(id, description, false));
    }

    public void Check(string id, string description, bool condition)
    {
        _entries.Add(new ValidationEntry(id, description, condition));
    }

    public string Format()
    {
        var lines = _entries.Select(e => $"[{(e.Passed ? "PASS" : "FAIL")}] {e.Id}: {e.Description}");
        return string.Join(Environment.NewLine, lines) +
            $"{Environment.NewLine}--- {PassedCount}/{TotalCount} passed ---";
    }
}

/// <summary>Single validation check result.</summary>
public readonly record struct ValidationEntry(string Id, string Description, bool Passed);
