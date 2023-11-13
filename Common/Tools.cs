using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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


        public static System.Windows.Media.SolidColorBrush CreateBrush(byte red = 0, byte green = 0, byte blue = 0, byte alpha = 255)
        {
            return new System.Windows.Media.SolidColorBrush(
                new System.Windows.Media.Color()
                {
                    R = red,
                    G = green,
                    B = blue,
                    A = alpha
                });
        }

        public static System.Windows.Media.SolidColorBrush CreateBrush(string hexColor)
        {
            hexColor = hexColor.Trim('#').ToLower();

            if (hexColor.Length != 6 && hexColor.Length != 8) return CreateBrush();

            byte alpha = 255;
            if (hexColor.Length == 8)
            {
                alpha = byte.Parse(hexColor[0..2], System.Globalization.NumberStyles.HexNumber);
                hexColor = hexColor[2..8];
            }

            byte red = byte.Parse(hexColor[0..2], System.Globalization.NumberStyles.HexNumber);
            byte green = byte.Parse(hexColor[2..4], System.Globalization.NumberStyles.HexNumber);
            byte blue = byte.Parse(hexColor[4..6], System.Globalization.NumberStyles.HexNumber);

            return CreateBrush(red, green, blue, alpha);
        }
    }
}
