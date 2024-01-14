using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
//using System.Windows.Shapes;

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
                    TreeViewItem unorganizedFile = CreateFileTreeItem(filePath);
                    items.Add(unorganizedFile);
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


            TreeViewItem directory = new TreeViewItem() { Header = itemHeader, AllowDrop = true, Tag = $"{directoryName}" };
            directory.Drop += Directory_Drop;
            directory.DragEnter += Directory_DragEnter;

            return directory;
        }

        private void Directory_DragEnter(object sender, DragEventArgs e)
        {
            e.Handled = true;
            if (sender is not TreeViewItem treeItem) return;
            treeItem.IsSelected = true;
            treeItem.IsExpanded = true;
        }

        private void Directory_Drop(object sender, DragEventArgs e)
        {
            if (sender is not TreeViewItem directoryItem) return;
            e.Handled = true;
            if (directoryItem.Equals(fromDragDirectory)) return;

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

            TreeViewItem file = new TreeViewItem() { Header = itemHeader, Tag = $"File", ToolTip = filePath };

            file.MouseMove += FileTreeItemDragStart_MouseMove;
            file.MouseDoubleClick += File_MouseDoubleClick;

            return file;
        }

        private void File_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TreeViewItem treeItem) return;
            if (treeItem.ToolTip is not string filePath) return;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Process.Start("explorer.exe", filePath);
            };
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

        public string MoveFilesOnDisk(string mainDirectory)
        {
            if (!Directory.Exists(mainDirectory))
            {
                MessageBox.Show("Cannot find selected directory for saving organized files");
                return string.Empty;
            };

            if (UserSettings.CreateOrganizedFolder.Value)
            {
                mainDirectory = Path.Combine(mainDirectory, "organized");
                Directory.CreateDirectory(mainDirectory);
            }

            OrganizeDirectories(mainDirectory, treeView.Items);

            return mainDirectory;
        }

        private void OrganizeDirectories(string directoryPath, ItemCollection items)
        {
            foreach (var item in items)
            {
                if (item is not TreeViewItem treeItem) continue;
                if (treeItem.Tag is not string itemTag) continue;
                if (itemTag == "File")
                {
                    if (treeItem.ToolTip is not string filePath) continue;
                    MoveFile(directoryPath, filePath);
                }
                else
                {
                    string deeperFolder = Path.Combine(directoryPath, itemTag);
                    CreateFolderOnDisk(deeperFolder);
                    OrganizeDirectories(deeperFolder, treeItem.Items);
                }
            }
        }

        private void CreateFolderOnDisk(string deeperFolder)
        {
            if (Directory.Exists(deeperFolder)) return;
            Directory.CreateDirectory(deeperFolder);
        }

        private static void MoveFile(string path, string filePath)
        {
            if (!File.Exists(filePath)) return;

            try
            {
                if (UserSettings.DeleteMovedFiles.Value)
                {
                    File.Move(filePath, Path.Combine(path, Path.GetFileName(filePath)), UserSettings.OverwriteMoved.Value);
                }
                else
                {
                    File.Copy(filePath, Path.Combine(path, Path.GetFileName(filePath)), UserSettings.OverwriteMoved.Value);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error when moving file {filePath} to {path}: " + e.ToString());
            }
        }
    }
}
