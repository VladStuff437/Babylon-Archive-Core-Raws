namespace BabylonArchiveCore.Core.Logging
{
    /// <summary>
    /// Контракт для записи логов в постоянное хранилище (файл).
    /// Реализация — в Runtime.
    /// </summary>
    public interface ILogWriter
    {
        /// <summary>
        /// Записать одну строку в хранилище.
        /// </summary>
        void Write(LogEntry entry);

        /// <summary>
        /// Принудительно сбросить буфер на диск.
        /// </summary>
        void Flush();

        /// <summary>
        /// Прочитать все записи из хранилища.
        /// </summary>
        IReadOnlyList<LogEntry> ReadAll();
    }
}
