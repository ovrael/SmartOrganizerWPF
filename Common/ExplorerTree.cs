using SmartOrganizerWPF.Models;
using System.Windows;
using System.Windows.Controls;

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
                TreeViewItem directoryTreeItem = new TreeViewItem();
                directoryTreeItem.Header = directory.CreateTreeItemContent();
                directoryTreeItem.Tag = directory.DirectoryInfo.FullName;
                directoryTreeItem.FontWeight = FontWeights.Normal;

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
                    fileTreeItem.Tag = file.FileInfo.FullName;
                    fileTreeItem.FontWeight = FontWeights.Normal;

                    treeView.Items.Add(fileTreeItem);
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
