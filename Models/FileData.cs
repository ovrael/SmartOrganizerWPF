using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartOrganizerWPF.Models
{
    public class FileData
    {
        public FileInfo FileInfo { get; set; }
        public int Depth { get; set; } = 0;

        public FileData(string path, int depth)
        {
            FileInfo = new FileInfo(path);
            Depth = depth;
        }
    }
}
