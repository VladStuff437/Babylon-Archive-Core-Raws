using BabylonArchiveCore.Runtime.State;

namespace BabylonArchiveCore.Runtime.Camera;

/// <summary>
/// Runtime-контроллер камеры, делегирующий состояние CameraStateManager.
/// </summary>
public sealed class CameraController : ICameraController
{
    private readonly CameraStateManager _stateManager;

    public CameraController(CameraStateManager stateManager)
    {
        ArgumentNullException.ThrowIfNull(stateManager);
        _stateManager = stateManager;
    }

    public string CurrentMode => _stateManager.GetState().Mode;

    public void SetMode(string mode) => _stateManager.SetMode(mode);

    public void Update(float deltaTime)
    {
        // Tick-обновление камеры (placeholder для интерполяции/сглаживания)
    }
}
