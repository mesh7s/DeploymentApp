using DeploymentApp.Logs;
using DeploymentApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static DeploymentApp.Enums;

namespace DeploymentApp.Dialogs
{
    /// <summary>
    /// Interaction logic for EditServerWebApps.xaml
    /// </summary>
    public partial class WebAppsDialog : Window
    {
        private readonly Configuration.Binding _config;
        private readonly Guid _serverId;
        public WebApp ChangedWebApp;
        public WebAppsDialog(Guid serverId)
        {
            InitializeComponent();
            _serverId = serverId;
            _config = MainWindow.Config;
            icApps.ItemsSource = _config.Config.ServerProfiles.FirstOrDefault(x => x.Id == serverId).Applications;
        }

        private void btnDeleteWebApp_Click(object sender, RoutedEventArgs e) =>
            _config.DeleteWebApp(_serverId, new Guid(((Button)sender).Tag.ToString()));

        private void btnAddWebApp_Click(object sender, RoutedEventArgs e)
        {
            new AddWebAppDialog(ManageProcess.Add, _serverId).ShowDialog();
        }

        private void btnEditWebApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addWebAppDialog = new AddWebAppDialog(ManageProcess.Update, _serverId, _config.GetWebApp(_serverId, new Guid(((Button)sender).Tag.ToString())));
                if (addWebAppDialog.ShowDialog() == true)
                    ChangedWebApp = addWebAppDialog.ChangedWebApp;
            }
            catch (Exception ex)
            {
                Task.Run(() => Logger.Log(ex.Message, true));
            }
        }
    }
}
