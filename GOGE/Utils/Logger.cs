using System.Text;
using System.Diagnostics;

namespace GOGE.Utils
{
    public static class Logger
    {
        private static readonly object _lock = new object();
        private static readonly string LogFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GOGE", "Logs");
        private static readonly string LogFile = Path.Combine(LogFolder, "goge.log");

        public static void Log(string message)
        {
            try
            {
                lock (_lock)
                {
                    if (!Directory.Exists(LogFolder))
                        Directory.CreateDirectory(LogFolder);

                    var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
                    File.AppendAllText(LogFile, line, Encoding.UTF8);
                }
            }
            catch
            {
                // swallow - logging must not break the app
            }
        }

        public static void Log(Exception ex)
        {
            Log(ex.ToString());
        }

        public static void OpenLog()
        {
            try
            {
                if (!Directory.Exists(LogFolder))
                    Directory.CreateDirectory(LogFolder);

                if (!File.Exists(LogFile))
                {
                    // create empty file
                    File.WriteAllText(LogFile, "", Encoding.UTF8);
                }

                var psi = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"/select,\"{LogFile}\"",
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch
            {
                // ignore failures
            }
        }
    }
}
