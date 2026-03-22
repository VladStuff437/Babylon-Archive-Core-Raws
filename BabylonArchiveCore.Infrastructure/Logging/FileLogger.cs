using BabylonArchiveCore.Core.Logging;

namespace BabylonArchiveCore.Infrastructure.Logging;

public sealed class FileLogger : ILogger
{
    private readonly string _filePath;
    private readonly object _syncRoot = new();

    public FileLogger(string filePath)
    {
        _filePath = filePath;
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public void Info(string message) => Write("INFO", message);

    public void Warn(string message) => Write("WARN", message);

    public void Error(string message) => Write("ERROR", message);

    private void Write(string level, string message)
    {
        var line = $"[{DateTime.UtcNow:O}] [{level}] {message}";

        lock (_syncRoot)
        {
            File.AppendAllLines(_filePath, [line]);
        }
    }
}
