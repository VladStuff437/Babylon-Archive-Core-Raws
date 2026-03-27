namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Диспетчеризация команд к WorldState.
/// Boundary contract: Core → Runtime (ADR-003).
/// UI отправляет команды через этот интерфейс, Runtime обрабатывает.
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>Отправить команду на выполнение.</summary>
    void Dispatch(ICommand command);

    /// <summary>Отправить команду и дождаться результата.</summary>
    bool DispatchAndValidate(ICommand command);
}
