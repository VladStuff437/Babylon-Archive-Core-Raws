namespace BabylonArchiveCore.Core.Logging
{
    /// <summary>
    /// Контракт сервиса логирования.
    /// Реализация — в Runtime, отображение — в UI.
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// Записать лог-сообщение.
        /// </summary>
        void Log(LogLevel level, string source, string message);

        /// <summary>
        /// Сохранить пользовательскую заметку (от Мастера).
        /// Сохраняется в лог с уровнем UserNote.
        /// </summary>
        void SaveNote(string note);

        /// <summary>
        /// Получить последние N записей лога для отображения в chat-UI.
        /// </summary>
        IReadOnlyList<LogEntry> GetRecentEntries(int count);

        /// <summary>
        /// Получить все записи лога.
        /// </summary>
        IReadOnlyList<LogEntry> GetAllEntries();

        /// <summary>
        /// Событие при добавлении новой записи — UI подписывается для обновления.
        /// </summary>
        event Action<LogEntry>? OnEntryAdded;
    }
}
