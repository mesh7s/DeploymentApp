using DeploymentApp.Logs;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace DeploymentApp.Dialogs
{
    /// <summary>
    /// Interaction logic for EditDefaultLocationDialog.xaml
    /// </summary>
    public partial class EditDefaultLocationDialog : Window
    {

        private readonly Configuration.Binding _config;

        public EditDefaultLocationDialog()
        {
            InitializeComponent();
            _config = MainWindow.Config;
            txtDefaultLocation.Text = _config.Config.DefaultServerLocation;
            txtDefaultLocation.SelectAll();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _config.UpdateDefaultServerLocation(txtDefaultLocation.Text);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                Task.Run(() => Logger.Log(ex.Message, true));
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtDefaultLocation.Text = _config.Config.AbsoluteDefaultServerLocation;
        }
    }
}
