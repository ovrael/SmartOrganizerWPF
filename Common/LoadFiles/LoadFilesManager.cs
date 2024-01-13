using SmartOrganizerWPF.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SmartOrganizerWPF.Common.LoadFiles
{
    // Later -> let user create own file type with certain extensions f.g. icons: .ico, .png, .svg
    public enum FileType
    {
        All,
        Music,
        Image,
        Document
    }


    public static class LoadFilesManager
    {
        private static readonly Dictionary<FileType, string[]> fileTypeExtensions = new Dictionary<FileType, string[]>()
        {
            {
                FileType.Music,
                new string[]
                {
                    ".mp3", ".wav", ".wma", ".aac", ".m4a", ".flac", ".ogg", ".alac", ".mp4"
                }
            },
            {
                FileType.Image,
                new string[]
                {
                    ".jpeg", ".jpg", ".png", ".gif", ".bmp", ".tiff", ".raw", ".svg"
                }
            },
            {
                FileType.Document,
                new string[]
                {
                    ".doc", ".docx", ".pdf", ".txt", ".rtf",".md", ".log", ".xml"
                }
            }
        };

        public static FileType CurrentFileType { get; set; } = FileType.All;
        private static string[] additionalExtensions = Array.Empty<string>();

        internal static List<FileData> LoadFiles(string directoryFullPath)
        {
            List<FileData> files = new List<FileData>();
            if (!Directory.Exists(directoryFullPath))
            {
                return files;
            }

            string[] filePaths = Directory.GetFiles(directoryFullPath);

            foreach (var filePath in filePaths)
            {
                string fileExtension = Path.GetExtension(filePath);
                if (CurrentFileType == FileType.All
                    || fileTypeExtensions[CurrentFileType].Contains(fileExtension)
                    || additionalExtensions.Contains(fileExtension))
                    files.Add(new FileData(filePath));
            }

            return files;
        }

        internal static void SetAdditionalExtensions(string text)
        {
            if (text.Length == 0 || text == "Additional extensions")
            {
                additionalExtensions = Array.Empty<string>();
                return;
            }

            string[] extensions = text.Split(new char[] { '?', '|' });
            for (int i = 0; i < extensions.Length; i++)
            {
                if (extensions[i][0] != '.')
                    extensions[i] = '.' + extensions[i];
            }

            additionalExtensions = extensions;
        }
    }
}
