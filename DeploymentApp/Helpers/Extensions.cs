using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DeploymentApp.Helpers
{
    public static class Extensions
    {
        #region IO Extensions

        public static Task<FileInfo> CopyToAsync(this FileInfo file, string dest, bool overwrite)
        {
            return Task.Run(() => file.CopyTo(dest, overwrite));
        }

        public static Task DeleteAsync(this FileInfo file)
        {
            return Task.Run(() => { file.Delete(); });
        }

        public static Task DeleteAsync(this DirectoryInfo dir, bool recursive)
        {
            return Task.Run(() => { dir.Delete(recursive); });
        }

        public static Task<FileInfo[]> GetFilesAsync(this DirectoryInfo dir, string searchPattern = "")
        {
            if (string.IsNullOrEmpty(searchPattern))
                return Task.Run(() => dir.GetFiles());
            return Task.Run(() => dir.GetFiles(searchPattern));
        }

        public static Task<DirectoryInfo[]> GetDirectoriesAsync(this DirectoryInfo dir, string searchPattern = "")
        {
            if (string.IsNullOrEmpty(searchPattern))
                return Task.Run(() => dir.GetDirectories());
            return Task.Run(() => dir.GetDirectories(searchPattern));
        }

        #endregion

        public static void BindComboBox<T>(this ComboBox ddl, ObservableCollection<T> source, string text, string value)
        {
            ddl.ItemsSource = source;
            ddl.DisplayMemberPath = text;
            ddl.SelectedValuePath = value;
        }

        public static void ModifyTextBox(this TextBox txt, string text, bool enabled)
        {
            txt.Text = text;
            txt.IsEnabled = enabled;
        }
    }
}
