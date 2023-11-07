using SmartOrganizerWPF.Models;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SmartOrganizerWPF.Common
{
    internal class ExplorerTree
    {
        private readonly TreeView treeView;

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

            directoryData.IsLoaded = true;

            foreach (var directory in directoryData.Directories)
            {
                TreeViewItem directoryTreeItem = new TreeViewItem
                {
                    FontWeight = FontWeights.Normal,
                    Header = directory.CreateTreeItemContent(),
                    Tag = directory
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

                //AddDirectoryTreeItem(directory, directoryTreeItem);

                if (directory.Files.Count > 0)
                {
                    directoryTreeItem.Items.Add("placeholder");
                }

                //foreach (var file in directory.Files)
                //{
                //    AddFileTreeItem(file, directoryTreeItem);
                //}
            }

            if (parent == null)
            {
                foreach (var file in directoryData.Files)
                {
                    TreeViewItem fileTreeItem = new TreeViewItem();
                    fileTreeItem.Header = file.CreateTreeItemContent();
                    fileTreeItem.Tag = file;
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
            if (sender is not TreeViewItem currentFolder) return;

            if (currentFolder.Tag is DirectoryData directoryData)
            {
                if (!directoryData.IsLoaded)
                {
                    currentFolder.Items.Clear();
                    foreach (var directory in directoryData.Directories)
                    {
                        AddDirectoryTreeItem(directory, currentFolder);
                    }

                    foreach (var file in directoryData.Files)
                    {
                        AddFileTreeItem(file, currentFolder);
                    }
                    directoryData.IsLoaded = true;
                }
            }

            UpdateFolderIcon(sender, true);
            e.Handled = true;
        }

        private static void UpdateFolderIcon(object sender, bool isOpen)
        {
            if (sender is not TreeViewItem currentFolder) return;
            if (currentFolder.Header is not StackPanel header) return;
            if (header.Children[1] is not Image folderImage) return;

            string fileName = isOpen ? "open" : "closed";

            Uri uri = new Uri(Tools.ResourcesPath + $"/Images/folder_icon_{fileName}.png");
            BitmapImage bitmap = new BitmapImage(uri);
            folderImage.Source = bitmap;
        }

        private static void AddFileTreeItem(FileData fileData, TreeViewItem parent)
        {
            TreeViewItem fileTreeItem = new TreeViewItem();
            fileTreeItem.Header = fileData.CreateTreeItemContent();
            fileTreeItem.Tag = fileData;
            fileTreeItem.FontWeight = FontWeights.Normal;

            parent.Items.Add(fileTreeItem);
        }
    }
}
