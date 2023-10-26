using System;
using System.Collections.Generic;
using System.DirectoryServices;
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

using SmartOrganizerWPF.Models;

namespace SmartOrganizerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DirectoryData? selectedDirectory;
        private int explorerTextBlockOffset = 10;
        private int explorerTextBlockHeight = 20;
        private int explorerTextBlockDepthOffset = 15;

        public MainWindow()
        {
            InitializeComponent();
            SelectedFolderTextBlock.Text = string.Empty;
            //Directory.GetLogicalDrives();
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            SelectedFolderTextBlock.Text = dialog.FileName;
            LoadDirectory(dialog.FileName);
        }

        private void LoadDirectory(string selectedFolderPath)
        {
            try
            {
                if (selectedFolderPath == null) return;
                if (selectedFolderPath == string.Empty) return;
                if (selectedFolderPath.ToLower() == "selected folder") return;
                loadingLabel.Content = "Loading ...";

                selectedDirectory = new DirectoryData(selectedFolderPath, 0);

                ExplorerGrid.Children.Clear();

                AddDirectoryElement(selectedDirectory);

                loadingLabel.Content = selectedFolderPath;

            }
            catch (Exception ex)
            {

            }
        }

        private void AddDirectoryElement(DirectoryData directoryData)
        {
            foreach (var directory in directoryData.Directories)
            {
                TextBlock directoryTextBlock = new TextBlock();
                directoryTextBlock.Text = "> " + directory.DirectoryInfo.Name;
                directoryTextBlock.FontSize = 12;
                directoryTextBlock.Height = explorerTextBlockHeight;
                directoryTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                directoryTextBlock.VerticalAlignment = VerticalAlignment.Top;
                directoryTextBlock.Margin = new Thickness(explorerTextBlockOffset + directory.Depth * explorerTextBlockDepthOffset, explorerTextBlockOffset + ExplorerGrid.Children.Count * explorerTextBlockHeight, 0, 0);

                ExplorerGrid.Children.Add(directoryTextBlock);

                AddDirectoryElement(directory);

                foreach (var file in directory.Files)
                {
                    AddFileElement(file);
                }
            }
        }

        private void AddFileElement(FileData fileData)
        {
            TextBlock fileTextBlock = new TextBlock();
            fileTextBlock.Text = "+ " + fileData.FileInfo.Name;
            fileTextBlock.FontSize = 12;
            fileTextBlock.Height = explorerTextBlockHeight;
            fileTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            fileTextBlock.VerticalAlignment = VerticalAlignment.Top;
            fileTextBlock.Margin = new Thickness(explorerTextBlockOffset + fileData.Depth * explorerTextBlockDepthOffset, explorerTextBlockOffset + ExplorerGrid.Children.Count * explorerTextBlockHeight, 0, 0);

            ExplorerGrid.Children.Add(fileTextBlock);
        }
    }
}
