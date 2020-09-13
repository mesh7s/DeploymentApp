using DeploymentApp.Helpers;
using DeploymentApp.Logs;
using DeploymentApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static DeploymentApp.Enums;

namespace DeploymentApp.Deployment
{
    public class DeploymentManager
    {
        private readonly DeploymentParams _deploymentParams;
        public DeploymentManager(DeploymentParams deploymentParams)
        {
            _deploymentParams = deploymentParams;
        }

        public async Task StartDeploymentProcess()
        {
            var opsCount = 0;
            await Logger.Log("-------------------------------------------------------------------------------------\nStarting...", false);
            if (string.IsNullOrWhiteSpace(_deploymentParams.FolderToDeployPath))
            {
                MessageBox.Show("No folder to deploy was specified");
                return;
            }

            foreach (var deployToProps in _deploymentParams.DeployToProps)
                if (!string.IsNullOrWhiteSpace(deployToProps.ServerName) && !string.IsNullOrWhiteSpace(deployToProps.FolderName))
                    opsCount += await Deploy(_deploymentParams.FolderToDeployPath, _deploymentParams.ServerLocation, deployToProps.ServerName, deployToProps.FolderName, _deploymentParams.Backup, _deploymentParams.Overwrite, _deploymentParams.SecondsToDelay);
                else
                    await Logger.Log($"No folder to deploy to was specified", true);            

            if (opsCount > 0)
            {
                await Logger.Log("-------------------------------------------------------------------------------------\nDeployment Complete", false);
                MessageBox.Show($"{opsCount} {(opsCount > 1 ? "Deployments" : "Deployment")} Complete", "Done", icon: MessageBoxImage.Information, button: MessageBoxButton.OK);
            }
            else
                await Logger.Log("-------------------------------------------------------------------------------------\nNO FOLDERS TO DEPLOY TO FOUND.", false);
        }
        public async Task<int> Deploy(string folderToDeployPath, string defaultServerLocation,string serverName, string folderName, bool? backup, bool? overwrite, int delaySeconds)
        {
            var folderToDeployToPath = Util.PrepareDeployToPath(serverName, defaultServerLocation, folderName);
            await PSHelper.RunScript(PSHelper.GetSiteOperationScript(serverName, folderName, SiteOperation.Stop));
            await Logger.Log($"Starting in {delaySeconds} seconds to ensure process is dead so files won't be locked.", true);
            await Util.Delay(delaySeconds);
            if (backup == true)
            {
                await Logger.Log("Creating backup", true);
                await CreateBackup(folderToDeployToPath, folderName);
            }
            await Logger.Log($"Starting deployment to: {folderToDeployToPath}", true);
            await AsyncIO.HandleFilesAndFoldersAsync(folderToDeployPath, folderToDeployToPath, overwrite);
            await Logger.Log($"Completed deployment to: {folderToDeployToPath}", true);
            await PSHelper.RunScript(PSHelper.GetSiteOperationScript(serverName, folderName, SiteOperation.Start));
            return 1;
        }

        async Task CreateBackup(string folderToDeployToPath, string folderToDeployToName)
        {
            string backupPath;
            if (folderToDeployToPath.StartsWith("\\\\"))
            {
                var serverCFolder = folderToDeployToPath.ToLower().Split("c$")[0] + "c$";
                backupPath = Path.Combine(serverCFolder, "DeploymentAppWebsitesBackup");
            }
            else
                backupPath = Path.Combine("C:\\", "DeploymentAppWebsitesBackup");

            string fullBackupPath = Path.Combine(backupPath, folderToDeployToName);

            var backupDir = Directory.CreateDirectory(fullBackupPath);
            var backupDirFiles = await backupDir.GetFilesAsync();
            if (backupDirFiles.Length > 0)
            {
                var result = MessageBox.Show($"Backup folder already exists for {folderToDeployToName} in {backupPath}, do you want to replace it?", "Warning", MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.Cancel:
                        throw new Exception("Operation Canceled");
                    case MessageBoxResult.Yes:
                        await AsyncIO.DeleteFilesAndFoldersAsync(backupDir, true);
                        break;
                    case MessageBoxResult.No:
                        return;
                    default:
                        throw new Exception("Operation Canceled");
                }
            }
            await AsyncIO.DirectoryCopyAsync(folderToDeployToPath, backupDir.FullName, true);
            await Logger.Log($"Backup created successfully at {backupDir.FullName}", true);
        }


    }
}
