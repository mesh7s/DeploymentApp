using DeploymentApp.Configuration;
using DeploymentApp.Logs;
using DeploymentApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            _config = MainWindow._config;
            icServerProfiles.ItemsSource = _config.GetServerProfiles();
        }

        private void btnDeleteServerProfile_Click(object sender, RoutedEventArgs e) =>        
            _config.DeleteServerProfile(new Guid(((Button)sender).Tag.ToString()));

        private void btnAddServerProfile_Click(object sender, RoutedEventArgs e)
        {
            var addServerDialog = new AddServerDialog(ManageProcess.Add);
            addServerDialog.ShowDialog();
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
