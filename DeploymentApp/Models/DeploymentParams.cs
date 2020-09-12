using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentApp.Models
{
    public class DeploymentParams
    {
        public string FolderToDeployPath { get; set; }
        public string ServerLocation { get; set; }
        public string FirstServerName { get; set; }
        public string FirstFolderName { get; set; }
        public string SecondServerName { get; set; }
        public string SecondFolderName { get; set; }
        public bool? Backup { get; set; }
        public bool? Overwrite { get; set; }
        public int SecondsToDelay { get; set; }

    }
}
