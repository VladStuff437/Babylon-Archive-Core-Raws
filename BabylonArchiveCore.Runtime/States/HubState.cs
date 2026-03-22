using BabylonArchiveCore.Core.Logging;
using BabylonArchiveCore.Core.State;
using BabylonArchiveCore.Domain;

namespace BabylonArchiveCore.Runtime.States;

public sealed class HubState(ILogger logger) : IGameState
{
    public SceneId Id => SceneId.HubA0;

    public void Enter() => logger.Info("Entered Hub A-0 state.");

    public void Update() => logger.Info("Hub A-0 tick.");

    public void Exit() => logger.Info("Exited Hub A-0 state.");
}
