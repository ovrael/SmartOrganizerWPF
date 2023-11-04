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

    }
}
