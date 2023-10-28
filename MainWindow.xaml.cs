using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using Microsoft.WindowsAPICodePack.Dialogs;

using SmartOrganizerWPF.Models;

namespace SmartOrganizerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DirectoryData? selectedDirectory;
        private int oldFolderIndex = -1;

        public MainWindow()
        {
            InitializeComponent();

            // ADD ALSO USER DEFINED DIRECTORIES
            SelectFolderComboBox.Text = "Select folder";
            SelectFolderComboBox.VerticalContentAlignment = VerticalAlignment.Center;
            SelectFolderComboBox.HorizontalAlignment = HorizontalAlignment.Left;
            SelectFolderComboBox.Items.Add("Select folder from explorer...");
            SelectFolderComboBox.Items.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            SelectFolderComboBox.Items.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            SelectFolderComboBox.Items.Add(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            SelectFolderComboBox.Items.Add($"C:\\Users\\{Environment.UserName}\\Downloads");
        }

        private void SelectFolderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null || comboBox.SelectedItem == null) return;

            string folderName = comboBox.SelectedItem.ToString();
            if (folderName == null || folderName.Length == 0) return;

            // Selected: choose other directory
            if (comboBox.SelectedIndex == 0)
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                SelectFolderComboBox.Items.Add(dialog.FileName);
                SelectFolderComboBox.SelectedIndex = SelectFolderComboBox.Items.Count - 1;
                folderName = dialog.FileName;
            }

            try
            {
                LoadDirectory(folderName);
                oldFolderIndex = comboBox.SelectedIndex;
            }
            catch (Exception ex)
            {
                //(SelectFolderComboBox.Items[comboBox.SelectedIndex] as ComboBoxItem).IsEnabled = false;
                SelectFolderComboBox.SelectedIndex = oldFolderIndex;

                MessageBox.Show(ex.ToString());
            }
        }

        private void LoadDirectory(string selectedFolderPath)
        {
            if (selectedFolderPath == null) return;
            if (selectedFolderPath == string.Empty) return;
            if (selectedFolderPath.ToLower() == "selected folder") return;
            loadingLabel.Content = "Loading ...";

            selectedDirectory = new DirectoryData(selectedFolderPath);

            ExplorerTreeView.Items.Clear();

            AddDirectoryTreeItem(selectedDirectory);

            loadingLabel.Content = selectedFolderPath;
        }

        private void AddDirectoryTreeItem(DirectoryData directoryData, TreeViewItem parent = null)
        {
            if (directoryData.Directories.Count == 0) return;

            foreach (var directory in directoryData.Directories)
            {
                TreeViewItem directoryTreeItem = new TreeViewItem();
                directoryTreeItem.Header = directory.CreateTreeItemContent();
                directoryTreeItem.Tag = directory.DirectoryInfo.FullName;
                directoryTreeItem.FontWeight = FontWeights.Normal;


                if (parent == null)
                {
                    ExplorerTreeView.Items.Add(directoryTreeItem);
                }
                else
                {
                    parent.Items.Add(directoryTreeItem);
                }

                AddDirectoryTreeItem(directory, directoryTreeItem);

                foreach (var file in directory.Files)
                {
                    AddFileTreeItem(file, directoryTreeItem);
                }
            }

            if (parent == null)
            {
                foreach (var file in directoryData.Files)
                {
                    TreeViewItem fileTreeItem = new TreeViewItem();
                    fileTreeItem.Header = file.CreateTreeItemContent();
                    fileTreeItem.Tag = file.FileInfo.FullName;
                    fileTreeItem.FontWeight = FontWeights.Normal;

                    ExplorerTreeView.Items.Add(fileTreeItem);
                }
            }
        }

        private void AddFileTreeItem(FileData fileData, TreeViewItem parent)
        {
            TreeViewItem fileTreeItem = new TreeViewItem();
            fileTreeItem.Header = fileData.CreateTreeItemContent();
            fileTreeItem.Tag = fileData.FileInfo.FullName;
            fileTreeItem.FontWeight = FontWeights.Normal;

            parent.Items.Add(fileTreeItem);
        }
    }
}
