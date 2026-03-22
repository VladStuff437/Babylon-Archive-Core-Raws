using BabylonArchiveCore.Core.Events;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Domain.Mission;
using BabylonArchiveCore.Domain.World;
using BabylonArchiveCore.Runtime.Mission;

namespace BabylonArchiveCore.Tests;

public class DayFourMissionTests
{
    // --- MissionGraphRuntime ---

    [Fact]
    public void Mission_StartsAndTraversesGraph()
    {
        var (runtime, _) = CreateMission();

        Assert.True(runtime.Start());
        Assert.Equal(MissionStatus.Active, runtime.Status);
        Assert.Equal("intro", runtime.CurrentNodeId);
    }

    [Fact]
    public void Mission_ChoiceNavigatesToNextNode()
    {
        var (runtime, _) = CreateMission();
        runtime.Start();

        Assert.True(runtime.Choose("accept"));
        Assert.Equal("investigate", runtime.CurrentNodeId);
        Assert.Equal(MissionStatus.Active, runtime.Status);
    }

    [Fact]
    public void Mission_InvalidChoiceReturnsFalse()
    {
        var (runtime, _) = CreateMission();
        runtime.Start();

        Assert.False(runtime.Choose("nonexistent"));
        Assert.Equal("intro", runtime.CurrentNodeId);
    }

    [Fact]
    public void Mission_TerminalSuccess()
    {
        var (runtime, _) = CreateMission();
        runtime.Start();
        runtime.Choose("accept");
        runtime.Choose("report");

        Assert.Equal(MissionStatus.Completed, runtime.Status);
        Assert.Equal("success", runtime.CurrentNodeId);
    }

    [Fact]
    public void Mission_TerminalFailure()
    {
        var (runtime, _) = CreateMission();
        runtime.Start();
        runtime.Choose("refuse");

        Assert.Equal(MissionStatus.Failed, runtime.Status);
        Assert.Equal("abandoned", runtime.CurrentNodeId);
    }

    [Fact]
    public void Mission_CannotChooseAfterCompletion()
    {
        var (runtime, _) = CreateMission();
        runtime.Start();
        runtime.Choose("accept");
        runtime.Choose("report");

        Assert.False(runtime.Choose("accept"));
    }

    [Fact]
    public void Mission_ForceFail()
    {
        var (runtime, _) = CreateMission();
        runtime.Start();
        runtime.Fail();

        Assert.Equal(MissionStatus.Failed, runtime.Status);
    }

    [Fact]
    public void Mission_PublishesStatusEvents()
    {
        var (runtime, bus) = CreateMission();
        var events = new List<MissionStatusChangedEvent>();
        bus.Subscribe<MissionStatusChangedEvent>(e => events.Add(e));

        runtime.Start();
        runtime.Choose("accept");
        runtime.Choose("report");

        Assert.Equal(2, events.Count); // NotStarted→Active, Active→Completed
        Assert.Equal(MissionStatus.Active, events[0].NewStatus);
        Assert.Equal(MissionStatus.Completed, events[1].NewStatus);
    }

    [Fact]
    public void Mission_SuccessEffectReturned()
    {
        var (runtime, _) = CreateMission();
        runtime.Start();
        runtime.Choose("accept");
        runtime.Choose("report");

        var effect = runtime.GetCompletionEffect();
        Assert.NotNull(effect);
        Assert.Equal("first_mission_done", effect.SetFlag);
        Assert.Equal(10, effect.MoralDelta);
    }

    [Fact]
    public void Mission_FailureEffectReturned()
    {
        var (runtime, _) = CreateMission();
        runtime.Start();
        runtime.Choose("refuse");

        var effect = runtime.GetCompletionEffect();
        Assert.NotNull(effect);
        Assert.Equal(-5, effect.MoralDelta);
    }

    // --- CompetitionRuntime ---

    [Fact]
    public void Competition_RaceMode_FirstToTargetWins()
    {
        var comp = CreateCompetition(CompetitionMode.Race, target: 5);
        var rt = new CompetitionRuntime(comp, new NullLogger());

        rt.AddScore(TeamSide.Alpha, 3);
        Assert.Null(rt.Winner);

        rt.AddScore(TeamSide.Beta, 5);
        Assert.Equal(TeamSide.Beta, rt.Winner);
    }

    [Fact]
    public void Competition_ControlMode_HighestScoreWins()
    {
        var comp = CreateCompetition(CompetitionMode.Control, target: 10);
        var rt = new CompetitionRuntime(comp, new NullLogger());

        rt.AddScore(TeamSide.Alpha, 7);
        rt.AddScore(TeamSide.Beta, 4);
        Assert.False(rt.IsFinished);

        rt.ResolveControl();
        Assert.True(rt.IsFinished);
        Assert.Equal(TeamSide.Alpha, rt.Winner);
    }

    [Fact]
    public void Competition_ScoreIgnoredAfterFinish()
    {
        var comp = CreateCompetition(CompetitionMode.Race, target: 3);
        var rt = new CompetitionRuntime(comp, new NullLogger());

        rt.AddScore(TeamSide.Alpha, 5);
        Assert.Equal(TeamSide.Alpha, rt.Winner);

        rt.AddScore(TeamSide.Beta, 100); // should be ignored
        Assert.Equal(TeamSide.Alpha, rt.Winner);
        Assert.Equal(0, comp.Beta.Score); // Beta never scored
    }

    [Fact]
    public void Competition_WinnerEffectReturned()
    {
        var comp = CreateCompetition(CompetitionMode.Race, target: 1);
        var rt = new CompetitionRuntime(comp, new NullLogger());

        rt.AddScore(TeamSide.Beta, 1);
        var effect = rt.GetWinnerEffect();
        Assert.NotNull(effect);
        Assert.Equal("beta_victory", effect.SetFlag);
    }

    // --- WorldStateEffectApplier ---

    [Fact]
    public void EffectApplier_SetsFlag()
    {
        var world = new WorldState { WorldSeed = 1 };
        var effect = new MissionEffect { SetFlag = "test_flag" };

        WorldStateEffectApplier.Apply(world, effect, new NullLogger());

        Assert.True(world.HasFlag("test_flag"));
    }

    [Fact]
    public void EffectApplier_AdjustsAxes()
    {
        var world = new WorldState { WorldSeed = 1 };
        var effect = new MissionEffect { MoralDelta = 15, TechnoArcaneDelta = -20 };

        WorldStateEffectApplier.Apply(world, effect, new NullLogger());

        Assert.Equal(15, world.MoralAxis);
        Assert.Equal(-20, world.TechnoArcaneAxis);
    }

    [Fact]
    public void EffectApplier_AdjustsRelations()
    {
        var world = new WorldState { WorldSeed = 1 };
        var effect = new MissionEffect
        {
            RelationDeltas = new() { ["CORE"] = 25, ["Archive Council"] = -10 },
        };

        WorldStateEffectApplier.Apply(world, effect, new NullLogger());

        Assert.Equal(25, world.EntityRelations["CORE"]);
        Assert.Equal(-10, world.EntityRelations["Archive Council"]);
    }

    [Fact]
    public void EffectApplier_FullMissionFlow_WorldStateUpdated()
    {
        var (runtime, _) = CreateMission();
        var world = new WorldState { WorldSeed = 42 };

        runtime.Start();
        runtime.Choose("accept");
        runtime.Choose("report");

        var effect = runtime.GetCompletionEffect()!;
        WorldStateEffectApplier.Apply(world, effect, new NullLogger());

        Assert.True(world.HasFlag("first_mission_done"));
        Assert.Equal(10, world.MoralAxis);
        Assert.Equal(20, world.EntityRelations["CORE"]);
    }

    // --- Helpers ---

    private static (MissionGraphRuntime, EventBus) CreateMission()
    {
        var def = new MissionDefinition
        {
            Id = "m_first",
            Title = "Первое задание",
            Type = MissionType.Main,
            StartNodeId = "intro",
            Nodes = new()
            {
                ["intro"] = new MissionNode
                {
                    Id = "intro",
                    Description = "Вводный брифинг от C.O.R.E.",
                    Transitions = new() { ["accept"] = "investigate", ["refuse"] = "abandoned" },
                },
                ["investigate"] = new MissionNode
                {
                    Id = "investigate",
                    Description = "Исследуйте аномалию в секторе 0.",
                    Transitions = new() { ["report"] = "success", ["ignore"] = "abandoned" },
                },
                ["success"] = new MissionNode
                {
                    Id = "success",
                    Description = "Аномалия задокументирована.",
                    IsTerminalSuccess = true,
                },
                ["abandoned"] = new MissionNode
                {
                    Id = "abandoned",
                    Description = "Задание отклонено оператором.",
                    IsTerminalFailure = true,
                },
            },
            OnSuccess = new MissionEffect
            {
                SetFlag = "first_mission_done",
                MoralDelta = 10,
                RelationDeltas = new() { ["CORE"] = 20 },
            },
            OnFailure = new MissionEffect
            {
                MoralDelta = -5,
                RelationDeltas = new() { ["CORE"] = -15 },
            },
        };

        var bus = new EventBus();
        var rt = new MissionGraphRuntime(def, new NullLogger(), bus);
        return (rt, bus);
    }

    private static CompetitionDefinition CreateCompetition(CompetitionMode mode, int target) =>
        new()
        {
            MissionId = "comp_test",
            Mode = mode,
            Alpha = new CompetitionTeam { Side = TeamSide.Alpha, Name = "Team A" },
            Beta = new CompetitionTeam { Side = TeamSide.Beta, Name = "Team B" },
            TargetScore = target,
            OnAlphaWins = new MissionEffect { SetFlag = "alpha_victory", MoralDelta = 5 },
            OnBetaWins = new MissionEffect { SetFlag = "beta_victory", TechnoArcaneDelta = 10 },
        };

    private sealed class NullLogger : ILogger
    {
        public void Info(string message) { }
        public void Warn(string message) { }
        public void Error(string message) { }
    }
}
