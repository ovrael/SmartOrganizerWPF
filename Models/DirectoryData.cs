using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartOrganizerWPF.Models
{
    public class DirectoryData
    {
        public DirectoryInfo? DirectoryInfo { get; set; }
        public int Depth { get; set; } = 0;

        public List<DirectoryData> Directories { get; set; } = new List<DirectoryData>();
        public List<FileData> Files { get; set; } = new List<FileData>();

        public DirectoryData(string path, int depth = 0)
        {
            DirectoryInfo = new DirectoryInfo(path);
            Depth = depth;

            if (DirectoryInfo == null) return;

            string[] directoryPaths = Directory.GetDirectories(DirectoryInfo.FullName);
            foreach (var directoryPath in directoryPaths)
            {
                Directories.Add(
                    new DirectoryData(directoryPath, depth + 1)
                    );
            }

            string[] filePaths = Directory.GetFiles(DirectoryInfo.FullName);

            foreach (var filePath in filePaths)
            {
                Files.Add(
                    new FileData(filePath, depth + 1)
                    );
            }
        }
    }
}
