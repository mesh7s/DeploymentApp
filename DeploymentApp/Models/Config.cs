using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentApp.Models
{
    public class Config
    {
        public string DefaultServerLocation { get; set; }
        public ObservableCollection<ServerProfile> ServerProfiles { get; set; }
    }
}
