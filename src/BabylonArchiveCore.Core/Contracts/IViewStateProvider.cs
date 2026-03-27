namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Read-only проекция WorldState для UI слоя.
/// Boundary contract: Runtime → UI (ADR-003, Pillar P3).
/// UI получает только snapshot, не ссылку на мутабельное состояние.
/// </summary>
public interface IViewStateProvider
{
    /// <summary>Получить текущий read-only snapshot состояния для UI.</summary>
    IWorldStateReader GetCurrentView();

    /// <summary>True если состояние изменилось с последнего запроса.</summary>
    bool HasChanges { get; }

    /// <summary>Сбросить флаг изменений (вызывается после рендеринга).</summary>
    void AcknowledgeChanges();
}
