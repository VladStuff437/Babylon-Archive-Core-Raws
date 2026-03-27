namespace BabylonArchiveCore.Core.Contracts;

/// <summary>
/// Загрузка и валидация контентных данных (JSON/YAML).
/// Boundary contract: Core → Runtime (INV-007, Pillar P7, ADR-003).
/// </summary>
public interface IContentLoader
{
    /// <summary>Загрузить контент из указанного пути. Возвращает null при ошибке валидации.</summary>
    T? Load<T>(string contentPath) where T : class;

    /// <summary>Валидировать контент без загрузки.</summary>
    bool Validate(string contentPath);
}
