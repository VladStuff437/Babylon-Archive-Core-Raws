using BabylonArchiveCore.Domain.Events.Semantic;
using BabylonArchiveCore.Domain.Mission;
using BabylonArchiveCore.Domain.World;
using BabylonArchiveCore.Domain.World.Morality;
using BabylonArchiveCore.Domain.World.Runtime;
using BabylonArchiveCore.Runtime.Archive;
using BabylonArchiveCore.Runtime.Mission;
using BabylonArchiveCore.Runtime.World;

namespace BabylonArchiveCore.Tests;

public sealed class Session10SystemIntegrationTests
{
    [Fact]
    public void MoralService_AppliesRule_AndLogsEntry()
    {
        var world = new WorldState { WorldSeed = 77 };
        var service = new MoralService(Session10MoralRulebook.CreateDefault());
        var log = new List<MoralLogEntry>();

        var applied = service.TryApply(new SemanticEventRecord
        {
            EventId = "EVT_A0_RESEARCH_ANOMALY_SOLVED",
            Kind = EventSemanticKind.Puzzle,
            SourceObjectId = "research_terminal",
            PayloadText = "Anomaly solved",
        }, world, log);

        Assert.True(applied);
        Assert.True(world.InsightScore >= 3);
        Assert.True(world.ArchiveIntegrityAxis >= 2);
        Assert.Single(log);
        Assert.Equal("EVT_A0_RESEARCH_ANOMALY_SOLVED", log[0].EventId);
    }

    [Fact]
    public void MissionLaunchValidator_BlocksMissingFlags_ForPlayer()
    {
        var validator = new MissionLaunchValidator();
        var page = new ArchivePageDefinition
        {
            PageId = "P02_INCOMPATIBLE_INDEX",
            TomeId = "T01_ThresholdOperations",
            DisplayName = "Несовместимый индекс",
            MissionType = "ResearchPuzzle",
            IsUnlockedInPlayerMode = true,
            RequiredFlags = ["PAGE_P01_COMPLETE"],
        };

        var result = validator.Validate(page, WorldRuntimeProfile.PlayerDefault, []);

        Assert.False(result.CanLaunch);
        Assert.Equal("Blocked", result.LaunchMode);
    }

    [Fact]
    public void MissionLaunchValidator_AllowsAdminOverride()
    {
        var validator = new MissionLaunchValidator();
        var page = new ArchivePageDefinition
        {
            PageId = "P03_EMPTY_WITNESS",
            TomeId = "T02_InterferenceTraces",
            DisplayName = "Пустой свидетель",
            MissionType = "DialogueAnomaly",
            IsUnlockedInPlayerMode = false,
            RequiredFlags = ["SOME_FLAG"],
        };

        var result = validator.Validate(page, WorldRuntimeProfile.AdminDefault, []);

        Assert.True(result.CanLaunch);
        Assert.True(result.IsAdminOverride);
        Assert.Equal("AdminPreview", result.LaunchMode);
    }

    [Fact]
    public void MissionKernelFactory_CreatesKernelFromPage()
    {
        var generator = new TemplateMissionPageGenerator();
        var factory = new MissionKernelFactory(generator);

        var kernel = factory.CreateKernel(new ArchivePageDefinition
        {
            PageId = "P01_ECHO_STACK_09",
            TomeId = "T01_ThresholdOperations",
            DisplayName = "Эхо в стеке 9",
            MissionType = "Investigation",
            RewardFlags = ["PAGE_P01_COMPLETE"],
        }, seed: 42);

        Assert.Equal("P01_ECHO_STACK_09", kernel.PageId);
        Assert.Contains("SignalTrace", kernel.RuntimeNodes);
        Assert.Contains("PAGE_P01_COMPLETE", kernel.SuccessFlags);
    }

    [Fact]
    public void Session10MissionCatalog_ExposesAdminDebugSlot_WhenRequested()
    {
        var slots = Session10MissionCatalog.BuildInitialBoard(includeAdminDebugSlot: true);
        Assert.Contains(slots, s => s.SlotId == "slot_debug_generator");
    }

    [Fact]
    public void ArchiveRoomLoader_UsesProfileForAdminUnlocks()
    {
        var loader = new ArchiveRoomLoader();
        var state = loader.Load("RM_HA_02_StackRing", WorldRuntimeProfile.AdminDefault);

        Assert.True(state.IsLoaded);
        Assert.True(state.IsAlwaysUnlockedInAdminMode);
        Assert.Contains("RM_HA_07_AnomalyAlcove", state.ConnectedRoomIds);
    }
}
