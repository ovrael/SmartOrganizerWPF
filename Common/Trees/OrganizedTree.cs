using System.IO;
using System.Windows.Controls;

namespace SmartOrganizerWPF.Common.Trees
{
    public class OrganizedTree
    {
        private readonly TreeView treeView;

        public OrganizedTree(TreeView treeView)
        {
            this.treeView = treeView;
        }

        public void BuildTree(string[] organizedFiles)
        {
            for (int i = 0; i < organizedFiles.Length; i++)
            {
                string[] parts = organizedFiles[i].Split('?');
                if (parts.Length != 2) return;

                string filePath = parts[0];
                string organizedPath = parts[1];

                AddFileTreeItem(filePath, organizedPath, treeView.Items);
            }
        }

        private TreeViewItem RetrieveTreeItem(string directoryName, ItemCollection items)
        {
            TreeViewItem searchItem = null;
            foreach (var item in items)
            {
                if (item is not TreeViewItem treeItem) continue;
                if (treeItem.Header is not string header) continue;
                if (header == directoryName) return treeItem;
            }

            return searchItem;
        }

        private void AddFileTreeItem(string filePath, string organizedPath, ItemCollection items)
        {
            int slashIndex = organizedPath.IndexOf('/');
            if (slashIndex >= 0)
            {
                string currentPath = organizedPath[..slashIndex];
                string goDeeperPath = organizedPath[(slashIndex + 1)..];

                TreeViewItem currentDirectoryTreeItem = RetrieveTreeItem(currentPath, items);
                currentDirectoryTreeItem ??= AddDirectoryTreeItem(currentPath, items);

                AddFileTreeItem(filePath, goDeeperPath, currentDirectoryTreeItem.Items);

                return;
            }

            TreeViewItem directoryToAdd = RetrieveTreeItem(organizedPath, items);
            directoryToAdd ??= AddDirectoryTreeItem(organizedPath, items);

            TreeViewItem fileTreeItem = new TreeViewItem()
            {
                Header = Path.GetFileName(filePath),
                Tag = filePath
            };

            directoryToAdd.Items.Add(fileTreeItem);
        }

        private TreeViewItem AddDirectoryTreeItem(string name, ItemCollection collectionToAdd)
        {
            TreeViewItem directory = new TreeViewItem()
            {
                Header = name
            };

            collectionToAdd.Add(directory);

            return directory;
        }

        public void MoveFilesOnDisk(string mainDirectory)
        {

        }
    }
}
