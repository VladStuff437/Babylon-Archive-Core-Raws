using BabylonArchiveCore.Domain;

namespace BabylonArchiveCore.Core.State;

public sealed class GameStateRouter
{
    private readonly Dictionary<SceneId, IGameState> _states = new();

    public IGameState? CurrentState { get; private set; }

    public void Register(IGameState gameState)
    {
        ArgumentNullException.ThrowIfNull(gameState);
        _states[gameState.Id] = gameState;
    }

    public bool TrySwitch(SceneId target)
    {
        if (!_states.TryGetValue(target, out var nextState))
        {
            return false;
        }

        CurrentState?.Exit();
        CurrentState = nextState;
        CurrentState.Enter();
        return true;
    }

    public void UpdateCurrent()
    {
        CurrentState?.Update();
    }
}
