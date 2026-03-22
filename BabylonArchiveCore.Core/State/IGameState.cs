using BabylonArchiveCore.Domain;

namespace BabylonArchiveCore.Core.State;

public interface IGameState
{
    SceneId Id { get; }

    void Enter();

    void Update();

    void Exit();
}
