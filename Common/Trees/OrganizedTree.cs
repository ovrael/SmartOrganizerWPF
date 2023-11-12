using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;


namespace SmartOrganizerWPF.Common.Trees
{
    public class OrganizedTree
    {
        private readonly TreeView treeView;
        private TreeViewItem? fromDragDirectory = null;

        public OrganizedTree(TreeView treeView)
        {
            this.treeView = treeView;
        }

        public void BuildTree(string[] organizedFiles)
        {
            treeView.Items.Clear();


            if (UserSettings.CreateOtherFolder.Value)
            {
                // Sort by organized paths to put 'Other' folder at the end
                try
                {
                    // Stupid way but quite works
                    organizedFiles = organizedFiles.OrderBy(f =>
                    {
                        string organizedPath = f.Split('?')[1];
                        organizedPath = organizedPath.Replace("Other", "ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ");

                        return organizedPath;
                    }).ToArray();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message.ToString());
                }
            }

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
                if (treeItem.Header is not StackPanel header) continue;
                if (header.Tag is not string headerTag) continue;
                if (headerTag == directoryName) return treeItem;
            }

            return searchItem;
        }

        private void AddFileTreeItem(string filePath, string organizedPath, ItemCollection items)
        {
            int slashIndex = organizedPath.IndexOf('/');
            if (slashIndex >= 0)
            {
                string currentDirectoryName = organizedPath[..slashIndex];
                string goDeeperPath = organizedPath[(slashIndex + 1)..];

                TreeViewItem currentDirectoryTreeItem = RetrieveTreeItem(currentDirectoryName, items);
                currentDirectoryTreeItem ??= AddDirectoryTreeItem(currentDirectoryName, items);

                AddFileTreeItem(filePath, goDeeperPath, currentDirectoryTreeItem.Items);

                return;
            }

            TreeViewItem directoryParent = RetrieveTreeItem(organizedPath, items);
            if (directoryParent == null)
            {
                if (organizedPath == "Other" && !UserSettings.CreateOtherFolder.Value)
                {
                    TreeViewItem fileTreeItem2 = CreateFileTreeItem(filePath);
                    items.Add(fileTreeItem2);
                    return;
                }
                else
                {
                    directoryParent = AddDirectoryTreeItem(organizedPath, items);
                }
            }


            TreeViewItem fileTreeItem = CreateFileTreeItem(filePath);
            directoryParent.Items.Add(fileTreeItem);
        }

        private TreeViewItem AddDirectoryTreeItem(string name, ItemCollection childrens)
        {
            TreeViewItem directory = CreateDirectoryTreeItem(name);

            childrens.Add(directory);

            return directory;
        }

        private TreeViewItem CreateDirectoryTreeItem(string directoryName)
        {
            StackPanel itemHeader = new StackPanel() { Orientation = Orientation.Horizontal, Tag = directoryName };

            // Folder icon
            Uri uri = new Uri(Tools.ResourcesPath + "/Images/folder_icon_closed.png");
            BitmapImage bitmap = new BitmapImage(uri);
            System.Windows.Controls.Image image = new System.Windows.Controls.Image()
            {
                Source = bitmap,
                Width = 18,
                Height = 18,
                Margin = new System.Windows.Thickness(5, 0, 0, 0)
            };
            itemHeader.Children.Add(image);

            // Direcotry name
            Label label = new Label() { Content = directoryName };
            itemHeader.Children.Add(label);


            TreeViewItem directory = new TreeViewItem() { Header = itemHeader, AllowDrop = true, Tag = $"{directoryName}_TreeItem" };
            directory.Drop += Directory_Drop;

            return directory;
        }

        private void Directory_Drop(object sender, DragEventArgs e)
        {
            if (sender is not TreeViewItem directoryItem) return;
            e.Handled = true;

            if (e.Data.GetData(DataFormats.StringFormat) is not string combinedData) return;

            string[] data = combinedData.Split('?');
            int.TryParse(data[0], out int index);

            TreeViewItem movedTreeItem = CreateFileTreeItem(data[1]);
            directoryItem.Items.Add(movedTreeItem);

            if (fromDragDirectory != null)
            {
                fromDragDirectory.Items.RemoveAt(index);
                fromDragDirectory = null;
            }
        }

        private TreeViewItem CreateFileTreeItem(string filePath)
        {
            StackPanel itemHeader = new StackPanel() { Orientation = Orientation.Horizontal, Tag = filePath };

            // File icon
            System.Windows.Controls.Image image = new System.Windows.Controls.Image()
            {
                Source = IconManager.FindIconForFilename(filePath),
                Width = 18,
                Height = 18,
                Margin = new Thickness(5, 0, 0, 0)
            };
            itemHeader.Children.Add(image);

            // File name
            Label label = new Label() { Content = Path.GetFileName(filePath) };
            itemHeader.Children.Add(label);

            TreeViewItem file = new TreeViewItem() { Header = itemHeader, Tag = $"{filePath}_TreeItem" };

            file.MouseMove += FileTreeItemDragStart_MouseMove;

            return file;
        }

        private void FileTreeItemDragStart_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not TreeViewItem fileTreeItem) return;
            if (!fileTreeItem.IsSelected) return;
            if (e.LeftButton != MouseButtonState.Pressed) return;

            if (fileTreeItem.Header is not StackPanel header) return;
            if (header.Tag is not string filePath) return;

            if (fileTreeItem.Parent is not TreeViewItem parent) return;
            fromDragDirectory = parent;
            int itemIndex = fromDragDirectory.Items.IndexOf(fileTreeItem);

            DragDrop.DoDragDrop(fileTreeItem, $"{itemIndex}?{filePath}", DragDropEffects.Copy);
        }

        public void MoveFilesOnDisk(string mainDirectory)
        {

        }
    }
}
