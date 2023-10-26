using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.WindowsAPICodePack.Dialogs;

using Newtonsoft.Json;

namespace SmartOrganizerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SelectedFolderTextBox.Text = string.Empty;
            SelectedFolderTextBox.IsEnabled = false;

            //Directory.GetLogicalDrives();
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SelectedFolderTextBox.Text = dialog.FileName;
            }
        }

        private void SelectedFolderTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox? selectedFolderTextBox = sender as TextBox;
                if (selectedFolderTextBox == null) return;
                if (selectedFolderTextBox.Text == string.Empty) return;
                if (selectedFolderTextBox.Text.ToLower() == "selected folder") return;

                MessageBox.Show(selectedFolderTextBox.Text);
            }
            catch (Exception ex)
            {

            }
        }

        private DirectoryInfo[] ReadDirectories(string path, int depth = 0)
        {
            return Array.Empty<DirectoryInfo>();
        }
    }
}
