using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentApp.Config
{
    public class Binding
    {
        public Config Config;
        public Binding()
        {
            var appsettingsFile = Path.Combine(@"C:\Users\alayedm\source\repos\DeploymentApp\DeploymentApp\Config", "appsettings.json");
            var json = File.ReadAllText(appsettingsFile);
            Config = JsonConvert.DeserializeObject<Config>(json);
        }
    }
}
