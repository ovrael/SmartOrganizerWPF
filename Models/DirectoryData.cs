using SmartOrganizerWPF.Common;
using SmartOrganizerWPF.Common.LoadFiles;
using SmartOrganizerWPF.Interfaces;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SmartOrganizerWPF.Models
{
    public class DirectoryData : IExplorerTreeItem
    {
        public DirectoryInfo? DirectoryInfo { get; private set; }

        public List<DirectoryData> Directories { get; private set; } = new List<DirectoryData>();
        public List<FileData> Files { get; private set; } = new List<FileData>();

        public List<string> GetAllFiles()
        {
            List<string> files = new List<string>();

            foreach (DirectoryData directory in Directories)
            {
                files.AddRange(directory.GetAllFiles());
            }

            foreach (FileData fileData in Files)
            {
                files.Add(fileData.FileInfo.FullName);
            }

            return files;
        }

        public DirectoryData(string path)
        {
            DirectoryInfo = new DirectoryInfo(path);

            if (DirectoryInfo == null) return;

            // Load directories
            if (UserSettings.DeepSearch.Value)
            {
                string[] directoryPaths = Array.Empty<string>();

                try
                {
                    directoryPaths = Directory.GetDirectories(DirectoryInfo.FullName);
                    foreach (var directoryPath in directoryPaths)
                    {
                        Directories.Add(new DirectoryData(directoryPath));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            // Load files
            Files = LoadFilesManager.LoadFiles(DirectoryInfo.FullName);
        }

        public StackPanel CreateTreeItemContent()
        {
            if (DirectoryInfo == null)
            {
                return new StackPanel();
            }

            StackPanel content = new StackPanel() { Orientation = Orientation.Horizontal, Tag = $"TreeItemHeader_{DirectoryInfo.Name}" };

            // Folder icon
            Uri uri = new Uri(Tools.ResourcesPath + "/Images/folder_icon.png");
            BitmapImage bitmap = new BitmapImage(uri);
            Image image = new Image() { Source = bitmap, Width = 18, Height = 18, Tag = $"TreeItemHeader_Image" };
            content.Children.Add(image);

            // Direcotry name
            Label label = new Label() { Content = DirectoryInfo.Name, Tag = $"TreeItemHeader_Label" };
            content.Children.Add(label);

            // Should be organized
            CheckBox shouldOrganizeCheckBox = new CheckBox() { IsChecked = null, IsThreeState = false, VerticalAlignment = System.Windows.VerticalAlignment.Center, Tag = $"TreeItemHeader_CheckBox" };
            shouldOrganizeCheckBox.Click += ShouldOrganizeCheckBox_Click;
            content.Children.Add(shouldOrganizeCheckBox);

            return content;
        }

        private void ShouldOrganizeCheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox == null) { return; }

            StackPanel treeItemHeader = checkBox.Parent as StackPanel;
            if (treeItemHeader == null) { return; }

            TreeViewItem treeItem = treeItemHeader.Parent as TreeViewItem;
            if (treeItem == null) { return; }

            ChangeChildrenStatus(treeItem, checkBox.IsChecked);
            ChangeParentStatus(treeItem);
        }

        private void ChangeParentStatus(TreeViewItem treeItem)
        {
            TreeViewItem parent = treeItem.Parent as TreeViewItem;
            if (parent == null) { return; }

            bool? newStatus = false;
            int uncheckedItems = 0;
            for (int i = 0; i < parent.Items.Count; i++)
            {
                if (parent.Items[i] is not TreeViewItem parentChild) continue;
                if (parentChild.Header is not StackPanel header) continue;
                if (header.Children[^1] is not CheckBox statusCheckBox) continue;

                if (statusCheckBox.IsChecked == null || statusCheckBox.IsChecked == false)
                {
                    uncheckedItems++;
                }
            }

            if (uncheckedItems == 0)
            {
                newStatus = true;
            }
            else if (uncheckedItems < parent.Items.Count)
            {
                newStatus = null;
            }
            else
            {
                newStatus = false;
            }

            if (parent.Header is not StackPanel parentHeader) return;
            if (parentHeader.Children[^1] is not CheckBox parentStatus) return;

            parentStatus.IsChecked = newStatus;
        }

        private void ChangeChildrenStatus(TreeViewItem? treeItem, bool? newStatus)
        {
            if (treeItem == null) return;

            for (int i = 0; i < treeItem.Items.Count; i++)
            {
                ChangeChildrenStatus(treeItem.Items[i] as TreeViewItem, newStatus);

                if (treeItem.Header is not StackPanel header) continue;
                if (header.Children[^1] is not CheckBox statusCheckBox) continue;

                statusCheckBox.IsChecked = newStatus;
            }
        }
    }
}
