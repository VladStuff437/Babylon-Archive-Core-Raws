using BabylonArchiveCore.Core.Logging;

namespace BabylonArchiveCore.UI.Logging
{
    /// <summary>
    /// Chat-like окно логирования. Отображает записи лога в виде ленты сообщений
    /// и предоставляет строку ввода для заметок Мастера.
    ///
    /// Интеграция с конкретным UI-фреймворком (WPF/Avalonia/Console) —
    /// через наследование или адаптер.
    /// </summary>
    public class LogChatWindow
    {
        private readonly ILogService _logService;
        private readonly List<string> _displayLines = new();
        private readonly int _maxVisibleLines;

        /// <summary>
        /// Текущие строки для отображения в UI.
        /// </summary>
        public IReadOnlyList<string> DisplayLines => _displayLines.AsReadOnly();

        /// <summary>
        /// Вызывается при обновлении отображаемых строк — UI должен перерисоваться.
        /// </summary>
        public event Action? OnDisplayUpdated;

        /// <summary>
        /// Текущий текст в строке ввода.
        /// </summary>
        public string InputText { get; set; } = string.Empty;

        public LogChatWindow(ILogService logService, int maxVisibleLines = 200)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _maxVisibleLines = maxVisibleLines;

            // Подписка на новые записи
            _logService.OnEntryAdded += OnNewEntry;

            // Загрузить существующие записи
            RefreshFromService();
        }

        /// <summary>
        /// Отправить заметку из строки ввода.
        /// </summary>
        public void SubmitNote()
        {
            var text = InputText?.Trim();
            if (string.IsNullOrEmpty(text))
                return;

            _logService.SaveNote(text);
            InputText = string.Empty;
        }

        /// <summary>
        /// Обработка горячих клавиш ввода.
        /// </summary>
        public void OnInputKeyPress(ConsoleKey key)
        {
            if (key == ConsoleKey.Enter)
            {
                SubmitNote();
            }
        }

        /// <summary>
        /// Получить цвет строки по уровню лога (для UI-рендера).
        /// </summary>
        public static string GetColorForLevel(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => "#888888",
                LogLevel.Debug => "#AAAAAA",
                LogLevel.Info => "#FFFFFF",
                LogLevel.Warning => "#FFD700",
                LogLevel.Error => "#FF4444",
                LogLevel.Fatal => "#FF0000",
                LogLevel.UserNote => "#00BFFF",  // Заметки — голубым
                _ => "#FFFFFF"
            };
        }

        private void OnNewEntry(LogEntry entry)
        {
            var line = entry.ToChatLine();
            _displayLines.Add(line);

            // Обрезаем сверху если превышен лимит
            while (_displayLines.Count > _maxVisibleLines)
            {
                _displayLines.RemoveAt(0);
            }

            OnDisplayUpdated?.Invoke();
        }

        private void RefreshFromService()
        {
            _displayLines.Clear();
            var entries = _logService.GetRecentEntries(_maxVisibleLines);
            foreach (var entry in entries)
            {
                _displayLines.Add(entry.ToChatLine());
            }
            OnDisplayUpdated?.Invoke();
        }
    }
}
