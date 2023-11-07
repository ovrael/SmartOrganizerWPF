using SmartOrganizerWPF.Interfaces;
using SmartOrganizerWPF.Models;

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SmartOrganizerWPF.Common
{
    internal class ExplorerTree
    {
        private readonly TreeView treeView;
        private string MainDirectoryName { get; set; }

        public ExplorerTree(TreeView treeView)
        {
            this.treeView = treeView;
        }

        public void BuildTree(DirectoryData mainDirectory)
        {
            treeView.Items.Clear();
            MainDirectoryName = mainDirectory.FullPath;
            AddDirectoryTreeItem(mainDirectory);

            treeView.MouseRightButtonDown += TreeView_MouseRightButtonDown;
        }

        private void TreeView_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is not TreeView treeView) return;

            treeView.ContextMenu = new ContextMenu();

            bool allStatus = GetCurrentAllStatus();
            MenuItem changeStatusAllMenuItem = new MenuItem()
            {
                Header = allStatus ? "Unselect all files from organize" : "Select all files to organize",
                Tag = allStatus
            };
            changeStatusAllMenuItem.Click += ChangeStatusAllMenuItem_Click;
            treeView.ContextMenu.Items.Add(changeStatusAllMenuItem);
        }

        private bool GetCurrentAllStatus()
        {
            foreach (var item in treeView.Items)
            {
                if (item is not TreeViewItem treeItem) continue;
                if (treeItem.Header is not StackPanel header) continue;
                if (header.Children[0] is not CheckBox checkBox) continue;
                if (checkBox.IsChecked == null || checkBox.IsChecked == true) return true;
            }

            return false;
        }

        private void ChangeStatusAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem) return;
            if (menuItem.Tag is not bool allStatus) return;
            menuItem.Header = allStatus ? "Unselect all files from organize" : "Select all files to organize";

            foreach (var item in treeView.Items)
            {
                if (item is not TreeViewItem treeItem) continue;

                if (treeItem.Tag is DirectoryData directory)
                {
                    directory.ChangeChildrenStatus(treeItem, !allStatus);
                }
                else if (treeItem.Tag is FileData file)
                {
                    file.IsChecked = !allStatus;
                }

                if (treeItem.Header is not StackPanel header) return;
                if (header.Children[0] is not CheckBox checkBox) return;

                checkBox.IsChecked = !allStatus;
                header.Opacity = !allStatus ? 1 : 0.5;
            }
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
                    Header = directory.CreateTreeItemContent(),
                    Tag = directory
                };

                directoryTreeItem.Expanded += DirectoryTreeItem_Expanded;
                directoryTreeItem.Collapsed += DirectoryTreeItem_Collapsed;
                directoryTreeItem.MouseRightButtonDown += ExplorerTreeItem_MouseRightButtonDown;

                if (parent == null)
                {
                    treeView.Items.Add(directoryTreeItem);
                }
                else
                {
                    parent.Items.Add(directoryTreeItem);
                }

                //AddDirectoryTreeItem(directory, directoryTreeItem);

                if (directory.Files.Count > 0 || directory.Directories.Count > 0)
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
                    treeView.Items.Add(CreateFileTreeItem(file));
                }
            }
        }

        private object CreateFileTreeItem(FileData file)
        {
            TreeViewItem fileTreeItem = new TreeViewItem();
            fileTreeItem.Header = file.CreateTreeItemContent();
            fileTreeItem.Tag = file;
            fileTreeItem.MouseRightButtonDown += ExplorerTreeItem_MouseRightButtonDown;

            return fileTreeItem;
        }

        private ContextMenu CreateContextMenu<T>(T explorerTreeItem, TreeViewItem treeItem) where T : IExplorerTreeItem
        {
            ContextMenu menu = new ContextMenu();

            MenuItem menuHeader = new MenuItem()
            {
                Header = explorerTreeItem.FullPath,
                IsEnabled = false,
                IsCheckable = false,
                IsHitTestVisible = false,
                IsManipulationEnabled = false,
            };

            MenuItem openMenuItem = new MenuItem()
            {
                Header = "Open in explorer",
                IsCheckable = false,
                Tag = explorerTreeItem.FullPath
            };
            openMenuItem.Click += OpenMenuItem_Click;

            MenuItem changeStatusMenuItem = new MenuItem()
            {
                Header = (explorerTreeItem.IsChecked == null || explorerTreeItem.IsChecked == true) ? "Select to organize" : "Unselect from organize",
                IsCheckable = false,
                Tag = new object[] { explorerTreeItem, treeItem }
            };
            changeStatusMenuItem.Click += ChangeStatusMenuItem_Click;


            menu.Items.Add(menuHeader);
            menu.Items.Add(openMenuItem);
            menu.Items.Add(changeStatusMenuItem);

            return menu;
        }

        private void ChangeStatusMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem) return;
            if (menuItem.Tag is not object[] objects) return;
            if (objects[0] is not IExplorerTreeItem explorerTreeItem) return;
            if (objects[1] is not TreeViewItem treeItem) return;

            explorerTreeItem.IsChecked = !(explorerTreeItem.IsChecked == null || explorerTreeItem.IsChecked == true);
            menuItem.Header = (explorerTreeItem.IsChecked == null || explorerTreeItem.IsChecked == true) ? "Remove from organize" : "Add to organize";

            if (explorerTreeItem is DirectoryData directory)
            {
                directory.ChangeChildrenStatus(treeItem, explorerTreeItem.IsChecked);
                directory.ChangeParentStatus(treeItem);
            }
            else if (explorerTreeItem is FileData file)
            {
                file.ChangeParentStatus(treeItem);
            }

            if (treeItem.Header is not StackPanel header) return;
            if (header.Children[0] is not CheckBox checkBox) return;

            checkBox.IsChecked = explorerTreeItem.IsChecked;
            header.Opacity = (explorerTreeItem.IsChecked == null || explorerTreeItem.IsChecked == true) ? 1 : 0.5;
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem) return;
            if (menuItem.Tag is not string directory) return;

            Process.Start("explorer.exe", directory);
        }

        private void ExplorerTreeItem_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is not TreeViewItem treeItem) return;


            e.Handled = true;

            if (treeItem.Tag is IExplorerTreeItem explorerTreeItem)
            {
                treeItem.IsSelected = true;
                treeItem.ContextMenu = CreateContextMenu(explorerTreeItem, treeItem);
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
                    AddDirectoryTreeItem(directoryData, currentFolder);

                    foreach (var file in directoryData.Files)
                    {
                        currentFolder.Items.Add(CreateFileTreeItem(file));
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
    }
}
