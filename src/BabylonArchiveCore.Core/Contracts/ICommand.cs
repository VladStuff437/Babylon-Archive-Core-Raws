namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Базовый контракт команды изменения состояния мира.
/// Все мутации WorldState проходят через команды (INV-001, ADR-003).
/// </summary>
public interface ICommand
{
    /// <summary>Уникальный идентификатор типа команды.</summary>
    string CommandType { get; }

    /// <summary>Тик, в который команда была создана.</summary>
    long CreatedAtTick { get; }
}
