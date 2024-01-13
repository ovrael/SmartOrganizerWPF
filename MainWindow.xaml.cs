using Microsoft.WindowsAPICodePack.Dialogs;

using SmartOrganizerWPF.Common;
using SmartOrganizerWPF.Common.LoadFiles;
using SmartOrganizerWPF.Common.Trees;
using SmartOrganizerWPF.Models;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SmartOrganizerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DirectoryData? selectedDirectory;
        private int oldFolderIndex = -1;

        private SettingsWindow? settingsWindow = null;

        private readonly ExplorerTree loadedFilesTree;
        private readonly OrganizedTree organizedTree;

        private CancellationTokenSource scanCancelTokenSource;
        private bool initialized = false;

        public MainWindow()
        {
            InitializeComponent();
            UserSettings.LoadFromFile();

            // ADD ALSO USER DEFINED DIRECTORIES
            SelectFolderComboBox.Items.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            SelectFolderComboBox.Items.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            SelectFolderComboBox.Items.Add(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            SelectFolderComboBox.Items.Add($"C:\\Users\\{Environment.UserName}\\Downloads");
#if DEBUG
            SelectFolderComboBox.Items.Add($"D:\\_Test");
#endif
            SelectFolderComboBox.Items.Add("Select folder from explorer...");
            SelectFolderComboBox.SelectedIndex = SelectFolderComboBox.Items.Count - 1;

            settingsWindow = null;

            FileTypesComboBox.ItemsSource = Enum.GetValues(typeof(FileType)).Cast<FileType>();
            FileTypesComboBox.SelectedIndex = 0;
            FileTypesComboBox.SelectedValue = FileType.All;

            loadedFilesTree = new ExplorerTree(ExplorerTreeView);
            organizedTree = new OrganizedTree(OrganizedTreeView);

            // Is scanning
            ScanButton.Tag = false;
            initialized = true;
        }

        private void ChangeScanStatus(bool isScanning)
        {
            ScanButton.Tag = isScanning;
            ChangeScanButton(isScanning);
            OrganizeButton.IsEnabled = !isScanning;
            LoadProgressBar.IsIndeterminate = isScanning;
        }

        private async void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            string? scanPath = SelectFolderComboBox.SelectedItem as string;
            if (scanPath == null || scanPath.Length == 0) return;

            if (sender is not Button scanButton) return;
            if (scanButton.Tag is not bool isLoading) return;

            if (!isLoading)
            {
                isLoading = true;
                try
                {
                    ChangeScanStatus(isLoading);
                    MoveFilesButton.IsEnabled = false;

                    scanCancelTokenSource = new CancellationTokenSource();
                    await LoadDirectory(scanPath, scanCancelTokenSource.Token);

                    oldFolderIndex = SelectFolderComboBox.SelectedIndex;
                }
                catch (Exception ex)
                {
                    SelectFolderComboBox.SelectedIndex = oldFolderIndex;

                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                scanCancelTokenSource.Cancel();
            }

            ChangeScanStatus(false);
        }
        private async void SelectFolderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!initialized) return;

            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null || comboBox.SelectedItem == null) return;

            string folderName = comboBox.SelectedItem.ToString();
            if (folderName == null || folderName.Length == 0) return;

            // Selected: choose other directory
            if (comboBox.SelectedIndex == SelectFolderComboBox.Items.Count - 1)
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                SelectFolderComboBox.Items.Insert(SelectFolderComboBox.Items.Count - 1, dialog.FileName);
                SelectFolderComboBox.SelectedIndex = SelectFolderComboBox.Items.Count - 2;
                folderName = dialog.FileName;

                dialog.Dispose();
                dialog = null;
            }

            try
            {
                MoveFilesButton.IsEnabled = false;
                ChangeScanStatus(true);

                scanCancelTokenSource = new CancellationTokenSource();
                await LoadDirectory(folderName, scanCancelTokenSource.Token);

                oldFolderIndex = comboBox.SelectedIndex;
                LoadProgressBar.IsIndeterminate = false;

            }
            catch (Exception ex)
            {
                //(SelectFolderComboBox.Items[comboBox.SelectedIndex] as ComboBoxItem).IsEnabled = false;
                SelectFolderComboBox.SelectedIndex = oldFolderIndex;

                MessageBox.Show(ex.ToString());
            }

            ChangeScanStatus(false);
        }

        private void ChangeScanButton(bool isLoading)
        {
            if (isLoading)
            {
                ScanButton.Content = "Cancel";
                ScanButton.Background = new System.Windows.Media.SolidColorBrush(new System.Windows.Media.Color()
                { R = 250, G = 80, B = 80, A = 255 });
            }
            else
            {
                ScanButton.Content = "Scan";
                ScanButton.Background = new System.Windows.Media.SolidColorBrush(new System.Windows.Media.Color()
                { R = 30, G = 220, B = 50, A = 255 });
            }
        }

        private async Task LoadDirectory(string selectedFolderPath, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }


            if (selectedFolderPath == null) return;
            if (selectedFolderPath == string.Empty) return;
            if (selectedFolderPath.ToLower() == "selected folder") return;
            loadingLabel.Content = "Loading...";
            OrganizeButton.IsEnabled = false;

            LoadFilesManager.SetAdditionalExtensions(ExtensionsTextBox.Text);

            await Task.Run(() =>
            {
                selectedDirectory = new DirectoryData(selectedFolderPath);
            }, token);

            if (selectedDirectory == null || loadedFilesTree == null) return;

            loadedFilesTree.BuildTree(selectedDirectory);

            loadingLabel.Content = selectedFolderPath;
            OrganizeButton.IsEnabled = true;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (settingsWindow != null)
            {
                settingsWindow.Close();
                settingsWindow = null;
            }
            else
            {
                settingsWindow = new SettingsWindow() { Owner = this };
                settingsWindow.Closed += (s, e) => { settingsWindow.Close(); settingsWindow = null; };
                settingsWindow.Show();
            }

        }

        private void OrganizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDirectory == null) return;
            MoveFilesButton.IsEnabled = false;

            try
            {
                string[] paths = PythonManager.OrganizeByDate(selectedDirectory.GetAllFiles());
                if (paths.Length == 0) return;

                organizedTree.BuildTree(paths);
                MoveFilesButton.IsEnabled = true;

                if (UserSettings.CopyOnRun.Value)
                {
                    MoveFiles();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FileTypesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ComboBox comboBox || comboBox.SelectedIndex == -1) return;

            LoadFilesManager.CurrentFileType = (FileType)comboBox.SelectedItem;
        }

        private void ExtensionsTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox? extensions = sender as TextBox;
            if (extensions == null) return;

            if (extensions.Text == "Additional extensions")
            {
                extensions.Text = string.Empty;
            }
        }

        private void ExtensionsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox? extensions = sender as TextBox;
            if (extensions == null) return;

            if (extensions.Text.Length == 0)
            {
                extensions.Text = "Additional extensions";
            }
        }

        private void MoveFiles()
        {
            MoveFilesButton.IsEnabled = false;

            string organizedFolder = string.Empty;
            try
            {
                organizedFolder = organizedTree.MoveFilesOnDisk(selectedDirectory.FullPath);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while organizing: " + ex.ToString());
                return;
            }

            MessageBox.Show("Organizing work is done!");
            //await LoadDirectory(organizedFolder, scanCancelTokenSource.Token);

            if (UserSettings.OpenOrganizedFolderAfterWork.Value)
            {
                if (Directory.Exists(organizedFolder))
                {
                    Process.Start("explorer.exe", organizedFolder);
                }
            }

            MoveFilesButton.IsEnabled = true;
        }

        private void MoveFilesButton_Click(object sender, RoutedEventArgs e)
        {
            MoveFiles();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (settingsWindow != null)
            {
                settingsWindow.Close();
                settingsWindow = null;
            }
        }
    }
}
