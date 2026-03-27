namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Обработка пользовательского ввода из UI.
/// Boundary contract: Runtime → UI (ADR-003).
/// UI передаёт input-действия, Runtime маппит их на команды.
/// </summary>
public interface IInputHandler
{
    /// <summary>Обработать действие игрока.</summary>
    void HandleAction(string actionId, object? payload = null);

    /// <summary>Проверить, доступно ли действие в текущем состоянии.</summary>
    bool IsActionAvailable(string actionId);
}
