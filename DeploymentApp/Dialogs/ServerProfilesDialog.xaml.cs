using DeploymentApp.Logs;
using DeploymentApp.Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static DeploymentApp.Enums;

namespace DeploymentApp.Dialogs
{
    /// <summary>
    /// Interaction logic for ServerProfilesDialog.xaml
    /// </summary>
    public partial class ServerProfilesDialog : Window
    {
        private readonly Configuration.Binding _config;
        public ServerProfile ChangedServer;
        public ServerProfilesDialog()
        {
            InitializeComponent();
            _config = MainWindow.Config;
            icServerProfiles.ItemsSource = _config.GetServerProfiles();
        }

        private void btnDeleteServerProfile_Click(object sender, RoutedEventArgs e) =>        
            _config.DeleteServerProfile(new Guid(((Button)sender).Tag.ToString()));

        private void btnAddServerProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addServerDialog = new AddServerDialog(ManageProcess.Add);
                addServerDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                Task.Run(() => Logger.Log(ex.Message, true));
            }
        }

        private void btnEditServerProfile_Click(object sender, RoutedEventArgs e)
        {
            var addServerDialog = new AddServerDialog(ManageProcess.Update, _config.GetServerProfile(new Guid(((Button)sender).Tag.ToString())));
            if (addServerDialog.ShowDialog() == true)
            {
                ChangedServer = addServerDialog.ChangedProfile;
            }
        }
    }
}
