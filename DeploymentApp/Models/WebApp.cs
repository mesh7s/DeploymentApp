using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentApp.Models
{
    public class WebApp
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FolderName { get; set; }
    }
}
