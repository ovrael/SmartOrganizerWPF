using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace SmartOrganizerWPF.Common
{
    public static class PythonManager
    {
        private static readonly string pythonExecPath = string.Empty;

        private static readonly string organizeByDateScriptPath = string.Empty;

        private static class Features
        {
            public static readonly string OrganizeByDate = "organizeByDate.py";
        }


        static PythonManager()
        {
            pythonExecPath = GetPythonPath();
            organizeByDateScriptPath = GetScriptPath(Features.OrganizeByDate);
        }

        private static string GetPythonPath()
        {
            string appDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string programsDirectory = Path.Combine(appDataDirectory, "Programs");
            if (!Directory.Exists(programsDirectory)) return string.Empty;

            string pythonDirectory = Path.Combine(programsDirectory, "Python");
            if (!Directory.Exists(pythonDirectory)) return string.Empty;

            string[] pythonVersions = Directory.GetDirectories(pythonDirectory);
            if (pythonVersions.Length == 0) return string.Empty;


            var orderedVersions = pythonVersions.OrderDescending();

            foreach (var version in orderedVersions)
            {
                string execPath = Path.Combine(version, "python.exe");
                if (File.Exists(execPath))
                {
                    return execPath;
                }
            }

            return string.Empty;
        }

        private static string GetScriptPath(string scriptName)
        {
            DirectoryInfo currentDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent;

            string pythonScriptsPath = Path.Combine(currentDirectory.FullName, "PythonScripts");
            if (!Directory.Exists(pythonScriptsPath)) return string.Empty;

            string scriptPath = Path.Combine(pythonScriptsPath, scriptName);
            if (!File.Exists(scriptPath)) return string.Empty;

            return scriptPath;
        }

        public static string[] OrganizeByDate(List<string> files)
        {
            if (!File.Exists(organizeByDateScriptPath))
            {
                MessageBox.Show("Oops... Something went wrong. Cannot localize script for organizing pictures");
                return Array.Empty<string>();
            }

            string randomTempPath = Path.GetTempFileName();
            File.WriteAllLines(randomTempPath, files, Encoding.UTF8);

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = pythonExecPath;
            start.Arguments = $"\"{organizeByDateScriptPath}\" {randomTempPath}";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.CreateNoWindow = true;

            string resultFilePath = string.Empty;

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script

                    if (stderr != null && stderr.Length > 0)
                    {
                        Debug.WriteLine("ERROR FROM SCRIPT: " + stderr);
                        MessageBox.Show("Python script error: " + stderr);
                        ClearTempFiles(randomTempPath, resultFilePath);
                        return Array.Empty<string>();
                    }

                    resultFilePath = reader.ReadToEnd().Trim();
                }

            }


            if (File.Exists(resultFilePath))
            {
                string[] result = File.ReadAllLines(resultFilePath);
                ClearTempFiles(randomTempPath, resultFilePath);
                return result;
            }

            // Delete temporary files
            ClearTempFiles(randomTempPath, resultFilePath);
            return Array.Empty<string>();
        }

        private static void ClearTempFiles(params string[] tempFiles)
        {
            foreach (var tempFile in tempFiles)
            {
                // Delete temporary files
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        internal static void PrintData()
        {
            MessageBox.Show($"pythonPath: {pythonExecPath}");
            MessageBox.Show($"organizePicturesScriptPath: {organizeByDateScriptPath}");
        }
    }
}
