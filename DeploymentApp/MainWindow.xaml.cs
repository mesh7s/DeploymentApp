using DeploymentApp.Configuration;
using DeploymentApp.Dialogs;
using DeploymentApp.Helpers;
using DeploymentApp.Logs;
using DeploymentApp.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Web.Administration;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static DeploymentApp.Enums;

namespace DeploymentApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static TextBlock LogsTextBlock;
        public static Configuration.Binding _config;
        public ServerProfile SelectedServerProfile { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            _config = new Configuration.Binding();
            Util.BindComboBox(ddlServerProfiles, _config.Config.ServerProfiles, "ProfileName", "Id");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LogsTextBlock = txtbLogs;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                txtFolderPath.Text = dialog.SelectedPath;
            }
        }
        private void btnBrowse2_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                txtFolderPath2.Text = dialog.SelectedPath;
            }
        }
        private void btnBrowse3_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                txtFolderPath3.Text = dialog.SelectedPath;
            }
        }

        public async Task RunScript(string scriptContents)
        {
            // create a new hosted PowerShell instance using the default runspace.
            // wrap in a using statement to ensure resources are cleaned up.
            using PowerShell ps = PowerShell.Create();
            // specify the script code to run.
            ps.AddScript(scriptContents);

            // specify the parameters to pass into the script.
            //ps.AddParameters(scriptParameters);

            // execute the script and await the result.
            var pipelineObjects = await ps.InvokeAsync().ConfigureAwait(false);

            // print the resulting pipeline objects to the console.
            foreach (var item in pipelineObjects)
            {
                await Logger.Log(item.BaseObject.ToString(), true);
            }
        }

        string GetScript(string serverName, string siteName, SiteOperation siteOperation)
        {
            return @$"Invoke-Command -Computername ""{serverName}"" -Scriptblock {{
                (Import-Module WebAdministration);
                {siteOperation}-Website -Name ""{siteName}""
                {siteOperation}-WebAppPool -Name ""{siteName}"";
                }}";
        }

        private async void btnDeploy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnDeploy.IsEnabled = false;
                txtbLogs.Text = _config.Config.DefaultServerLocation;
                //txtbLogs.Text = "";
                //await Logger.Log("-------------------------------------------------------------------------------------\nStarting...", false);
                //SwitchpbStatus();
                //if (string.IsNullOrWhiteSpace(txtFolderPath.Text))
                //{
                //    MessageBox.Show("Folder path is empty");
                //    SwitchpbStatus();
                //    return;
                //}
                //var opsCount = 0;
                //var seconds = 6;
                //if (!string.IsNullOrWhiteSpace(txtFolderPath2.Text))
                //{
                //    await RunScript(GetScript("172.16.18.147", "InternalCoreLocalApi", SiteOperation.Stop));
                //    await Logger.Log($"Starting in {seconds} seconds to ensure process is dead so files won't be locked.", true);
                //    await Util.Delay(seconds);
                //    if (cbBackup.IsChecked == true)
                //    {
                //        await Logger.Log("Creating backup", true);
                //        await CreateBackup(txtFolderPath2.Text);
                //    }
                //    await Logger.Log($"Starting deployment to: {txtFolderPath2.Text}", true);
                //    await HandleFilesAndFoldersAsync(txtFolderPath.Text, txtFolderPath2.Text, cbOverwrite.IsChecked);
                //    await Logger.Log($"Completed deployment to: {txtFolderPath2.Text}", true);
                //    await RunScript(GetScript("spappdev02", "InternalCoreLocalApi", SiteOperation.Start));
                //    opsCount++;
                //}
                //else
                //{
                //    await Logger.Log($"NO PATH FOUND FOR FIRST FOLDER", true);
                //}

                //if (!string.IsNullOrWhiteSpace(txtFolderPath3.Text))
                //{
                //    await RunScript(GetScript("spappdev02", "InternalCoreLocalApi", SiteOperation.Stop));
                //    await Logger.Log($"Starting in {seconds} seconds to ensure process is dead so files won't be locked.", true);
                //    await Util.Delay(seconds);
                //    if (cbBackup.IsChecked == true)
                //    {
                //        await Logger.Log("Creating backup", true);
                //        await CreateBackup(txtFolderPath3.Text);
                //    }
                //    await Logger.Log($"Starting deployment to: {txtFolderPath3.Text}", true);
                //    await HandleFilesAndFoldersAsync(txtFolderPath.Text, txtFolderPath3.Text, cbOverwrite.IsChecked);
                //    await Logger.Log($"Completed deployment to: {txtFolderPath3.Text}", true);
                //    await RunScript(GetScript("spappdev02", "InternalCoreLocalApi", SiteOperation.Start));
                //    opsCount++;
                //}
                //else
                //{
                //    await Logger.Log($"NO PATH FOUND FOR SECOND FOLDER", true);
                //}
                //SwitchpbStatus();

                //if (opsCount > 0)
                //{
                //    await Logger.Log("-------------------------------------------------------------------------------------\nDeployment Complete", false);
                //    MessageBox.Show("Deployment Complete", "Done", icon: MessageBoxImage.Information, button: MessageBoxButton.OK);
                //}
                //else
                //    await Logger.Log("-------------------------------------------------------------------------------------\nNO FOLDERS TO DEPLOY TO FOUND.", false);
                btnDeploy.IsEnabled = true;
            }
            catch (Exception ex)
            {
                btnDeploy.IsEnabled = true;
                await Logger.Log(ex.Message, true);
            }
        }

        async Task HandleFilesAndFoldersAsync(string folderToDeployPath, string folderToDeployToPath, bool? overwriteSettings)
        {
            if (!Directory.Exists(folderToDeployPath))
                Directory.CreateDirectory(folderToDeployPath);
            var folderToDeployTo = new DirectoryInfo(folderToDeployToPath);
            
            await AsyncIO.DeleteFilesAndFoldersAsync(folderToDeployTo, overwriteSettings);
            await AsyncIO.DirectoryCopyAsync(folderToDeployPath, folderToDeployToPath, true);
            if (overwriteSettings == true)
            {
                var appSettingsFileName = "appsettings.json";
                var appSettingsDevFileName = "appsettings.Development.json";
                var tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "temp");
                var appSettingsFile = Path.Combine(folderToDeployTo.FullName, appSettingsFileName);
                var appSettingsDevFile = Path.Combine(folderToDeployTo.FullName, appSettingsDevFileName);
                await AsyncIO.CopyFileAsync(tempFolder, appSettingsFile, true);
                await AsyncIO.CopyFileAsync(tempFolder, appSettingsDevFile, true);
                var tempDir = new DirectoryInfo(tempFolder);
                await tempDir.DeleteAsync(true);
            }
        }

        async Task CreateBackup(string folderToDeployToPath)
        {
            string backupPath;
            var folderToDeployToName = folderToDeployToPath.Substring(folderToDeployToPath.LastIndexOf('\\') + 1);
            var serverCFolder = Util.GetServerCFolder(folderToDeployToPath);
            if (folderToDeployToPath.StartsWith("\\\\"))            
                backupPath = Path.Combine(serverCFolder, "DeploymentAppWebsitesBackup");
            else            
                backupPath = Path.Combine("C:\\", "DeploymentAppWebsitesBackup");

            string fullBackupPath = Path.Combine(backupPath, folderToDeployToName);

            var backupDir = Directory.CreateDirectory(fullBackupPath);
            var backupDirFiles = await backupDir.GetFilesAsync();
            if (backupDirFiles.Length > 0)
            {
                var result = MessageBox.Show($"Backup folder already exists for {folderToDeployToName} in {serverCFolder.Replace("\\c$\\", "")}, do you want to replace it?", "Warning", MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.Cancel:
                        throw new Exception("Operation Canceled");
                    case MessageBoxResult.Yes:
                        await AsyncIO.DeleteFilesAndFoldersAsync(backupDir, true);
                        break;
                    case MessageBoxResult.No:
                        return;
                    default:
                        throw new Exception("Operation Canceled"); 
                }                
            }
            await AsyncIO.DirectoryCopyAsync(folderToDeployToPath, backupDir.FullName, true);
        }

        void SwitchPbStatus()
        {
            pbStatus.IsIndeterminate = !pbStatus.IsIndeterminate;
        }

        void SwitchAppsControls(bool trigger)
        {
            ddlApplications.IsEnabled = trigger;
            btnEditWebApps.IsEnabled = trigger;
        }

        private void btnEditServerProfiles_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ServerProfilesDialog();
            dialog.ShowDialog();
            if (dialog.ChangedServer != null)
            {
                if (dialog.ChangedServer.Id == SelectedServerProfile.Id)
                {
                    txtServerName1.Text = dialog.ChangedServer.FirstServerName;
                    txtServerName2.Text = dialog.ChangedServer.SecondServerName;
                }
            }
        }

        private void btnEditWebApps_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WebAppsDialog(SelectedServerProfile.Id);
            dialog.ShowDialog();
            if (dialog.ChangedWebApp != null)
            {
                if (dialog.ChangedWebApp.Id == ((WebApp)ddlApplications.SelectedItem).Id)
                {
                    txtFolderPath2.Text = dialog.ChangedWebApp.FolderName;
                    txtFolderPath3.Text = dialog.ChangedWebApp.FolderName;
                }
            }
        }

        private void ddlServerProfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ddlServerProfiles.SelectedItem == null) return;
            ClearFields();
            var selectedItem = ((ServerProfile)ddlServerProfiles.SelectedItem);
            SelectedServerProfile = selectedItem;
            txtServerName1.Text = selectedItem.FirstServerName;
            txtServerName2.Text = selectedItem.SecondServerName;
            Util.BindComboBox(ddlApplications, selectedItem.Applications, "Name", "FolderName");
            if (selectedItem.Applications != null)
                SwitchAppsControls(true);
            else
                SwitchAppsControls(false);
        }

        private void ddlApplications_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ddlApplications.SelectedItem == null) return;
            var selectedItem = ((WebApp)ddlApplications.SelectedItem);
            txtFolderPath2.Text = selectedItem.FolderName;
            txtFolderPath3.Text = selectedItem.FolderName;
        }

        private void ClearFields()
        {
            txtFolderPath.Text = string.Empty;
            txtFolderPath2.Text = string.Empty;
            txtFolderPath3.Text = string.Empty;
            txtServerName1.Text = string.Empty;
            txtServerName2.Text = string.Empty;
        }
    }
}
