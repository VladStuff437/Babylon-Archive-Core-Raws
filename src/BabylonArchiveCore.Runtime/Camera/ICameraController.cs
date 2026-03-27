namespace BabylonArchiveCore.Runtime.Camera;

/// <summary>
/// Интерфейс контроллера камеры для режимов Follow/Aim/Inspect.
/// </summary>
public interface ICameraController
{
    string CurrentMode { get; }
    void SetMode(string mode);
    void Update(float deltaTime);
}
