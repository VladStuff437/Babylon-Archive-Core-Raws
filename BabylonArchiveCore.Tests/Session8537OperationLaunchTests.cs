using BabylonArchiveCore.Domain.Mission;
using BabylonArchiveCore.Runtime.Mission;

namespace BabylonArchiveCore.Tests;

public sealed class Session8537OperationLaunchTests
{
    [Fact]
    public void Evaluate_Denies_WhenProtocolZeroRequiredButLocked()
    {
        var mission = CreateMission(requiresProtocolZero: true);

        var result = OperationLaunchGate.Evaluate(mission, protocolZeroUnlocked: false, MissionStatus.NotStarted);

        Assert.False(result.Allowed);
        Assert.Equal("Protocol Zero is required.", result.Reason);
    }

    [Fact]
    public void Evaluate_Allows_WhenProtocolZeroRequiredAndUnlocked()
    {
        var mission = CreateMission(requiresProtocolZero: true);

        var result = OperationLaunchGate.Evaluate(mission, protocolZeroUnlocked: true, MissionStatus.NotStarted);

        Assert.True(result.Allowed);
        Assert.Equal("OK", result.Reason);
    }

    [Fact]
    public void Evaluate_Denies_WhenAlreadyActive()
    {
        var mission = CreateMission();

        var result = OperationLaunchGate.Evaluate(mission, protocolZeroUnlocked: true, MissionStatus.Active);

        Assert.False(result.Allowed);
        Assert.Equal("Mission is already active.", result.Reason);
    }

    [Fact]
    public void Evaluate_Denies_WhenCompletedAndNotReplayable()
    {
        var mission = CreateMission(isReplayable: false);

        var result = OperationLaunchGate.Evaluate(mission, protocolZeroUnlocked: true, MissionStatus.Completed);

        Assert.False(result.Allowed);
        Assert.Equal("Mission cannot be replayed.", result.Reason);
    }

    [Fact]
    public void Evaluate_Allows_WhenCompletedAndReplayable()
    {
        var mission = CreateMission(isReplayable: true);

        var result = OperationLaunchGate.Evaluate(mission, protocolZeroUnlocked: true, MissionStatus.Completed);

        Assert.True(result.Allowed);
        Assert.Equal("OK", result.Reason);
    }

    [Fact]
    public void BuildCatalog_GroupsByTerminalTab()
    {
        var missions = new[]
        {
            CreateMission("M_CORE", "Core One", MissionTerminalTab.Core),
            CreateMission("M_BRANCH", "Branch One", MissionTerminalTab.Branches),
            CreateMission("M_SERVICE", "Service One", MissionTerminalTab.Service),
        };

        var catalog = OperationLaunchGate.BuildCatalog(missions);

        Assert.Single(catalog[MissionTerminalTab.Core]);
        Assert.Single(catalog[MissionTerminalTab.Branches]);
        Assert.Single(catalog[MissionTerminalTab.Service]);
    }

    [Fact]
    public void BuildCatalog_ContainsAllTabsEvenIfEmpty()
    {
        var catalog = OperationLaunchGate.BuildCatalog([CreateMission()]);

        foreach (var tab in Enum.GetValues<MissionTerminalTab>())
        {
            Assert.True(catalog.ContainsKey(tab));
        }
    }

    [Fact]
    public void BuildCatalog_SortsTitlesInsideTab()
    {
        var missions = new[]
        {
            CreateMission("M2", "Zulu", MissionTerminalTab.Core),
            CreateMission("M1", "Alpha", MissionTerminalTab.Core),
        };

        var catalog = OperationLaunchGate.BuildCatalog(missions);

        Assert.Equal("Alpha", catalog[MissionTerminalTab.Core][0].Title);
        Assert.Equal("Zulu", catalog[MissionTerminalTab.Core][1].Title);
    }

    private static MissionDefinition CreateMission(
        string id = "M_TEST",
        string title = "Test Mission",
        MissionTerminalTab tab = MissionTerminalTab.Core,
        bool requiresProtocolZero = false,
        bool isReplayable = true)
    {
        return new MissionDefinition
        {
            Id = id,
            Title = title,
            Type = MissionType.Main,
            TerminalTab = tab,
            RequiresProtocolZero = requiresProtocolZero,
            IsReplayable = isReplayable,
            StartNodeId = "start",
            Nodes = new()
            {
                ["start"] = new MissionNode
                {
                    Id = "start",
                    Description = "Start",
                    IsTerminalSuccess = false,
                    IsTerminalFailure = false,
                },
            },
        };
    }
}
