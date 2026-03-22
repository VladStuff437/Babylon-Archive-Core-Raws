using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Core.State;
using BabylonArchiveCore.Domain;

namespace BabylonArchiveCore.Runtime.States;

public sealed class BootState(ILogger logger) : IGameState
{
    public SceneId Id => SceneId.Boot;

    public void Enter() => logger.Info("Entered Boot state.");

    public void Update() => logger.Info("Boot state tick.");

    public void Exit() => logger.Info("Exited Boot state.");
}
