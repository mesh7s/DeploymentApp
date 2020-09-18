using DeploymentApp.Logs;
using System.Management.Automation;
using System.Threading.Tasks;
using static DeploymentApp.Enums;

namespace DeploymentApp.Helpers
{
    public static class PSHelper
    {
        public static async Task RunScript(string scriptContents)
        {
            // create a new hosted PowerShell instance using the default runspace.
            // wrap in a using statement to ensure resources are cleaned up.
            using PowerShell ps = PowerShell.Create();
            // specify the script code to run.
            ps.AddScript(scriptContents);

            // specify the parameters to pass into the script.
            //ps.AddParameters(scriptParameters);

            // execute the script and await the result.
            var pipelineObjects = await ps.InvokeAsync().ConfigureAwait(false);

            // print the resulting pipeline objects to the console.
            foreach (var item in pipelineObjects)
            {
                await Logger.Log(item.BaseObject.ToString(), true);
            }
        }

        public static string GetSiteOperationScript(string serverName, string siteName, SiteOperation siteOperation)
        {
            return @$"Invoke-Command -Computername ""{serverName}"" -Scriptblock {{
                (Import-Module WebAdministration);
                {siteOperation}-Website -Name ""{siteName}""
                {siteOperation}-WebAppPool -Name ""{siteName}"";
                }}";
        }
    }
}
