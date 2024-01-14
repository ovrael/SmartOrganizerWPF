using SmartOrganizerWPF.Models.Settings;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SmartOrganizerWPF.Common
{
    internal static class UserSettings
    {
        private static readonly string settingsFileName = "settings.stgs";

        public static UserSetting<bool> DeepSearch = new UserSetting<bool>(
            "Deep search",
            "Allow to search content of directories in selected folder",
            new BoolSetting(
                true,
                "Organize only files within selected folder",
                "Organize files and content of directories within selected folder"
            ));

        public static UserSetting<bool> IncludeEmptyFolders = new UserSetting<bool>(
            "Include empty folders",
            "Include empty folders into search and then into organizing",
            new BoolSetting(
                true,
                "Do not include and organize empty folders",
                "Empty folders are included and organized"
            ));

        public static UserSetting<bool> CreateOtherFolder = new UserSetting<bool>(
            "Create 'other' folder",
            "Should create folder named 'other' for uncategorized files",
            new BoolSetting(
                true,
                "Uncategorized files will be put in the same folder as higher category",
                "If file is uncategorized at given level, it will be put into 'other' folder"
            ));

        public static UserSetting<bool> DeleteMovedFiles = new UserSetting<bool>(
        "Delete moved files",
        "Should delete organized files from old directories",
        new BoolSetting(
            false,
            "Organized files are not deleted from old directories, works like cut -> paste",
            "Delete files at old directories after organizing, works like copy -> paste"
        ));

        public static UserSetting<bool> CreateOrganizedFolder = new UserSetting<bool>(
        "Create organized folder",
        "All organized directories are put into 'organized' folder in selected directory",
        new BoolSetting(
            true,
            "Organized folders are created at selected directory",
            "Create 'organized' folder then organized folders are created inside 'organizded'"
        ));

        public static UserSetting<bool> CreateEmptyFolders = new UserSetting<bool>(
        "Create empty folders",
        "Should create empty organized folders",
        new BoolSetting(
            false,
            "Empty organized folders are not created",
            "Creates empty organized folders"
        ));

        public static UserSetting<bool> OpenOrganizedFolderAfterWork = new UserSetting<bool>(
        "Open organized folder after work",
        "After organizing all files program will open top level organized folder",
        new BoolSetting(
            false,
            "Program will only show notification after work is done",
            "Top level organized folder will be open after notification of completed work"
        ));

        public static UserSetting<bool> CopyOnRun = new UserSetting<bool>(
            "Copy on run",
            "Copies files to destination after organizing without user interaction.",
            new BoolSetting(
                false,
                "Copy files to destination after run",
                "Allow user to modifie organized tree after run, then user must start copying "
            ));

        public static UserSetting<bool> OverwriteMoved = new UserSetting<bool>(
            "Overwrite moved files",
            "Overwrites existed files when moving",
            new BoolSetting(
                false,
                "Overwrite already existed files",
                "Do not overwrite, old files still exist and file won't be moved"
            ));

        static UserSettings()
        {
            LoadFromFile();
        }

        public static void SaveToFile()
        {
            string dataFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(dataFolderPath))
            {
                Directory.CreateDirectory(dataFolderPath);
            }

            string settingsPath = Path.Combine(dataFolderPath, settingsFileName);
            List<string> settingsLines = new List<string>();

            var settings = GetFields();
            foreach (var settingField in settings)
            {
                object fieldObject = settingField.GetValue(null);
                MethodInfo getValueMethod = fieldObject.GetType().GetMethod("get_Value");
                object? settingValue = getValueMethod.Invoke(fieldObject, null);

                settingsLines.Add($"{settingField.Name}=-:-={settingValue}");
            }
            File.WriteAllLines(settingsPath, settingsLines);
        }

        public static void LoadFromFile()
        {
            string dataFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            string settingsPath = Path.Combine(dataFolderPath, settingsFileName);
            if (!File.Exists(settingsPath))
            {
                SaveToFile();
                return;
            }

            FieldInfo[] settings = GetFields();
            string[] settingsLines = File.ReadAllLines(settingsPath);
            foreach (var settingLine in settingsLines)
            {
                string[] settingParts = settingLine.Split("=-:-=");
                if (settingParts.Length != 2) continue;

                var setting = settings.FirstOrDefault(s => s.Name == settingParts[0]);
                if (setting == null) continue;

                object fieldObject = setting.GetValue(null);
                MethodInfo setValueMethod = fieldObject.GetType().GetMethod("LoadData");
                if (setValueMethod == null) continue;

                setValueMethod.Invoke(fieldObject, new object[] { settingParts[1] });
            }
        }

        public static void ResetToDefault()
        {
            var settings = GetFields();
            foreach (var settingField in settings)
            {
                object fieldObject = settingField.GetValue(null);
                MethodInfo getValueMethod = fieldObject.GetType().GetMethod("get_DefaultValue");
                object? defaultValue = getValueMethod.Invoke(fieldObject, null);
                //Type settingType = getValueMethod.ReturnType;

                MethodInfo setValueMethod = fieldObject.GetType().GetMethod("SetValue");
                setValueMethod.Invoke(fieldObject, new object[] { defaultValue });
            }

        }

        private static FieldInfo[] GetFields()
        {
            Type userSettings = typeof(UserSettings);

            if (!userSettings.IsClass)
                return Array.Empty<FieldInfo>();

            if (!userSettings.IsAbstract || !userSettings.IsSealed)
                return Array.Empty<FieldInfo>();

            return userSettings.GetFields(BindingFlags.Public | BindingFlags.Static);
        }
    }
}
