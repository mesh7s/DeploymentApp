using DeploymentApp.Helpers;
using DeploymentApp.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DeploymentApp.Enums;

namespace DeploymentApp.Deployment
{
    public static class Deployer
    {
        public static async Task<int> Deploy(string folderToDeployPath, string defaultServerLocation,string serverName, string folderName, bool? backup, bool? overwrite, int delaySeconds)
        {
            var folderToDeployToPath = Util.PrepareDeployToPath(serverName, defaultServerLocation, folderName);
            await PSHelper.RunScript(PSHelper.GetSiteOperationScript(serverName, folderName, SiteOperation.Stop));
            await Logger.Log($"Starting in {delaySeconds} seconds to ensure process is dead so files won't be locked.", true);
            await Util.Delay(delaySeconds);
            if (backup == true)
            {
                await Logger.Log("Creating backup", true);
                await AsyncIO.CreateBackup(folderToDeployToPath, folderName);
            }
            await Logger.Log($"Starting deployment to: {folderToDeployToPath}", true);
            await AsyncIO.HandleFilesAndFoldersAsync(folderToDeployPath, folderToDeployToPath, overwrite);
            await Logger.Log($"Completed deployment to: {folderToDeployToPath}", true);
            await PSHelper.RunScript(PSHelper.GetSiteOperationScript(serverName, folderName, SiteOperation.Start));
            return 1;
        }
    }
}
