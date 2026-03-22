using BabylonArchiveCore.Core.Events;
using BabylonArchiveCore.Domain.Dialogue;
using BabylonArchiveCore.Domain.Mission;
using BabylonArchiveCore.Domain.Narrative;
using BabylonArchiveCore.Domain.World;
using BabylonArchiveCore.Runtime.Narrative;
using Xunit;

namespace BabylonArchiveCore.Tests;

public class DayFiveNarrativeTests
{
    private static Core.Logging.ILogger MakeLogger() => new Infrastructure.Logging.FileLogger(
        Path.Combine(Path.GetTempPath(), $"bac_test_{Guid.NewGuid():N}.log"));

    // ───── Dialogue Runtime ─────

    private static DialogueDefinition BuildTestDialogue() => new()
    {
        Id = "dlg_test",
        SpeakerEntity = "CORE",
        StartLineId = "greeting",
        Lines = new()
        {
            ["greeting"] = new DialogueLine
            {
                Id = "greeting",
                Speaker = "C.O.R.E.",
                Text = "Обнаружена аномалия. Как поступите?",
                Options = new()
                {
                    new DialogueOption
                    {
                        Id = "investigate",
                        Text = "Исследовать аномалию",
                        TargetLineId = "investigation",
                    },
                    new DialogueOption
                    {
                        Id = "lie",
                        Text = "Притвориться, что ничего нет",
                        TargetLineId = "deception_result",
                        CheckType = DialogueCheckType.Deception,
                        Difficulty = 30,
                    },
                    new DialogueOption
                    {
                        Id = "secret",
                        Text = "Показать секретные данные",
                        TargetLineId = "secret_path",
                        RequiredFlag = "has_secret_data",
                    },
                },
            },
            ["investigation"] = new DialogueLine
            {
                Id = "investigation",
                Speaker = "C.O.R.E.",
                Text = "Данные расходятся с терминалом. Что вы думаете?",
                Options = new()
                {
                    new DialogueOption
                    {
                        Id = "present_evidence",
                        Text = "Предъявить доказательства",
                        TargetLineId = "evidence_accepted",
                        CheckType = DialogueCheckType.Evidence,
                        Difficulty = 20,
                        Effect = new MissionEffect { SetFlag = "evidence_shown", RelationDeltas = new() { ["CORE"] = 10 } },
                    },
                },
            },
            ["evidence_accepted"] = new DialogueLine
            {
                Id = "evidence_accepted",
                Speaker = "C.O.R.E.",
                Text = "Интересно. Это подтверждает мои подозрения.",
            },
            ["deception_result"] = new DialogueLine
            {
                Id = "deception_result",
                Speaker = "C.O.R.E.",
                Text = "Хорошо, продолжайте работу.",
            },
            ["secret_path"] = new DialogueLine
            {
                Id = "secret_path",
                Speaker = "C.O.R.E.",
                Text = "Эти данные меняют всё.",
            },
        },
    };

    [Fact]
    public void Dialogue_Start_RecordsFirstLine()
    {
        var ws = new WorldState { WorldSeed = 1 };
        var rt = new DialogueRuntime(BuildTestDialogue(), ws, new(), MakeLogger(), new EventBus());
        Assert.True(rt.Start());
        Assert.Single(rt.Transcript);
        Assert.Contains("C.O.R.E.", rt.Transcript[0]);
        Assert.False(rt.IsFinished);
    }

    [Fact]
    public void Dialogue_ChooseSimpleOption_TransitionsToNextLine()
    {
        var ws = new WorldState { WorldSeed = 1 };
        var rt = new DialogueRuntime(BuildTestDialogue(), ws, new(), MakeLogger(), new EventBus());
        rt.Start();
        Assert.True(rt.Choose("investigate"));
        Assert.Equal("investigation", rt.CurrentLineId);
        Assert.Equal(2, rt.Transcript.Count);
    }

    [Fact]
    public void Dialogue_TerminalLine_SetsFinished()
    {
        var ws = new WorldState { WorldSeed = 1 };
        var stats = new Dictionary<string, int> { ["Evidence"] = 50 };
        var rt = new DialogueRuntime(BuildTestDialogue(), ws, stats, MakeLogger(), new EventBus());
        rt.Start();
        rt.Choose("investigate");
        rt.Choose("present_evidence");
        Assert.True(rt.IsFinished);
        Assert.Equal("evidence_accepted", rt.CurrentLineId);
        Assert.Equal(3, rt.Transcript.Count);
    }

    [Fact]
    public void Dialogue_SkillCheckPass_AppliesEffectAndTransitions()
    {
        var ws = new WorldState { WorldSeed = 1 };
        var stats = new Dictionary<string, int> { ["Evidence"] = 50 };
        var rt = new DialogueRuntime(BuildTestDialogue(), ws, stats, MakeLogger(), new EventBus());
        rt.Start();
        rt.Choose("investigate");
        rt.Choose("present_evidence");
        Assert.True(rt.LastCheckPassed);
        Assert.True(ws.HasFlag("evidence_shown"));
        Assert.Equal(10, ws.EntityRelations["CORE"]);
    }

    [Fact]
    public void Dialogue_SkillCheckFail_DoesNotTransition()
    {
        var ws = new WorldState { WorldSeed = 1 };
        var stats = new Dictionary<string, int> { ["Deception"] = 10 }; // below 30 threshold
        var rt = new DialogueRuntime(BuildTestDialogue(), ws, stats, MakeLogger(), new EventBus());
        rt.Start();
        var result = rt.Choose("lie");
        Assert.True(result); // choice accepted
        Assert.False(rt.LastCheckPassed); // check failed
        Assert.Equal("greeting", rt.CurrentLineId); // stayed on same line
    }

    [Fact]
    public void Dialogue_SkillCheckPass_Deception_Transitions()
    {
        var ws = new WorldState { WorldSeed = 1 };
        var stats = new Dictionary<string, int> { ["Deception"] = 30 }; // exactly meets threshold
        var rt = new DialogueRuntime(BuildTestDialogue(), ws, stats, MakeLogger(), new EventBus());
        rt.Start();
        rt.Choose("lie");
        Assert.True(rt.LastCheckPassed);
        Assert.Equal("deception_result", rt.CurrentLineId);
        Assert.True(rt.IsFinished);
    }

    [Fact]
    public void Dialogue_FlagGatedOption_HiddenWithoutFlag()
    {
        var ws = new WorldState { WorldSeed = 1 };
        var rt = new DialogueRuntime(BuildTestDialogue(), ws, new(), MakeLogger(), new EventBus());
        rt.Start();
        var opts = rt.GetAvailableOptions();
        Assert.DoesNotContain(opts, o => o.Id == "secret");
    }

    [Fact]
    public void Dialogue_FlagGatedOption_VisibleWithFlag()
    {
        var ws = new WorldState { WorldSeed = 1 };
        ws.SetFlag("has_secret_data");
        var rt = new DialogueRuntime(BuildTestDialogue(), ws, new(), MakeLogger(), new EventBus());
        rt.Start();
        var opts = rt.GetAvailableOptions();
        Assert.Contains(opts, o => o.Id == "secret");
    }

    [Fact]
    public void Dialogue_PublishesCheckEvent()
    {
        var ws = new WorldState { WorldSeed = 1 };
        var bus = new EventBus();
        DialogueCheckEvent? captured = null;
        bus.Subscribe<DialogueCheckEvent>(e => captured = e);

        var stats = new Dictionary<string, int> { ["Deception"] = 50 };
        var rt = new DialogueRuntime(BuildTestDialogue(), ws, stats, MakeLogger(), bus);
        rt.Start();
        rt.Choose("lie");

        Assert.NotNull(captured);
        Assert.Equal("Deception", captured!.CheckType);
        Assert.True(captured.Passed);
    }

    // ───── Intervention Tracker ─────

    [Fact]
    public void Tracker_NoEvidence_Unaware()
    {
        var tracker = new InterventionTracker(MakeLogger(), new EventBus());
        Assert.Equal(InterventionCertainty.Unaware, tracker.Certainty);
        Assert.Empty(tracker.Evidence);
    }

    [Fact]
    public void Tracker_AddEvidence_RaisesCertainty()
    {
        var tracker = new InterventionTracker(MakeLogger(), new EventBus());
        tracker.AddEvidence(new InterventionEvidence
        {
            Id = "ev1", Address = "S00.H00.M00.SH00.C00.T000.P001",
            TerminalData = "Normal", ArchiveData = "Altered", Severity = 3,
        });
        Assert.Equal(InterventionCertainty.Suspicious, tracker.Certainty);
    }

    [Fact]
    public void Tracker_AccumulatedEvidence_ReachesProven()
    {
        var tracker = new InterventionTracker(MakeLogger(), new EventBus());
        for (int i = 0; i < 6; i++)
        {
            tracker.AddEvidence(new InterventionEvidence
            {
                Id = $"ev{i}", Address = $"S00.H00.M00.SH00.C00.T000.P{i:D3}",
                TerminalData = "Normal", ArchiveData = $"Altered-{i}", Severity = 5,
            });
        }
        Assert.Equal(InterventionCertainty.Proven, tracker.Certainty);
        Assert.Equal(30, tracker.TotalSeverityScore);
    }

    [Fact]
    public void Tracker_SignatureConfirmation()
    {
        var tracker = new InterventionTracker(MakeLogger(), new EventBus());
        var sig = new InterventionSignature
        {
            PatternId = "pattern_alpha",
            Description = "Повторяющееся смещение данных",
            ConfirmationThreshold = 3,
        };
        tracker.RegisterSignature(sig);

        for (int i = 0; i < 3; i++)
        {
            tracker.AddEvidence(new InterventionEvidence
            {
                Id = $"sig_ev{i}", Address = $"S00.H00.M0{i}.SH00.C00.T000.P000",
                TerminalData = "X", ArchiveData = "Y", Severity = 1,
            }, "pattern_alpha");
        }

        Assert.True(sig.IsConfirmed);
        Assert.Single(tracker.GetConfirmedSignatures());
    }

    [Fact]
    public void Tracker_PublishesCertaintyChangeEvent()
    {
        var bus = new EventBus();
        InterventionCertaintyChangedEvent? captured = null;
        bus.Subscribe<InterventionCertaintyChangedEvent>(e => captured = e);

        var tracker = new InterventionTracker(MakeLogger(), bus);
        tracker.AddEvidence(new InterventionEvidence
        {
            Id = "ev_trigger", Address = "S01.H00.M00.SH00.C00.T000.P000",
            TerminalData = "A", ArchiveData = "B", Severity = 5,
        });

        Assert.NotNull(captured);
        Assert.Equal("Unaware", captured!.OldLevel);
        Assert.Equal("Suspicious", captured.NewLevel);
    }

    // ───── Archive Unlock Resolver ─────

    [Fact]
    public void Resolver_FlagGate_Unlocks()
    {
        var ws = new WorldState { WorldSeed = 1 };
        ws.SetFlag("mission_done");

        var gate = new ArchiveUnlockGate
        {
            Id = "gate1",
            TargetAddress = "S01.H00.M00.SH00.C00.T000.P000",
            Requirements = new() { new UnlockRequirement { Type = UnlockRequirementType.Flag, Key = "mission_done" } },
        };

        var resolver = new ArchiveUnlockResolver(MakeLogger());
        var unlocked = resolver.ResolveUnlocked(new[] { gate }, ws, InterventionCertainty.Unaware);
        Assert.Single(unlocked);
    }

    [Fact]
    public void Resolver_UnsatisfiedGate_Locked()
    {
        var ws = new WorldState { WorldSeed = 1 };
        var gate = new ArchiveUnlockGate
        {
            Id = "gate_locked",
            TargetAddress = "S02.H00.M00.SH00.C00.T000.P000",
            Requirements = new() { new UnlockRequirement { Type = UnlockRequirementType.Flag, Key = "nonexistent" } },
        };

        var resolver = new ArchiveUnlockResolver(MakeLogger());
        var unlocked = resolver.ResolveUnlocked(new[] { gate }, ws, InterventionCertainty.Unaware);
        Assert.Empty(unlocked);
    }

    [Fact]
    public void Resolver_RelationThreshold_Unlocks()
    {
        var ws = new WorldState { WorldSeed = 1 };
        ws.AdjustRelation("CORE", 30);

        var gate = new ArchiveUnlockGate
        {
            Id = "gate_rel",
            TargetAddress = "S01.H01.M00.SH00.C00.T000.P000",
            Requirements = new() { new UnlockRequirement { Type = UnlockRequirementType.RelationThreshold, Key = "CORE", Threshold = 20 } },
        };

        var resolver = new ArchiveUnlockResolver(MakeLogger());
        Assert.True(resolver.IsGateSatisfied(gate, ws, InterventionCertainty.Unaware));
    }

    [Fact]
    public void Resolver_CertaintyGate_Unlocks()
    {
        var ws = new WorldState { WorldSeed = 1 };
        var gate = new ArchiveUnlockGate
        {
            Id = "gate_cert",
            TargetAddress = "S01.H02.M00.SH00.C00.T000.P000",
            Requirements = new() { new UnlockRequirement { Type = UnlockRequirementType.CertaintyLevel, Key = "certainty", Threshold = 2 } },
        };

        var resolver = new ArchiveUnlockResolver(MakeLogger());
        Assert.False(resolver.IsGateSatisfied(gate, ws, InterventionCertainty.Suspicious));
        Assert.True(resolver.IsGateSatisfied(gate, ws, InterventionCertainty.Investigating));
    }

    [Fact]
    public void Resolver_MultipleRequirements_AND_Logic()
    {
        var ws = new WorldState { WorldSeed = 1 };
        ws.SetFlag("chapter1_done");
        ws.AdjustRelation("CORE", 15);

        var gate = new ArchiveUnlockGate
        {
            Id = "gate_multi",
            TargetAddress = "S02.H00.M01.SH00.C00.T000.P000",
            Requirements = new()
            {
                new UnlockRequirement { Type = UnlockRequirementType.Flag, Key = "chapter1_done" },
                new UnlockRequirement { Type = UnlockRequirementType.RelationThreshold, Key = "CORE", Threshold = 20 },
            },
        };

        var resolver = new ArchiveUnlockResolver(MakeLogger());
        // CORE = 15, needed 20 → not satisfied
        Assert.False(resolver.IsGateSatisfied(gate, ws, InterventionCertainty.Unaware));

        ws.AdjustRelation("CORE", 10); // now 25
        Assert.True(resolver.IsGateSatisfied(gate, ws, InterventionCertainty.Unaware));
    }

    // ───── Narrative Spine Runner ─────

    [Fact]
    public void Spine_TriggersChapterWhenCertaintyMet()
    {
        var ws = new WorldState { WorldSeed = 1 };
        var chapters = new List<NarrativeChapter>
        {
            new()
            {
                Id = "ch1", Title = "Первые подозрения", Order = 1,
                RequiredCertainty = InterventionCertainty.Suspicious,
                CompletionFlag = "chapter1_triggered",
                UnlocksAddresses = new() { "S01.H00.M00.SH00.C00.T000.P000" },
            },
        };

        var runner = new NarrativeSpineRunner(chapters, ws, MakeLogger());
        var triggered = runner.Evaluate(InterventionCertainty.Suspicious);

        Assert.Single(triggered);
        Assert.True(ws.HasFlag("chapter1_triggered"));
        Assert.Contains("S01.H00.M00.SH00.C00.T000.P000", ws.VisitedAddresses);
    }

    [Fact]
    public void Spine_SkipsChapterIfCertaintyTooLow()
    {
        var ws = new WorldState { WorldSeed = 1 };
        var chapters = new List<NarrativeChapter>
        {
            new()
            {
                Id = "ch2", Title = "Расследование", Order = 2,
                RequiredCertainty = InterventionCertainty.Investigating,
                CompletionFlag = "chapter2_triggered",
            },
        };

        var runner = new NarrativeSpineRunner(chapters, ws, MakeLogger());
        var triggered = runner.Evaluate(InterventionCertainty.Suspicious);

        Assert.Empty(triggered);
        Assert.False(ws.HasFlag("chapter2_triggered"));
    }

    [Fact]
    public void Spine_RequiredFlags_MustBeMet()
    {
        var ws = new WorldState { WorldSeed = 1 };
        var chapters = new List<NarrativeChapter>
        {
            new()
            {
                Id = "ch_gated", Title = "Глава за флагом", Order = 1,
                RequiredCertainty = InterventionCertainty.Unaware,
                RequiredFlags = new() { "prerequisite_flag" },
                CompletionFlag = "gated_triggered",
            },
        };

        var runner = new NarrativeSpineRunner(chapters, ws, MakeLogger());
        Assert.Empty(runner.Evaluate(InterventionCertainty.Proven)); // flag missing

        ws.SetFlag("prerequisite_flag");
        Assert.Single(runner.Evaluate(InterventionCertainty.Proven)); // now passes
    }

    [Fact]
    public void Spine_DoesNotReTriggerChapter()
    {
        var ws = new WorldState { WorldSeed = 1 };
        var chapters = new List<NarrativeChapter>
        {
            new()
            {
                Id = "ch_once", Title = "Одноразовая", Order = 1,
                RequiredCertainty = InterventionCertainty.Unaware,
                CompletionFlag = "ch_once_done",
            },
        };

        var runner = new NarrativeSpineRunner(chapters, ws, MakeLogger());
        Assert.Single(runner.Evaluate(InterventionCertainty.Proven));
        Assert.Empty(runner.Evaluate(InterventionCertainty.Proven)); // already triggered
    }

    // ───── Integration: Dialogue → Intervention → Spine → Unlock ─────

    [Fact]
    public void Integration_FullNarrativePipeline()
    {
        var ws = new WorldState { WorldSeed = 42 };
        var bus = new EventBus();
        var logger = MakeLogger();

        // 1. Dialogue reveals evidence via effect
        var dlg = new DialogueDefinition
        {
            Id = "dlg_reveal", SpeakerEntity = "CORE", StartLineId = "start",
            Lines = new()
            {
                ["start"] = new DialogueLine
                {
                    Id = "start", Speaker = "C.O.R.E.", Text = "Расхождение обнаружено.",
                    Options = new()
                    {
                        new DialogueOption
                        {
                            Id = "analyze", Text = "Анализировать", TargetLineId = "result",
                            Effect = new MissionEffect { SetFlag = "discrepancy_found", MoralDelta = 5 },
                        },
                    },
                },
                ["result"] = new DialogueLine
                {
                    Id = "result", Speaker = "C.O.R.E.", Text = "Данные зафиксированы.",
                },
            },
        };

        var dlgRt = new DialogueRuntime(dlg, ws, new(), logger, bus);
        dlgRt.Start();
        dlgRt.Choose("analyze");

        Assert.True(ws.HasFlag("discrepancy_found"));
        Assert.Equal(5, ws.MoralAxis);

        // 2. Intervention tracker accumulates evidence
        var tracker = new InterventionTracker(logger, bus);
        tracker.AddEvidence(new InterventionEvidence
        {
            Id = "ev_dlg", Address = "S00.H00.M00.SH00.C00.T001.P001",
            TerminalData = "Стандартный", ArchiveData = "Изменённый", Severity = 5,
        });
        tracker.AddEvidence(new InterventionEvidence
        {
            Id = "ev_field", Address = "S00.H00.M00.SH00.C00.T001.P002",
            TerminalData = "Нормальный", ArchiveData = "Подменённый", Severity = 4,
        });

        Assert.Equal(InterventionCertainty.Investigating, tracker.Certainty);

        // 3. Narrative spine triggers chapter
        var chapters = new List<NarrativeChapter>
        {
            new()
            {
                Id = "ch_discovery", Title = "Обнаружение вмешательства", Order = 1,
                RequiredCertainty = InterventionCertainty.Investigating,
                RequiredFlags = new() { "discrepancy_found" },
                CompletionFlag = "ch_discovery_done",
                UnlocksAddresses = new() { "S01.H00.M00.SH00.C00.T000.P000" },
            },
        };

        var spine = new NarrativeSpineRunner(chapters, ws, logger);
        var triggered = spine.Evaluate(tracker.Certainty);
        Assert.Single(triggered);
        Assert.True(ws.HasFlag("ch_discovery_done"));

        // 4. Archive unlock resolver opens gated zones
        var gate = new ArchiveUnlockGate
        {
            Id = "gate_sector1",
            TargetAddress = "S01.H01.M00.SH00.C00.T000.P000",
            Requirements = new()
            {
                new UnlockRequirement { Type = UnlockRequirementType.Flag, Key = "ch_discovery_done" },
                new UnlockRequirement { Type = UnlockRequirementType.CertaintyLevel, Key = "certainty", Threshold = (int)InterventionCertainty.Investigating },
            },
        };

        var resolver = new ArchiveUnlockResolver(logger);
        var unlocked = resolver.ResolveUnlocked(new[] { gate }, ws, tracker.Certainty);
        Assert.Single(unlocked);
        Assert.Equal("S01.H01.M00.SH00.C00.T000.P000", unlocked[0].TargetAddress);
    }
}
