using SmartOrganizerWPF.Common;
using SmartOrganizerWPF.Interfaces;

using System.IO;
using System.Windows.Controls;

namespace SmartOrganizerWPF.Models
{
    public class FileData : IExplorerTreeItem
    {
        public bool? IsChecked { get; set; } = true;
        public FileInfo FileInfo { get; private set; }

        public string FullPath => FileInfo.FullName;


        public FileData() { }
        public FileData(string path)
        {
            FileInfo = new FileInfo(path);
        }

        public StackPanel CreateTreeItemContent()
        {
            StackPanel content = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Tag = $"TreeItem_StackPanel_File_{FileInfo.Name}",
                Opacity = (IsChecked == null || IsChecked == true) ? 1 : 0.5
            };

            // Should organize checkbox
            CheckBox shouldOrganizeCheckBox = new CheckBox()
            {
                IsChecked = this.IsChecked,
                IsThreeState = false,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Tag = $"TreeItem_CheckBox_File_{FileInfo.Name}"
            };
            shouldOrganizeCheckBox.Click += ShouldOrganizeCheckBox_Click;
            content.Children.Add(shouldOrganizeCheckBox);

            // File icon
            Image image = new Image()
            {
                Source = IconManager.FindIconForFilename(FileInfo.FullName),
                Width = 18,
                Height = 18,
                Margin = new System.Windows.Thickness(5, 0, 0, 0),
                Tag = $"TreeItem_Image_File_{FileInfo.Name}"
            };
            content.Children.Add(image);

            // File name
            Label label = new Label() { Content = FileInfo.Name, Tag = $"TreeItem_Label_File_{FileInfo.Name}" };
            content.Children.Add(label);


            return content;
        }

        private void ShouldOrganizeCheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is not CheckBox checkBox) return;
            if (checkBox.Parent is not StackPanel treeItemHeader) return;
            if (treeItemHeader.Parent is not TreeViewItem treeItem) return;

            if (treeItem.Tag is FileData fileData)
            {
                fileData.IsChecked = checkBox.IsChecked.GetValueOrDefault();
                treeItemHeader.Opacity = (fileData.IsChecked == null || fileData.IsChecked == true) ? 1 : 0.5;
            }

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
    }
}
