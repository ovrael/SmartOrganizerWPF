using System.Collections.Generic;

using SmartOrganizerWPF.Models.Settings;

namespace SmartOrganizerWPF.Common
{
    internal static class UserSettings
    {
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

    }
}
