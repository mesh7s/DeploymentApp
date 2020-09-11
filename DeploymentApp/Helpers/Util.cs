using DeploymentApp.Logs;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DeploymentApp.Helpers
{
    public static class Util
    {
        public static bool IsAppSettingsFile(string fileName)
        {
            var loweredFileName = fileName.ToLower();
            return loweredFileName.Contains("appsettings") && loweredFileName.Contains(".json");
        }

        public static async Task Delay(int seconds)
        {
            for (int i = seconds; i > 0; i--)
            {
                await Logger.Log($"Starting in {i} seconds", true);
                await Task.Delay(1000);
            }
        }

        public static string GetServerCFolder(string url)
        {
            int counter = 0;
            var sb = new StringBuilder();
            foreach (var c in url)
            {
                if (c == '\\') counter++;
                if (counter == 3)
                {
                    sb.Append("\\c$\\");
                    break;
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        public static void BindComboBox<T>(ComboBox ddl, ObservableCollection<T> source,string text, string value)
        {
            ddl.ItemsSource = source;
            ddl.DisplayMemberPath = text;
            ddl.SelectedValuePath = value;
        }
    }
}
