using Newtonsoft.Json;

using SmartOrganizerWPF.Models;

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace SmartOrganizerWPF.Common
{
    internal class ExplorerTree
    {
        private TreeView treeView;

        public ExplorerTree(TreeView treeView)
        {
            this.treeView = treeView;
        }

        public void BuildTree(DirectoryData mainDirectory)
        {
            treeView.Items.Clear();
            AddDirectoryTreeItem(mainDirectory);
        }

        private void AddDirectoryTreeItem(DirectoryData directoryData, TreeViewItem parent = null)
        {
            if (directoryData.Directories.Count == 0 && directoryData.Files.Count == 0)
            {
                return;
            }

            foreach (var directory in directoryData.Directories)
            {
                TreeViewItem directoryTreeItem = new TreeViewItem
                {
                    FontWeight = FontWeights.Normal,
                    Header = directory.CreateTreeItemContent(),
                    Tag = $"TreeItem_Directory_{directory.DirectoryInfo.Name}",
                };

                directoryTreeItem.Expanded += DirectoryTreeItem_Expanded;
                directoryTreeItem.Collapsed += DirectoryTreeItem_Collapsed;

                if (parent == null)
                {
                    treeView.Items.Add(directoryTreeItem);
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
                    fileTreeItem.Tag = $"TreeItem_File_{file.FileInfo.Name}";
                    fileTreeItem.FontWeight = FontWeights.Normal;

                    treeView.Items.Add(fileTreeItem);
                }
            }
        }

        private void DirectoryTreeItem_Collapsed(object sender, RoutedEventArgs e)
        {
            UpdateFolderIcon(sender, false);
            e.Handled = true;
        }

        private void DirectoryTreeItem_Expanded(object sender, RoutedEventArgs e)
        {
            UpdateFolderIcon(sender, true);
            e.Handled = true;
        }

        private void UpdateFolderIcon(object sender, bool isOpen)
        {
            if (sender is not TreeViewItem currentFolder) return;

            Debug.WriteLine("Update folder icon for: " + currentFolder.Tag + " is focused? " + currentFolder.IsFocused);

            if (currentFolder.Header is not StackPanel header) return;
            if (header.Children[1] is not Image folderImage) return;

            string fileName = isOpen ? "open" : "closed";

            Uri uri = new Uri(Tools.ResourcesPath + $"/Images/folder_icon_{fileName}.png");
            BitmapImage bitmap = new BitmapImage(uri);
            folderImage.Source = bitmap;
        }

        private void AddFileTreeItem(FileData fileData, TreeViewItem parent)
        {
            TreeViewItem fileTreeItem = new TreeViewItem();
            fileTreeItem.Header = fileData.CreateTreeItemContent();
            fileTreeItem.Tag = $"TreeItem_File_{fileData.FileInfo.Name}";
            fileTreeItem.FontWeight = FontWeights.Normal;

            parent.Items.Add(fileTreeItem);
        }
    }
}
