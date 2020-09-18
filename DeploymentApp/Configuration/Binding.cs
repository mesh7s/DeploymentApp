using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DeploymentApp.Models;
using System.Collections.ObjectModel;
using DeploymentApp.Helpers;

namespace DeploymentApp.Configuration
{
    public class Binding
    {
        
        public Config Config;
        private static readonly string deploymentAppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DeploymentApp");
        private readonly string _appsettingsFilePath;
        public Binding()
        {
            Task.Run(() => AsyncIO.CreateDirectoryAsync(deploymentAppFolder)).GetAwaiter().GetResult();
            var appsettingsFilePath = Path.Combine(deploymentAppFolder, "appsettings.json");
            _appsettingsFilePath = appsettingsFilePath;
            if (!File.Exists(appsettingsFilePath)) CreateSettingsFile(appsettingsFilePath);
            var json = File.ReadAllText(appsettingsFilePath);
            Config = JsonConvert.DeserializeObject<Config>(json);
        }

        private void CreateSettingsFile(string settingsFilePath)
        {
            var file = File.Create(settingsFilePath);
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
