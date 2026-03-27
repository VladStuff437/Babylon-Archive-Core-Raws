using BabylonArchiveCore.Core.Logging;

namespace BabylonArchiveCore.Runtime.Logging
{
    /// <summary>
    /// Реализация ILogService — центральный сервис логирования.
    /// Хранит записи в памяти и пишет в файл через ILogWriter.
    /// </summary>
    public sealed class LogService : ILogService, IDisposable
    {
        private readonly List<LogEntry> _entries = new();
        private readonly ILogWriter _writer;
        private readonly object _lock = new();

        public event Action<LogEntry>? OnEntryAdded;

        public LogService(ILogWriter writer)
        {
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public void Log(LogLevel level, string source, string message)
        {
            var entry = new LogEntry(DateTime.UtcNow, level, source, message);

            lock (_lock)
            {
                _entries.Add(entry);
            }

            _writer.Write(entry);
            OnEntryAdded?.Invoke(entry);
        }

        public void SaveNote(string note)
        {
            if (string.IsNullOrWhiteSpace(note))
                return;

            Log(LogLevel.UserNote, "Master", note);
        }

        public IReadOnlyList<LogEntry> GetRecentEntries(int count)
        {
            lock (_lock)
            {
                if (count >= _entries.Count)
                    return _entries.ToList().AsReadOnly();

                return _entries.Skip(_entries.Count - count).ToList().AsReadOnly();
            }
        }

        public IReadOnlyList<LogEntry> GetAllEntries()
        {
            lock (_lock)
            {
                return _entries.ToList().AsReadOnly();
            }
        }

        public void Dispose()
        {
            _writer.Flush();
        }
    }
}
