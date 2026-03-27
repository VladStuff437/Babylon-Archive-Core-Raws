namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Мутация WorldState через command pattern.
/// Boundary contract: Core → Runtime (INV-001, ADR-003).
/// Только Runtime реализует этот интерфейс.
/// </summary>
public interface IWorldStateMutator
{
    /// <summary>Применить команду к WorldState. Возвращает true при успехе.</summary>
    bool ApplyCommand(ICommand command);

    /// <summary>Продвинуть WorldTick на один шаг.</summary>
    void AdvanceTick();
}
