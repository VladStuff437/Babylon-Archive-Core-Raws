using BabylonArchiveCore.Core.Logging;

namespace BabylonArchiveCore.UI.Logging
{
    /// <summary>
    /// Консольный рендер LogChatWindow — для отладки и раннего прототипирования.
    /// Отображает лог-ленту в консоли и принимает ввод заметок.
    /// </summary>
    public class ConsoleLogRenderer
    {
        private readonly LogChatWindow _chatWindow;
        private readonly ILogService _logService;

        public ConsoleLogRenderer(ILogService logService)
        {
            _logService = logService;
            _chatWindow = new LogChatWindow(logService, maxVisibleLines: 50);
            _chatWindow.OnDisplayUpdated += Render;
        }

        /// <summary>
        /// Запуск интерактивного цикла ввода заметок (блокирующий).
        /// </summary>
        public void RunInputLoop()
        {
            _logService.Log(LogLevel.Info, "LogUI", "Chat-log window started. Type a note and press Enter. Type /quit to exit.");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("[NOTE] > ");
                Console.ResetColor();

                var input = Console.ReadLine();
                if (input == null || input.Trim().Equals("/quit", StringComparison.OrdinalIgnoreCase))
                    break;

                if (!string.IsNullOrWhiteSpace(input))
                {
                    _chatWindow.InputText = input;
                    _chatWindow.SubmitNote();
                }
            }

            _logService.Log(LogLevel.Info, "LogUI", "Chat-log window closed.");
        }

        private void Render()
        {
            // Простой вывод последней строки
            var lines = _chatWindow.DisplayLines;
            if (lines.Count == 0)
                return;

            var lastLine = lines[^1];
            var color = GetConsoleColor(lastLine);
            Console.ForegroundColor = color;
            Console.WriteLine(lastLine);
            Console.ResetColor();
        }

        private static ConsoleColor GetConsoleColor(string line)
        {
            if (line.Contains("[NOTE]")) return ConsoleColor.Cyan;
            if (line.Contains("[ERROR]") || line.Contains("[FATAL]")) return ConsoleColor.Red;
            if (line.Contains("[WARNING]")) return ConsoleColor.Yellow;
            if (line.Contains("[DEBUG]") || line.Contains("[TRACE]")) return ConsoleColor.DarkGray;
            return ConsoleColor.Gray;
        }
    }
}
