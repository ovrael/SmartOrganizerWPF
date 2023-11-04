using SmartOrganizerWPF.Common;
using SmartOrganizerWPF.Common.LoadFiles;
using SmartOrganizerWPF.Interfaces;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SmartOrganizerWPF.Models
{
    public class DirectoryData : IExplorerTreeItem
    {
        public DirectoryInfo? DirectoryInfo { get; private set; }

        public List<DirectoryData> Directories { get; private set; } = new List<DirectoryData>();
        public List<FileData> Files { get; private set; } = new List<FileData>();

        public List<string> GetAllFiles()
        {
            List<string> files = new List<string>();

            foreach (DirectoryData directory in Directories)
            {
                files.AddRange(directory.GetAllFiles());
            }

            foreach (FileData fileData in Files)
            {
                files.Add(fileData.FileInfo.FullName);
            }

            return files;
        }

        public DirectoryData(string path)
        {
            DirectoryInfo = new DirectoryInfo(path);

            if (DirectoryInfo == null) return;

            // Load directories
            if (UserSettings.DeepSearch.Value)
            {
                string[] directoryPaths = Array.Empty<string>();

                try
                {
                    directoryPaths = Directory.GetDirectories(DirectoryInfo.FullName);
                    foreach (var directoryPath in directoryPaths)
                    {
                        Directories.Add(new DirectoryData(directoryPath));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            // Load files
            Files = LoadFilesManager.LoadFiles(DirectoryInfo.FullName);
        }

        public StackPanel CreateTreeItemContent()
        {
            StackPanel content = new StackPanel() { Orientation = Orientation.Horizontal };

            // Get icon
            Uri uri = new Uri(Tools.ResourcesPath + "/Images/folder_icon.png");
            BitmapImage bitmap = new BitmapImage(uri);
            Image image = new Image() { Source = bitmap, Width = 18, Height = 18 };
            content.Children.Add(image);


            Label label = new Label() { Content = DirectoryInfo.Name };
            content.Children.Add(label);

            return content;
        }
    }
}
