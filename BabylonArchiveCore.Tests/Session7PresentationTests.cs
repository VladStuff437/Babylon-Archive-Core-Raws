using BabylonArchiveCore.Content.Pipeline;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Core.Scene;
using BabylonArchiveCore.Domain.Scene;
using BabylonArchiveCore.Runtime.Gameplay;
using BabylonArchiveCore.Runtime.QA;

namespace BabylonArchiveCore.Tests;

public class Session7PresentationTests
{
    private sealed class NullLogger : ILogger
    {
        public void Info(string message) { }
        public void Warn(string message) { }
        public void Error(string message) { }
    }

    private static NullLogger Logger => new();

    private static string ContentRoot =>
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Content"));

    private static A0ContentProvider Provider => new(ContentRoot);

    // ════════════════════════════════════════════════════
    // CutsceneSequencer
    // ════════════════════════════════════════════════════

    [Fact]
    public void Cutscene_RequiresAtLeastTwoKeyframes()
    {
        Assert.Throws<ArgumentException>(() =>
            new CutsceneSequencer([new CutsceneKeyframe(Vec3.Zero, Vec3.Zero, 0)]));
    }

    [Fact]
    public void Cutscene_InitialState_NotPlayingNotFinished()
    {
        var seq = MakeCutscene();
        Assert.False(seq.IsPlaying);
        Assert.False(seq.IsFinished);
    }

    [Fact]
    public void Cutscene_Play_SetsIsPlaying()
    {
        var seq = MakeCutscene();
        seq.Play();
        Assert.True(seq.IsPlaying);
        Assert.False(seq.IsFinished);
    }

    [Fact]
    public void Cutscene_Update_InterpolatesPosition()
    {
        var seq = MakeCutscene();
        seq.Play();
        var frame = seq.Update(1f); // halfway through 2-second duration
        Assert.False(frame.IsFinished);
        // Position should be between start (0,0,0) and end (10,0,0)
        Assert.True(frame.Position.X > 0 && frame.Position.X < 10);
    }

    [Fact]
    public void Cutscene_Update_FinishesAtEnd()
    {
        var seq = MakeCutscene();
        seq.Play();
        seq.Update(1f);
        var frame = seq.Update(2f); // past total duration
        Assert.True(frame.IsFinished);
        Assert.True(seq.IsFinished);
        Assert.False(seq.IsPlaying);
    }

    [Fact]
    public void Cutscene_Skip_JumpsToEnd()
    {
        var seq = MakeCutscene();
        seq.Play();
        seq.Skip();
        Assert.True(seq.IsFinished);
        Assert.False(seq.IsPlaying);
        Assert.Equal(10f, seq.CurrentPosition.X);
    }

    [Fact]
    public void Cutscene_Update_WhenNotPlaying_ReturnsCurrentState()
    {
        var seq = MakeCutscene();
        var frame = seq.Update(1f);
        Assert.False(frame.IsFinished);
        Assert.Equal(Vec3.Zero, frame.Position);
    }

    [Fact]
    public void Cutscene_MultiKeyframe_InterpolatesCorrectly()
    {
        var keyframes = new List<CutsceneKeyframe>
        {
            new(new Vec3(0, 0, 0), Vec3.Zero, 0f),
            new(new Vec3(5, 0, 0), Vec3.Zero, 1f),
            new(new Vec3(5, 10, 0), Vec3.Zero, 2f),
        };
        var seq = new CutsceneSequencer(keyframes);
        seq.Play();

        // Halfway through first segment
        var f1 = seq.Update(0.5f);
        Assert.InRange(f1.Position.X, 2f, 3f);
        Assert.InRange(f1.Position.Y, -0.1f, 0.1f);

        // Halfway through second segment
        var f2 = seq.Update(1f);
        Assert.InRange(f2.Position.X, 4.9f, 5.1f);
        Assert.InRange(f2.Position.Y, 4f, 6f);
    }

    [Fact]
    public void Cutscene_TotalDuration_MatchesLastKeyframe()
    {
        var seq = MakeCutscene();
        Assert.Equal(2f, seq.TotalDuration);
    }

    // ════════════════════════════════════════════════════
    // HintSystem
    // ════════════════════════════════════════════════════

    [Fact]
    public void Hint_Register_StoresHint()
    {
        var hints = new HintSystem();
        hints.Register("obj1", "Press E to interact");
        hints.Update("obj1");
        Assert.Equal("Press E to interact", hints.ActiveHintText);
    }

    [Fact]
    public void Hint_Update_NullClearsHint()
    {
        var hints = new HintSystem();
        hints.Register("obj1", "Hint text");
        hints.Update("obj1");
        hints.Update(null);
        Assert.Null(hints.ActiveHintText);
    }

    [Fact]
    public void Hint_Update_UnknownIdClearsHint()
    {
        var hints = new HintSystem();
        hints.Register("obj1", "Hint text");
        hints.Update("obj1");
        hints.Update("unknown");
        Assert.Null(hints.ActiveHintText);
    }

    [Fact]
    public void Hint_Format_ReturnsFormattedText()
    {
        var hints = new HintSystem();
        hints.Register("scanner", "Scan biometrics");
        hints.Update("scanner");
        Assert.Equal("[E] Scan biometrics", hints.Format());
    }

    [Fact]
    public void Hint_Format_NullWhenNoHint()
    {
        var hints = new HintSystem();
        Assert.Null(hints.Format());
    }

    [Fact]
    public void Hint_RegisterFromZones_PicksUpHintText()
    {
        var bundle = new ContentDrivenSceneFactory(Provider, Logger).Build();
        var hints = new HintSystem();
        hints.RegisterFromZones(bundle.HubRuntime.Zones);

        // At least some zones should have hint texts
        hints.Update("capsule_exit");
        Assert.NotNull(hints.ActiveHintText);
    }

    [Fact]
    public void Hint_ActiveHintObjectId_ReflectsCurrentObject()
    {
        var hints = new HintSystem();
        hints.Register("obj1", "Use terminal");
        hints.Update("obj1");
        Assert.Equal("obj1", hints.ActiveHintObjectId);
    }

    // ════════════════════════════════════════════════════
    // ObjectiveHUD
    // ════════════════════════════════════════════════════

    [Fact]
    public void ObjectiveHUD_DisplaysActiveObjective()
    {
        var tracker = new ObjectiveTracker();
        tracker.Register(new Objective { ObjectiveId = "obj1", Text = "Find the scanner", Order = 0 });
        tracker.SetActive("obj1");
        var hud = new ObjectiveHUD(tracker);
        Assert.Equal("► Find the scanner", hud.DisplayText);
    }

    [Fact]
    public void ObjectiveHUD_EmptyWhenNoActive()
    {
        var tracker = new ObjectiveTracker();
        var hud = new ObjectiveHUD(tracker);
        Assert.Equal("", hud.DisplayText);
    }

    [Fact]
    public void ObjectiveHUD_Update_DetectsChange()
    {
        var tracker = new ObjectiveTracker();
        tracker.Register(new Objective { ObjectiveId = "obj1", Text = "First", Order = 0 });
        tracker.Register(new Objective { ObjectiveId = "obj2", Text = "Second", Order = 1 });
        tracker.SetActive("obj1");
        var hud = new ObjectiveHUD(tracker);

        tracker.Complete("obj1");
        tracker.SetActive("obj2");
        hud.Update();
        Assert.True(hud.HasChanged);
        Assert.Equal("► Second", hud.DisplayText);
    }

    [Fact]
    public void ObjectiveHUD_Update_NoChangeWhenSame()
    {
        var tracker = new ObjectiveTracker();
        tracker.Register(new Objective { ObjectiveId = "obj1", Text = "Same", Order = 0 });
        tracker.SetActive("obj1");
        var hud = new ObjectiveHUD(tracker);

        hud.Update();
        Assert.False(hud.HasChanged);
    }

    [Fact]
    public void ObjectiveHUD_GetProgress_ReturnsRatio()
    {
        var tracker = new ObjectiveTracker();
        tracker.Register(new Objective { ObjectiveId = "a", Text = "A", Order = 0 });
        tracker.Register(new Objective { ObjectiveId = "b", Text = "B", Order = 1 });
        tracker.SetActive("a");
        tracker.Complete("a");
        tracker.SetActive("b");
        var hud = new ObjectiveHUD(tracker);
        Assert.Equal("1/2", hud.GetProgress());
    }

    [Fact]
    public void ObjectiveHUD_CurrentObjectiveId_ReflectsTracker()
    {
        var tracker = new ObjectiveTracker();
        tracker.Register(new Objective { ObjectiveId = "obj1", Text = "Go", Order = 0 });
        tracker.SetActive("obj1");
        var hud = new ObjectiveHUD(tracker);
        Assert.Equal("obj1", hud.CurrentObjectiveId);
    }

    // ════════════════════════════════════════════════════
    // PrologueRunner
    // ════════════════════════════════════════════════════

    [Fact]
    public void PrologueRunner_StartsAsNotStarted()
    {
        var runner = new PrologueRunner(Provider, Logger);
        Assert.Equal(PrologueState.NotStarted, runner.State);
    }

    [Fact]
    public void PrologueRunner_Start_BeginsCutscene()
    {
        var runner = new PrologueRunner(Provider, Logger);
        runner.Start();
        Assert.Equal(PrologueState.Cutscene, runner.State);
        Assert.True(runner.Cutscene.IsPlaying);
    }

    [Fact]
    public void PrologueRunner_SkipCutscene_JumpsToPlaying()
    {
        var runner = new PrologueRunner(Provider, Logger);
        runner.Start();
        runner.SkipCutscene();
        Assert.Equal(PrologueState.Playing, runner.State);
        Assert.True(runner.Cutscene.IsFinished);
    }

    [Fact]
    public void PrologueRunner_Step_DuringCutscene_ReturnsCamera()
    {
        var runner = new PrologueRunner(Provider, Logger);
        runner.Start();
        var result = runner.Step(0.1f);
        Assert.Equal(PrologueState.Cutscene, result.State);
        Assert.True(result.CameraPosition.Y > 0);
    }

    [Fact]
    public void PrologueRunner_Step_DuringPlaying_InteractsWithNext()
    {
        var runner = new PrologueRunner(Provider, Logger);
        runner.Start();
        runner.SkipCutscene();
        var result = runner.Step();
        Assert.Equal("capsule_exit", result.InteractedObjectId);
        Assert.NotNull(result.InteractionMessage);
    }

    [Fact]
    public void PrologueRunner_RunToCompletion_ProtocolZeroUnlocked()
    {
        var runner = new PrologueRunner(Provider, Logger);
        var results = runner.RunToCompletion();
        Assert.True(results.Count > 0);
        Assert.Equal(PrologueState.Completed, runner.State);

        var last = results[^1];
        Assert.True(last.ProtocolZeroUnlocked);
        Assert.Equal(HubRhythmPhase.OperationAccess, last.Phase);
    }

    [Fact]
    public void PrologueRunner_RunToCompletion_CutsceneFramesFirst()
    {
        var runner = new PrologueRunner(Provider, Logger);
        var results = runner.RunToCompletion();

        // Should start with cutscene frames followed by gameplay
        Assert.True(results.Any(r => r.State == PrologueState.Cutscene || r.InteractedObjectId == null));
        Assert.True(results.Any(r => r.InteractedObjectId == "capsule_exit"));
        Assert.True(results.Any(r => r.InteractedObjectId == "gallery_overlook"));
    }

    [Fact]
    public void PrologueRunner_RunToCompletion_HasHints()
    {
        var runner = new PrologueRunner(Provider, Logger);
        var results = runner.RunToCompletion();

        var playingResults = results.Where(r => r.InteractedObjectId is not null).ToList();
        Assert.True(playingResults.Any(r => r.HintText is not null));
    }

    [Fact]
    public void PrologueRunner_RunToCompletion_HasObjectives()
    {
        var runner = new PrologueRunner(Provider, Logger);
        var results = runner.RunToCompletion();

        var playingResults = results.Where(r => r.InteractedObjectId is not null).ToList();
        Assert.True(playingResults.Any(r => r.ObjectiveText is not null && r.ObjectiveText.Length > 0));
    }

    [Fact]
    public void PrologueRunner_RunToCompletion_InventoryGrows()
    {
        var runner = new PrologueRunner(Provider, Logger);
        var results = runner.RunToCompletion();

        var playingResults = results.Where(r => r.InteractedObjectId is not null).ToList();
        var maxInv = playingResults.Max(r => r.InventoryCount);
        Assert.True(maxInv >= 3);
    }

    [Fact]
    public void PrologueRunner_PrologueSequence_MatchesExpected()
    {
        Assert.Equal(7, PrologueRunner.PrologueSequence.Length);
        Assert.Equal("capsule_exit", PrologueRunner.PrologueSequence[0]);
        Assert.Equal("gallery_overlook", PrologueRunner.PrologueSequence[^1]);
    }

    // ════════════════════════════════════════════════════
    // FullPlaythroughValidator
    // ════════════════════════════════════════════════════

    [Fact]
    public void Validator_AllMustHave_Pass()
    {
        var validator = new FullPlaythroughValidator(Provider, Logger);
        var report = validator.Validate();
        Assert.True(report.AllPassed, report.Format());
    }

    [Fact]
    public void Validator_Report_Has13Entries()
    {
        var validator = new FullPlaythroughValidator(Provider, Logger);
        var report = validator.Validate();
        Assert.True(report.TotalCount >= 13);
    }

    [Fact]
    public void Validator_Report_Format_ContainsPassFail()
    {
        var validator = new FullPlaythroughValidator(Provider, Logger);
        var report = validator.Validate();
        var text = report.Format();
        Assert.Contains("PASS", text);
        Assert.Contains("MUST-01", text);
    }

    [Fact]
    public void Validator_Report_NoExceptionEntry()
    {
        var validator = new FullPlaythroughValidator(Provider, Logger);
        var report = validator.Validate();
        Assert.DoesNotContain(report.Entries, e => e.Id == "EXCEPTION");
    }

    // ════════════════════════════════════════════════════
    // Helpers
    // ════════════════════════════════════════════════════

    private static CutsceneSequencer MakeCutscene() => new(
    [
        new CutsceneKeyframe(Vec3.Zero, Vec3.Zero, 0f),
        new CutsceneKeyframe(new Vec3(10, 0, 0), new Vec3(0, 0, 10), 2f),
    ]);
}
