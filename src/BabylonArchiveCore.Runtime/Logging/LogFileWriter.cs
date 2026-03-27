using BabylonArchiveCore.Core.Logging;

namespace BabylonArchiveCore.Runtime.Logging
{
    /// <summary>
    /// Запись логов в файл. Один файл на сессию запуска.
    /// Путь по умолчанию: logs/session_{timestamp}.log
    /// </summary>
    public sealed class LogFileWriter : ILogWriter, IDisposable
    {
        private readonly StreamWriter _stream;
        private readonly object _lock = new();

        public string FilePath { get; }

        public LogFileWriter(string logDirectory)
        {
            if (string.IsNullOrWhiteSpace(logDirectory))
                throw new ArgumentNullException(nameof(logDirectory));

            Directory.CreateDirectory(logDirectory);

            var fileName = $"session_{DateTime.UtcNow:yyyyMMdd_HHmmss}.log";
            FilePath = Path.Combine(logDirectory, fileName);

            _stream = new StreamWriter(FilePath, append: true, encoding: System.Text.Encoding.UTF8)
            {
                AutoFlush = false
            };

            // Заголовок файла
            _stream.WriteLine($"# Babylon Archive Core — Log Session");
            _stream.WriteLine($"# Started: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            _stream.WriteLine($"# Format: Timestamp|Level|Source|Message");
            _stream.WriteLine("---");
            _stream.Flush();
        }

        public void Write(LogEntry entry)
        {
            lock (_lock)
            {
                _stream.WriteLine(entry.ToFileLine());
            }
        }

        public void Flush()
        {
            lock (_lock)
            {
                _stream.Flush();
            }
        }

        public IReadOnlyList<LogEntry> ReadAll()
        {
            Flush();

            var entries = new List<LogEntry>();
            foreach (var line in File.ReadAllLines(FilePath))
            {
                if (line.StartsWith('#') || line == "---")
                    continue;

                var entry = LogEntry.FromFileLine(line);
                if (entry != null)
                    entries.Add(entry);
            }
            return entries.AsReadOnly();
        }

        public void Dispose()
        {
            Flush();
            _stream.Dispose();
        }
    }
}
