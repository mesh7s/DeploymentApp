using DeploymentApp.Logs;
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
