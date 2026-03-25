using System.Windows.Forms;
using BabylonArchiveCore.Domain.World.Runtime;
using System.Threading;

namespace BabylonArchiveCore.Desktop;

internal static class Program
{
    private const string SingleInstanceMutexName = "Global\\BabylonArchiveCore.Desktop.SingleInstance";

    [STAThread]
    private static void Main(string[] args)
    {
        try
        {
            using var singleInstanceMutex = new Mutex(true, SingleInstanceMutexName, out var createdNew);
            if (!createdNew)
            {
                MessageBox.Show(
                    "Клиент уже запущен. Закройте текущий экземпляр перед новым запуском.",
                    "Babylon Archive Core Desktop",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var profile = args.Any(a =>
                a.Equals("--admin", StringComparison.OrdinalIgnoreCase) ||
                a.Equals("--mode=admin", StringComparison.OrdinalIgnoreCase))
                ? WorldRuntimeProfile.AdminDefault
                : WorldRuntimeProfile.PlayerDefault;

            ApplicationConfiguration.Initialize();
            Application.Run(new PrologueClientForm(profile));
        }
        catch (Exception ex)
        {
            var logPath = Path.Combine(AppContext.BaseDirectory, "desktop-client-error.log");
            File.WriteAllText(logPath, ex.ToString());
            MessageBox.Show(
                $"Desktop client failed to start.\n\n{ex.Message}\n\nSee: {logPath}",
                "Babylon Archive Core Desktop",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
