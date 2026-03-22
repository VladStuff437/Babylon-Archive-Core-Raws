using BabylonArchiveCore.Content.Pipeline;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain.Player;
using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Runtime.Scene;

namespace BabylonArchiveCore.Runtime.Gameplay;

/// <summary>
/// Orchestrates the full A-0 prologue from capsule to Protocol Zero.
/// Combines cutscene, hints, objective HUD, interactions, and prologue tracking
/// into a single runnable pipeline.
/// </summary>
public sealed class PrologueRunner
{
    private readonly ContentDrivenSceneFactory.SceneBundle _bundle;
    private readonly CutsceneSequencer _cutscene;
    private readonly HintSystem _hints;
    private readonly ObjectiveHUD _objectiveHUD;
    private readonly ILogger _logger;

    public PrologueState State { get; private set; } = PrologueState.NotStarted;
    public ContentDrivenSceneFactory.SceneBundle Bundle => _bundle;
    public CutsceneSequencer Cutscene => _cutscene;
    public HintSystem Hints => _hints;
    public ObjectiveHUD ObjectiveHUD => _objectiveHUD;

    /// <summary>Canonical interaction sequence for the A-0 prologue.</summary>
    public static readonly string[] PrologueSequence =
    [
        "capsule_exit", "bio_scanner", "supply_terminal",
        "drone_dock", "core_console", "op_terminal", "gallery_overlook"
    ];

    public PrologueRunner(A0ContentProvider contentProvider, ILogger logger)
    {
        _logger = logger;

        // Build the full scene
        var factory = new ContentDrivenSceneFactory(contentProvider, logger);
        _bundle = factory.Build();

        // Set up cutscene: fly from above, swoop to capsule
        _cutscene = new CutsceneSequencer(
        [
            new CutsceneKeyframe(new Vec3(0, 20, -15), new Vec3(0, 0, 0), 0f),
            new CutsceneKeyframe(new Vec3(0, 14, -10), new Vec3(0, 0, 0), 1.5f),
            new CutsceneKeyframe(new Vec3(-5, 10, -8), new Vec3(-7, 0, -6), 3f),
            new CutsceneKeyframe(new Vec3(-7, 4, -8), new Vec3(-7, 0.5f, -6), 4.5f),
        ]);

        // Set up hints from all zones
        _hints = new HintSystem();
        _hints.RegisterFromZones(_bundle.HubRuntime.Zones);

        // Set up objective HUD
        _objectiveHUD = new ObjectiveHUD(_bundle.ObjectiveTracker);
    }

    /// <summary>Start the prologue: begins with the cutscene.</summary>
    public void Start()
    {
        State = PrologueState.Cutscene;
        _cutscene.Play();
        _logger.Info("PrologueRunner: started — cutscene playing.");
    }

    /// <summary>
    /// Advance the prologue by one step. During cutscene, advances camera.
    /// During gameplay, processes the next interaction in sequence.
    /// Returns a snapshot of the current state.
    /// </summary>
    public PrologueStepResult Step(float deltaTime = 1f / 60f)
    {
        switch (State)
        {
            case PrologueState.Cutscene:
                var frame = _cutscene.Update(deltaTime);
                if (frame.IsFinished)
                {
                    State = PrologueState.Playing;
                    _logger.Info("PrologueRunner: cutscene finished, gameplay begins.");
                }
                return new PrologueStepResult
                {
                    State = State,
                    CameraPosition = frame.Position,
                    CameraLookAt = frame.LookAt,
                    Phase = _bundle.HubRuntime.CurrentPhase,
                };

            case PrologueState.Playing:
                _objectiveHUD.Update();
                // Find the next unvisited interaction
                string? nextObjectId = null;
                foreach (var id in PrologueSequence)
                {
                    if (!_bundle.PrologueTracker.HasVisited(id))
                    {
                        nextObjectId = id;
                        break;
                    }
                }

                if (nextObjectId is null)
                {
                    State = PrologueState.Completed;
                    _logger.Info("PrologueRunner: prologue complete — Protocol Zero.");
                    return new PrologueStepResult
                    {
                        State = State,
                        Phase = _bundle.HubRuntime.CurrentPhase,
                        ObjectiveText = _objectiveHUD.DisplayText,
                        ProtocolZeroUnlocked = _bundle.PrologueTracker.IsProtocolZeroUnlocked,
                    };
                }

                // Hint for focused object
                _hints.Update(nextObjectId);

                // Interact
                var obj = _bundle.HubRuntime.Zones
                    .SelectMany(z => z.Objects)
                    .First(o => o.Id == nextObjectId);
                var baseResult = _bundle.HubRuntime.Interact(nextObjectId);
                var enriched = _bundle.InteractionHandler.Handle(obj, baseResult);
                _bundle.PrologueTracker.RecordVisit(nextObjectId);

                // XP
                var xp = OperatorIdentity.InteractionXp.GetValueOrDefault(nextObjectId, 0);

                _objectiveHUD.Update();

                _logger.Info($"PrologueRunner: [{nextObjectId}] phase={_bundle.HubRuntime.CurrentPhase}, " +
                    $"+{xp}XP, hint={_hints.ActiveHintText}, obj={_objectiveHUD.DisplayText}");

                return new PrologueStepResult
                {
                    State = State,
                    InteractedObjectId = nextObjectId,
                    InteractionMessage = enriched.Message,
                    Phase = _bundle.HubRuntime.CurrentPhase,
                    HintText = _hints.Format(),
                    ObjectiveText = _objectiveHUD.DisplayText,
                    XpGained = xp,
                    InventoryCount = _bundle.Inventory.Items.Count,
                    ProtocolZeroUnlocked = _bundle.PrologueTracker.IsProtocolZeroUnlocked,
                };

            default:
                return new PrologueStepResult { State = State };
        }
    }

    /// <summary>Skip the cutscene and jump directly to gameplay.</summary>
    public void SkipCutscene()
    {
        if (State == PrologueState.Cutscene)
        {
            _cutscene.Skip();
            State = PrologueState.Playing;
            _logger.Info("PrologueRunner: cutscene skipped.");
        }
    }

    /// <summary>
    /// Run the entire prologue to completion: cutscene + all interactions.
    /// Returns the sequence of step results.
    /// </summary>
    public List<PrologueStepResult> RunToCompletion()
    {
        var results = new List<PrologueStepResult>();

        if (State == PrologueState.NotStarted)
            Start();

        // Run cutscene to completion
        while (State == PrologueState.Cutscene)
            results.Add(Step(0.5f)); // large steps to finish quickly

        // Run all interactions
        while (State == PrologueState.Playing)
            results.Add(Step());

        return results;
    }
}

/// <summary>Prologue lifecycle states.</summary>
public enum PrologueState
{
    NotStarted,
    Cutscene,
    Playing,
    Completed,
}

/// <summary>Result of a single prologue step.</summary>
public sealed record PrologueStepResult
{
    public PrologueState State { get; init; }
    public string? InteractedObjectId { get; init; }
    public string? InteractionMessage { get; init; }
    public HubRhythmPhase Phase { get; init; }
    public Vec3 CameraPosition { get; init; }
    public Vec3 CameraLookAt { get; init; }
    public string? HintText { get; init; }
    public string? ObjectiveText { get; init; }
    public int XpGained { get; init; }
    public int InventoryCount { get; init; }
    public bool ProtocolZeroUnlocked { get; init; }
}
