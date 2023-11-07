using SmartOrganizerWPF.Common;
using SmartOrganizerWPF.Common.LoadFiles;
using SmartOrganizerWPF.Interfaces;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SmartOrganizerWPF.Models
{
    public class DirectoryData : IExplorerTreeItem
    {
        public bool IsLoaded { get; set; } = false;
        public bool? IsChecked { get; set; } = true;
        public string FullPath => DirectoryInfo.FullName;

        public DirectoryInfo? DirectoryInfo { get; private set; }
        public List<DirectoryData> Directories { get; private set; } = new List<DirectoryData>();
        public List<FileData> Files { get; private set; } = new List<FileData>();

        public DirectoryData(string path)
        {
            DirectoryInfo = new DirectoryInfo(path);

            if (DirectoryInfo == null) return;

            // Load directories
            if (UserSettings.DeepSearch.Value)
            {
                try
                {
                    string[] directoryPaths = Directory.GetDirectories(DirectoryInfo.FullName);
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

        public List<string> GetAllFiles()
        {
            List<string> files = new List<string>();

            foreach (DirectoryData directory in Directories)
            {
                files.AddRange(directory.GetAllFiles());
            }

            foreach (FileData fileData in Files)
            {
                if (fileData != null && fileData.IsChecked == true)
                    files.Add(fileData.FileInfo.FullName);
            }

            return files;
        }

        public StackPanel CreateTreeItemContent()
        {
            if (DirectoryInfo == null)
            {
                return new StackPanel();
            }

            StackPanel content = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Tag = $"TreeItem_StackPanel_Directory_{DirectoryInfo.Name}",
                Opacity = (IsChecked == null || IsChecked == true) ? 1 : 0.5
            };

            // Should be organized
            CheckBox shouldOrganizeCheckBox = new CheckBox()
            {
                IsChecked = this.IsChecked,
                IsThreeState = false,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Tag = $"TreeItem_CheckBox_Directory_{DirectoryInfo.Name}"
            };
            shouldOrganizeCheckBox.Click += ShouldOrganizeCheckBox_Click;
            content.Children.Add(shouldOrganizeCheckBox);

            // Folder icon
            Uri uri = new Uri(Tools.ResourcesPath + "/Images/folder_icon_closed.png");
            BitmapImage bitmap = new BitmapImage(uri);
            Image image = new Image()
            {
                Source = bitmap,
                Width = 18,
                Height = 18,
                Margin = new System.Windows.Thickness(5, 0, 0, 0),
                Tag = $"TreeItem_Image_Directory_{DirectoryInfo.Name}"
            };
            content.Children.Add(image);

            // Direcotry name
            Label label = new Label() { Content = DirectoryInfo.Name, Tag = $"TreeItem_Label_Directory_{DirectoryInfo.Name}" };
            content.Children.Add(label);


            return content;
        }

        private void ShouldOrganizeCheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is not CheckBox checkBox) { return; }
            if (checkBox.Parent is not StackPanel treeItemHeader) { return; }
            if (treeItemHeader.Parent is not TreeViewItem treeItem) { return; }

            if (checkBox.IsChecked == false)
            {
                treeItemHeader.Opacity = 0.5;
            }
            else
            {
                treeItemHeader.Opacity = 1.0;
            }

            ChangeChildrenStatus(treeItem, checkBox.IsChecked);
            ChangeParentStatus(treeItem);
        }

        public void ChangeParentStatus(TreeViewItem treeItem)
        {
            if (treeItem.Parent is not TreeViewItem parent) return;

            bool? newStatus = false;
            int uncheckedItems = 0;
            for (int i = 0; i < parent.Items.Count; i++)
            {
                if (parent.Items[i] is not TreeViewItem parentChild) continue;
                if (parentChild.Header is not StackPanel header) continue;
                if (header.Children[0] is not CheckBox statusCheckBox) continue;

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
            if (parentHeader.Children[0] is not CheckBox parentStatus) return;

            parentStatus.IsChecked = newStatus;
            if (parentStatus.IsChecked == false)
            {
                parentHeader.Opacity = 0.5;
            }
            else
            {
                parentHeader.Opacity = 1.0;
            }

            ChangeParentStatus(parent);
        }

        private void ChangeUnloadedStatus(bool? newStatus)
        {
            IsChecked = newStatus;
            foreach (var childDirectory in Directories)
            {
                childDirectory.ChangeUnloadedStatus(newStatus);
            }

            foreach (var file in Files)
            {
                file.IsChecked = newStatus.GetValueOrDefault();
            }
        }

        public void ChangeChildrenStatus(TreeViewItem? treeItem, bool? newStatus)
        {
            if (treeItem == null) return;
            if (treeItem.Tag is not DirectoryData directory) return;


            if (!directory.IsLoaded)
            {
                directory.ChangeUnloadedStatus(newStatus);
            }
            else
            {
                for (int i = 0; i < treeItem.Items.Count; i++)
                {
                    if (treeItem.Items[i] is not TreeViewItem child) continue;

                    ChangeChildrenStatus(child, newStatus);

                    if (child.Header is not StackPanel header) continue;

                    if (newStatus == false)
                    {
                        header.Opacity = 0.5;
                    }
                    else
                    {
                        header.Opacity = 1.0;
                    }

                    if (child.Tag is FileData fileData)
                    {
                        fileData.IsChecked = newStatus.GetValueOrDefault();
                    }


                    if (header.Children[0] is not CheckBox statusCheckBox) continue;

                    statusCheckBox.IsChecked = newStatus;
                }
            }
        }
    }
}
