using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartOrganizerWPF.Common
{
    public class Tools
    {
        public static string ResourcesPath
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                if (assembly == null)
                    return string.Empty;

                string appFolderPath = Path.GetDirectoryName(assembly.Location);
                string sourceFolder = "Resources";

                return Path.Combine(appFolderPath, "../../../", sourceFolder);
            }
        }
    }
}
