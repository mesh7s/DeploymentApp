using DeploymentApp.Helpers;
using System.Collections.ObjectModel;

namespace DeploymentApp.Models
{
    public class Config : NotifyUIBase
    {
        public string AbsoluteDefaultServerLocation { get; } = "c$\\inetpub\\wwwroot\\";

        private string _defaultServerLocation;
        public string DefaultServerLocation 
        {
            get 
            {
                return _defaultServerLocation; 
            }
            set 
            {
                _defaultServerLocation = value;
                RaisePropertyChanged("DefaultServerLocation");

            }
        }
        public ObservableCollection<ServerProfile> ServerProfiles { get; set; }
    }
}
