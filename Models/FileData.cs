using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using SmartOrganizerWPF.Common;
using SmartOrganizerWPF.Interfaces;

namespace SmartOrganizerWPF.Models
{
    public class FileData : IExplorerTreeItem
    {
        public FileInfo FileInfo { get; private set; }

        public FileData(string path)
        {
            FileInfo = new FileInfo(path);
        }

        public StackPanel CreateTreeItemContent()
        {
            StackPanel content = new StackPanel() { Orientation = Orientation.Horizontal, Tag = $"TreeItem_StackPanel_File_{FileInfo.Name}" };

            // Should organize checkbox
            CheckBox shouldOrganizeCheckBox = new CheckBox()
            {
                IsChecked = true,
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

            ChangeParentStatus(treeItem);
        }

        private void ChangeParentStatus(TreeViewItem treeItem)
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
        }
    }
}
