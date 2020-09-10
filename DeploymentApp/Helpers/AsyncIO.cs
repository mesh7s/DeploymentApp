using DeploymentApp.Logs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentApp.Helpers
{
    public static class AsyncIO
    {
        public static Task CreateDirectoryAsync(string destDirName)
        {
            return Task.Run(() => { Directory.CreateDirectory(destDirName); });
        }

        public static Task CopyFileAsync(string sourceFileName, string destFileName, bool overwrite = false)
        {
            return Task.Run(() => File.Copy(sourceFileName, destFileName, overwrite));
        }

        public static async Task DeleteFilesAndFoldersAsync(DirectoryInfo folderToDeployTo, bool? overwriteSettings = false)
        {
            if (overwriteSettings == false)
            {
                var appSettingsFileName = "appsettings.json";
                var appSettingsDevFileName = "appsettings.Development.json";
                var tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "temp");
                await CreateDirectoryAsync(tempFolder);
                var appSettingsFile = Path.Combine(folderToDeployTo.FullName, appSettingsFileName);
                var appSettingsDevFile = Path.Combine(folderToDeployTo.FullName, appSettingsDevFileName);
                if (File.Exists(appSettingsFile))
                {
                    await CopyFileAsync(appSettingsFile, Path.Combine(tempFolder, appSettingsFileName), true);
                    await Logger.Log("Done Copying appsettings.json to temp folder.", true);
                }
                if (File.Exists(appSettingsDevFile))
                {
                    await CopyFileAsync(appSettingsDevFile, Path.Combine(tempFolder, appSettingsDevFileName), true);
                    await Logger.Log("Done Copying appsettings.Development.json to temp folder.", true);
                }
            }

            foreach (var file in folderToDeployTo.GetFiles())
            {
                await file.DeleteAsync();
                await Logger.Log($"file {file.Name} deleted.", false);
            }

            foreach (var dir in folderToDeployTo.GetDirectories())
            {
                await dir.DeleteAsync(true);
                await Logger.Log($"directory {dir.Name} deleted with all of its content.", false);
            }

            await Logger.Log($"Deleted all folders from {folderToDeployTo.FullName}", true);
        }

        public static async Task DirectoryCopyAsync(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = await dir.GetDirectoriesAsync();

            // If the destination directory doesn't exist, create it.       
            await CreateDirectoryAsync(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = await dir.GetFilesAsync();
            await Logger.Log($"Starting copying files...", true);
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                await file.CopyToAsync(temppath, false);
                await Logger.Log($"File {file.Name} was copied to {destDirName}", false);
            }
            await Logger.Log($"Copied all files.", true);

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                await Logger.Log($"Copying subfolders", true);
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    await Logger.Log($"Copying folder {subdir.FullName} to {temppath}", false);
                    await DirectoryCopyAsync(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
