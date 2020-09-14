using DeploymentApp.Logs;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Net.Http;
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

        public static string PrepareDeployToPath(string serverName, string location, string folderName)
        {
            if (serverName.ToLower() == "c:")
                return Path.Combine(serverName, location.ToLower().Replace("c$\\", string.Empty), folderName);
            return Path.Combine($"\\\\{serverName}", location, folderName);
        }

        public static async Task<Bitmap> GetBitmapFromUrl(string imageUrl)
        {
            using var client = new HttpClient();
            var response = client.GetAsync(imageUrl).Result;
            var stream = await response.Content.ReadAsStreamAsync();
            return new Bitmap(stream);
        }

        public static Bitmap Base64StringToBitmap(string base64String)
        {
            Bitmap bmpReturn = null;
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            using (var memoryStream = new MemoryStream(byteBuffer))
            {
                memoryStream.Position = 0;
                bmpReturn = (Bitmap)System.Drawing.Image.FromStream(memoryStream);
            }
            return bmpReturn;
        }
    }
}
