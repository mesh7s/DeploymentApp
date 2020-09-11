using DeploymentApp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentApp.Models
{
    public class ServerProfile : NotifyUIBase
    {
        public Guid Id { get; set; }

        private string _profileName;
        public string ProfileName
        {
            get { return _profileName; }
            set
            {
                _profileName = value;
                RaisePropertyChanged("ProfileName");
            }
        }

        private string _firstServerName;
        public string FirstServerName
        {
            get { return _firstServerName; }
            set
            {
                _firstServerName = value;
                RaisePropertyChanged("FirstServerName");
            }
        }

        private string _secondServerName;
        public string SecondServerName
        {
            get { return _secondServerName; }
            set
            {
                _secondServerName = value;
                RaisePropertyChanged("SecondServerName");
            }
        }
        public ObservableCollection<WebApp> Applications { get; set; }
    }
}
