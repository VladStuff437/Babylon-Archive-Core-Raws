using BabylonArchiveCore.Core.Input;
using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Core.State;
using BabylonArchiveCore.Domain;
using BabylonArchiveCore.Infrastructure.Save;

namespace BabylonArchiveCore.Tests;

public class DayOneFoundationTests
{
    [Fact]
    public void InputMap_DefaultContainsTouchAndKeyboardBindings()
    {
        var map = InputMap.CreateDefault();

        var interactBindings = map.GetBindings(InputAction.Interact);

        Assert.Contains(interactBindings,
            binding => binding == new InputBinding(InputDeviceType.Keyboard, "E"));
        Assert.Contains(interactBindings,
            binding => binding == new InputBinding(InputDeviceType.Touch, "Tap"));
    }

    [Fact]
    public void Router_CanSwitchBetweenRegisteredStates()
    {
        var router = new GameStateRouter();
        var logger = new NullLogger();
        router.Register(new TestState(SceneId.Boot, logger));
        router.Register(new TestState(SceneId.HubA0, logger));

        var switchedToBoot = router.TrySwitch(SceneId.Boot);
        var switchedToHub = router.TrySwitch(SceneId.HubA0);

        Assert.True(switchedToBoot);
        Assert.True(switchedToHub);
        Assert.Equal(SceneId.HubA0, router.CurrentState?.Id);
    }

    [Fact]
    public void SaveStore_LoadOrCreateBuildsNewSaveForMissingFile()
    {
        var path = Path.Combine(Path.GetTempPath(), $"bac-save-{Guid.NewGuid():N}.json");
        var store = new SaveGameStore();

        try
        {
            var save = store.LoadOrCreate(path, expectedVersion: 3);
            Assert.Equal(3, save.Version);
            Assert.True(File.Exists(path));
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    private sealed class NullLogger : ILogger
    {
        public void Info(string message)
        {
        }

        public void Warn(string message)
        {
        }

        public void Error(string message)
        {
        }
    }

    private sealed class TestState(SceneId id, ILogger logger) : IGameState
    {
        public SceneId Id => id;

        public void Enter() => logger.Info($"Enter {id}");

        public void Update() => logger.Info($"Update {id}");

        public void Exit() => logger.Info($"Exit {id}");
    }
}
