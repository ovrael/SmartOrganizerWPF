using System;
using System.Collections.Generic;
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
            StackPanel content = new StackPanel() { Orientation = Orientation.Horizontal };

            // Get icon
            Image image = new Image() { Source = IconManager.FindIconForFilename(FileInfo.FullName), Width = 18, Height = 18 };

            content.Children.Add(image);

            Label label = new Label() { Content = FileInfo.Name };
            content.Children.Add(label);

            return content;
        }
    }
}
