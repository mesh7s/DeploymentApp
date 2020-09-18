using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeploymentApp.Models;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;

namespace DeploymentApp.Configuration
{
    public class Binding
    {
        
        public Config Config;
        private readonly string _appsettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DeploymentApp", "appsettings.json");
        public Binding()
        {
            if (!File.Exists(_appsettingsFilePath)) CreateSettingsFile();
            var json = File.ReadAllText(_appsettingsFilePath);
            Config = JsonConvert.DeserializeObject<Config>(json);
        }

        private void CreateSettingsFile()
        {
            var file = File.Create(_appsettingsFilePath);
            file.Close(); // closing file to be able to write text to it.
            File.WriteAllText(file.Name, JsonConvert.SerializeObject(new Config
            {
                DefaultServerLocation = "c$\\inetpub\\wwwroot\\",
                ServerProfiles = new ObservableCollection<ServerProfile>()
            }, Formatting.Indented));
        }

        public void UpdateDefaultServerLocation(string newLocation)
        {
            Config.DefaultServerLocation = newLocation;
            SaveChanges();
        }

        public ObservableCollection<ServerProfile> GetServerProfiles()
        {
            return Config.ServerProfiles;
        }
        
        public ServerProfile GetServerProfile(Guid id)
        {
            return Config.ServerProfiles.FirstOrDefault(x => x.Id == id);
        }

        public void AddServerProfile(ServerProfile profile)
        {
            Config.ServerProfiles.Add(profile);
            SaveChanges();
        }

        public void DeleteServerProfile(Guid id)
        {
            var profileToDelete = Config.ServerProfiles.FirstOrDefault(x => x.Id == id);
            if (profileToDelete == null) return;
            Config.ServerProfiles.Remove(profileToDelete);
            SaveChanges();
        }

        internal void UpdateServerProfile(ServerProfile serverProfile)
        {
            var profileToUpdate = Config.ServerProfiles.FirstOrDefault(x => x.Id == serverProfile.Id);
            if (profileToUpdate == null) return;
            profileToUpdate = serverProfile;
            SaveChanges();
        }

        public WebApp GetWebApp(Guid serverId, Guid id)
        {
            var serverProfile = Config.ServerProfiles.FirstOrDefault(x => x.Id == serverId);
            return serverProfile.Applications.FirstOrDefault(x => x.Id == id);
        }

        public void AddWebApp(Guid serverId, WebApp app)
        {
            Config.ServerProfiles.FirstOrDefault(x => x.Id == serverId).Applications.Add(app);
            SaveChanges();
        }

        public void DeleteWebApp(Guid serverId, Guid appId)
        {
            var serverProfile = Config.ServerProfiles.FirstOrDefault(x => x.Id == serverId);            
            serverProfile.Applications.Remove(serverProfile.Applications.FirstOrDefault(x => x.Id == appId));
            SaveChanges();
        }

        internal void UpdateWebApp(Guid serverId, WebApp app)
        {
            var serverProfile = Config.ServerProfiles.FirstOrDefault(x => x.Id == serverId);
            var webAppToUpdate = serverProfile.Applications.FirstOrDefault(x => x.Id == app.Id);
            if (webAppToUpdate == null) return;
            webAppToUpdate = app;
            SaveChanges();
        }

        private void SaveChanges() =>
            File.WriteAllText(_appsettingsFilePath, JsonConvert.SerializeObject(Config, Formatting.Indented));
    }
}
