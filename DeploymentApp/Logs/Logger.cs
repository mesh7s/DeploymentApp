using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DeploymentApp.Logs
{
    public static class Logger
    {
        static Logger()
        {
            Directory.CreateDirectory(Path.Combine(@"C:\temp", "DeploymentAppsLog"));
        }

        public static async Task Log(string message, bool logToAppConsole)
        {
            string logMsg = $"{DateTime.Now:HH:mm:ss} - {message}{Environment.NewLine}";
            await File.AppendAllTextAsync(Path.Combine(@"C:\temp", "DeploymentAppsLog", $"WPFLOG {DateTime.Now:dd-MM-yyyy HH}.txt"), logMsg);
            if (logToAppConsole)
                Application.Current.Dispatcher.Invoke(() => MainWindow.LogsTextBlock.Text += logMsg);
        }
    }
}
