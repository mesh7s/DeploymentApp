using DeploymentApp.Logs;
using DeploymentApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for AddWebAppDialog.xaml
    /// </summary>
    public partial class AddWebAppDialog : Window
    {
        private readonly ManageProcess _process;
        private WebApp _webAppForUpdate;
        private readonly Configuration.Binding _config;
        private readonly Guid _serverId;
        public WebApp ChangedWebApp { get; set; }

        public AddWebAppDialog(ManageProcess process, Guid serverId = new Guid(), WebApp webAppForUpdate = null)
        {
            InitializeComponent();
            _config = MainWindow.Config;
            _process = process;
            _webAppForUpdate = webAppForUpdate;
            _serverId = serverId;
            if (process == ManageProcess.Update)
            {
                txtWebAppName.Text = _webAppForUpdate.Name;
                txtFolderName.Text = _webAppForUpdate.FolderName;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtWebAppName.Text) || string.IsNullOrWhiteSpace(txtFolderName.Text)) return;

                if (_process == ManageProcess.Add)
                    _config.AddWebApp(_serverId, new WebApp
                    {
                        Id = Guid.NewGuid(),
                        Name = txtWebAppName.Text,
                        FolderName = txtFolderName.Text
                    });
                else
                {
                    _webAppForUpdate.Name = txtWebAppName.Text;
                    _webAppForUpdate.FolderName = txtFolderName.Text;
                    _config.UpdateWebApp(_serverId, _webAppForUpdate);
                    ChangedWebApp = _webAppForUpdate;
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                Task.Run(() => Logger.Log(ex.Message, true));
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
