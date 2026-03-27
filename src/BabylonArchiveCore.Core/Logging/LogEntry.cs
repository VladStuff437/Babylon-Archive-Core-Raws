namespace BabylonArchiveCore.Core.Logging
{
    /// <summary>
    /// Одна запись лога. Хранит время, уровень, источник и текст сообщения.
    /// Используется как DTO между слоями Core → Runtime → UI.
    /// </summary>
    public sealed class LogEntry
    {
        public DateTime Timestamp { get; }
        public LogLevel Level { get; }
        public string Source { get; }
        public string Message { get; }

        public LogEntry(DateTime timestamp, LogLevel level, string source, string message)
        {
            Timestamp = timestamp;
            Level = level;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        /// <summary>
        /// Формат для отображения в chat-UI: [HH:mm:ss] [SOURCE] message
        /// </summary>
        public string ToChatLine()
        {
            var tag = Level == LogLevel.UserNote ? "NOTE" : Level.ToString().ToUpperInvariant();
            return $"[{Timestamp:HH:mm:ss}] [{tag}] {Source}: {Message}";
        }

        /// <summary>
        /// Формат для записи в файл: ISO timestamp + полные данные.
        /// </summary>
        public string ToFileLine()
        {
            return $"{Timestamp:yyyy-MM-ddTHH:mm:ss.fff}|{Level}|{Source}|{Message}";
        }

        /// <summary>
        /// Разбор строки из файла обратно в LogEntry.
        /// </summary>
        public static LogEntry? FromFileLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            var parts = line.Split('|', 4);
            if (parts.Length < 4)
                return null;

            if (!DateTime.TryParse(parts[0], out var ts))
                return null;

            if (!Enum.TryParse<LogLevel>(parts[1], out var level))
                level = LogLevel.Info;

            return new LogEntry(ts, level, parts[2], parts[3]);
        }
    }
}
