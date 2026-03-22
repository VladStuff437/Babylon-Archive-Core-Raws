using BabylonArchiveCore.Content.Pipeline;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Runtime.QA;

namespace BabylonArchiveCore.Tests;

public class Session8AcceptanceTests
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
    // V1CompletionReport
    // ════════════════════════════════════════════════════

    [Fact]
    public void V1Report_Generate_ReturnsReport()
    {
        var gen = new V1CompletionReport(Provider, Logger);
        var report = gen.Generate();
        Assert.NotNull(report);
        Assert.True(report.Results.Count > 0);
    }

    [Fact]
    public void V1Report_AllMustHave_Pass()
    {
        var gen = new V1CompletionReport(Provider, Logger);
        var report = gen.Generate();
        Assert.True(report.IsV1Ready, report.Format());
    }

    [Fact]
    public void V1Report_Has13MustCriteria()
    {
        var gen = new V1CompletionReport(Provider, Logger);
        var report = gen.Generate();
        Assert.Equal(13, report.MustTotal);
        Assert.Equal(13, report.MustPassed);
    }

    [Fact]
    public void V1Report_Has7ShouldCriteria()
    {
        var gen = new V1CompletionReport(Provider, Logger);
        var report = gen.Generate();
        Assert.Equal(7, report.ShouldTotal);
    }

    [Fact]
    public void V1Report_ShouldHave_MostPass()
    {
        var gen = new V1CompletionReport(Provider, Logger);
        var report = gen.Generate();
        // At least 5 of 7 SHOULD criteria should pass (19 is visual-skip)
        Assert.True(report.ShouldPassed >= 5);
    }

    [Fact]
    public void V1Report_Has5NiceCriteria()
    {
        var gen = new V1CompletionReport(Provider, Logger);
        var report = gen.Generate();
        Assert.Equal(5, report.NiceTotal);
    }

    [Fact]
    public void V1Report_NiceHave_DebugToolsPass()
    {
        var gen = new V1CompletionReport(Provider, Logger);
        var report = gen.Generate();
        Assert.True(report.NicePassed >= 2); // overlay + teleport
    }

    [Fact]
    public void V1Report_SkippedCriteria_AreVisualOnly()
    {
        var gen = new V1CompletionReport(Provider, Logger);
        var report = gen.Generate();
        Assert.True(report.Skipped >= 3); // lighting, sounds, steam, holograms
        var skippedIds = report.Results
            .Where(r => r.Status == CriterionStatus.Skipped)
            .Select(r => r.Id)
            .ToList();
        Assert.All(skippedIds, id => Assert.True(
            id.Contains("19") || id.Contains("23") || id.Contains("24") || id.Contains("25")));
    }

    [Fact]
    public void V1Report_Summary_ContainsCounts()
    {
        var gen = new V1CompletionReport(Provider, Logger);
        var report = gen.Generate();
        var summary = report.Summary();
        Assert.Contains("MUST:", summary);
        Assert.Contains("SHOULD:", summary);
        Assert.Contains("NICE:", summary);
        Assert.Contains("V1 Ready:", summary);
    }

    [Fact]
    public void V1Report_Format_ContainsAllTiers()
    {
        var gen = new V1CompletionReport(Provider, Logger);
        var report = gen.Generate();
        var text = report.Format();
        Assert.Contains("MUST HAVE", text);
        Assert.Contains("SHOULD HAVE", text);
        Assert.Contains("NICE HAVE", text);
        Assert.Contains("ИТОГО", text);
    }

    [Fact]
    public void V1Report_NoException()
    {
        var gen = new V1CompletionReport(Provider, Logger);
        var report = gen.Generate();
        Assert.DoesNotContain(report.Results, r => r.Id == "EXCEPTION");
    }

    // ════════════════════════════════════════════════════
    // EndToEndSmokeTest
    // ════════════════════════════════════════════════════

    [Fact]
    public void SmokeTest_Run_Passes()
    {
        var smoke = new EndToEndSmokeTest(Provider, Logger);
        var result = smoke.Run();
        Assert.True(result.Passed);
    }

    [Fact]
    public void SmokeTest_Run_HasCutsceneFrames()
    {
        var smoke = new EndToEndSmokeTest(Provider, Logger);
        var result = smoke.Run();
        Assert.True(result.CutsceneFrames >= 2);
    }

    [Fact]
    public void SmokeTest_Run_Has7InteractionSteps()
    {
        var smoke = new EndToEndSmokeTest(Provider, Logger);
        var result = smoke.Run();
        Assert.Equal(7, result.InteractionSteps);
    }

    [Fact]
    public void SmokeTest_Run_HasInventory()
    {
        var smoke = new EndToEndSmokeTest(Provider, Logger);
        var result = smoke.Run();
        Assert.True(result.InventoryCount >= 3);
    }

    [Fact]
    public void SmokeTest_Run_HasJournalEntries()
    {
        var smoke = new EndToEndSmokeTest(Provider, Logger);
        var result = smoke.Run();
        Assert.True(result.JournalEntries > 0);
    }

    [Fact]
    public void SmokeTest_Run_CompletesQuickly()
    {
        var smoke = new EndToEndSmokeTest(Provider, Logger);
        var result = smoke.Run();
        Assert.True(result.ElapsedMs < 10_000); // under 10 seconds
    }

    [Fact]
    public void SmokeTest_Run_AllChecksRecorded()
    {
        var smoke = new EndToEndSmokeTest(Provider, Logger);
        var result = smoke.Run();
        Assert.True(result.Checks.Count >= 8);
    }

    // ════════════════════════════════════════════════════
    // Cross-session regression
    // ════════════════════════════════════════════════════

    [Fact]
    public void Regression_ContentLoads_AllZones()
    {
        var provider = Provider;
        var zones = provider.LoadZones();
        Assert.True(zones.Count >= 9);
    }

    [Fact]
    public void Regression_ContentLoads_AllDialogues()
    {
        var provider = Provider;
        var dialogues = provider.LoadDialogues();
        Assert.True(dialogues.Count >= 6);
    }

    [Fact]
    public void Regression_ContentLoads_AllObjectives()
    {
        var provider = Provider;
        var objectives = provider.LoadObjectives();
        Assert.True(objectives.Count >= 7);
    }

    [Fact]
    public void Regression_ContentLoads_AllTriggers()
    {
        var provider = Provider;
        var triggers = provider.LoadTriggers();
        Assert.True(triggers.Count >= 5);
    }
}
