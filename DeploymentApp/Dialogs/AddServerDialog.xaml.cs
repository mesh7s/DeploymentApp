using DeploymentApp.Logs;
using DeploymentApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static DeploymentApp.Enums;

namespace DeploymentApp.Dialogs
{
    /// <summary>
    /// Interaction logic for AddServerDialog.xaml
    /// </summary>
    public partial class AddServerDialog : Window
    {
        private readonly ManageProcess _process;
        private ServerProfile profileForUpdate;
        private readonly Configuration.Binding _config;
        public ServerProfile ChangedProfile { get; set; }
        public AddServerDialog(ManageProcess process, ServerProfile profileForUpdate = null)
        {
            InitializeComponent();
            _config = MainWindow.Config;
            _process = process;
            this.profileForUpdate = profileForUpdate;
            if (process == ManageProcess.Update)
            {
                txtProfileName.Text = profileForUpdate.ProfileName;
                txtServerName1.Text = profileForUpdate.FirstServerName;
                txtServerName2.Text = profileForUpdate.SecondServerName;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtProfileName.Text) || string.IsNullOrWhiteSpace(txtServerName1.Text)) return;

                if (_process == ManageProcess.Add)
                {
                    var newProfile = new ServerProfile
                    {
                        Id = Guid.NewGuid(),
                        ProfileName = txtProfileName.Text,
                        FirstServerName = txtServerName1.Text,
                        SecondServerName = txtServerName2.Text,
                        Applications = new ObservableCollection<WebApp>()
                    };
                    _config.AddServerProfile(newProfile);
                    ChangedProfile = newProfile;
                }
                else
                {
                    profileForUpdate.ProfileName = txtProfileName.Text;
                    profileForUpdate.FirstServerName = txtServerName1.Text;
                    profileForUpdate.SecondServerName = txtServerName2.Text;
                    _config.UpdateServerProfile(profileForUpdate);
                    ChangedProfile = profileForUpdate;
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                Task.Run(() => Logger.Log(ex.Message, true));
            }
        }
        private new void GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox textBox && !textBox.IsReadOnly && e.KeyboardDevice.IsKeyDown(Key.Tab))
                textBox.SelectAll();
        }
    }
}
