using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Newtonsoft.Json;

namespace SmartOrganizerWPF.Common
{
    public static class PythonManager
    {
        private static string pythonExecPath = string.Empty;

        private static string organizePicturesScriptPath = string.Empty;

        private static class Features
        {
            public static readonly string OrganizePictures = "test.py";
        }


        static PythonManager()
        {
            pythonExecPath = GetPythonPath();
            organizePicturesScriptPath = GetScriptPath(Features.OrganizePictures);
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

        public static void OrganizePictures(List<string> files)
        {
            string randomTempPath = Path.GetTempFileName();
            File.WriteAllLines(randomTempPath, files);

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = pythonExecPath;
            start.Arguments = string.Format("{0} {1}", organizePicturesScriptPath, randomTempPath);
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
                        return;
                    }

                    resultFilePath = reader.ReadToEnd().Trim();

                    //Debug.WriteLine("Path to file from Python script: " + resultFilePath);
                }
            }


            if (File.Exists(resultFilePath))
            {
                DoWork(resultFilePath);
            }

            // Delete temporary files
            ClearTempFiles(randomTempPath, resultFilePath);
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

        private static void DoWork(string path)
        {
            string fileContent = File.ReadAllText(path);
            MessageBox.Show(fileContent);
        }

        internal static void PrintData()
        {
            MessageBox.Show($"pythonPath: {pythonExecPath}");
            MessageBox.Show($"organizePicturesScriptPath: {organizePicturesScriptPath}");
        }
    }
}
