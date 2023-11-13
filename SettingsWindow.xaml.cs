using System.Windows;
using System.Windows.Controls;

using SmartOrganizerWPF.Common;
using SmartOrganizerWPF.Models.Settings;

namespace SmartOrganizerWPF
{
    /// <summary>
    /// Logika interakcji dla klasy SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            AddCheckBoxSetting(UserSettings.DeepSearch);
            AddCheckBoxSetting(UserSettings.IncludeEmptyFolders);
            AddCheckBoxSetting(UserSettings.CreateOtherFolder);

            //DeepSearchCheckBox.IsChecked = UserSettings.DeepSearch.Value;
            //IncludeEmptyFoldersCheckBox.IsChecked = UserSettings.IncludeEmptyFolders.Value;
            //CreateOtherFolderCheckBox.IsChecked = UserSettings.CreateOtherFolder.Value;
        }

        private void AddCheckBoxSetting(UserSetting<bool> setting)
        {
            Border border = new Border()
            {
                Padding = new Thickness(5, 5, 5, 5),
                BorderBrush = null,
                BorderThickness = new Thickness(0, 0, 0, 0)
            };

            StackPanel settingsContent = new StackPanel()
            {
                Height = double.NaN,
                ToolTip = setting.Tooltip
            };


            CheckBox checkBox = new CheckBox()
            {
                Content = $"{setting.Name}",
                Height = 20,
                FontSize = 13,
                Foreground = Tools.CreateBrush("FF6DB38B"),
                IsThreeState = false,
                IsChecked = setting.Value,
                Tag = setting
            };
            checkBox.Click += SettingCheckBox_Click;


            TextBlock description = new TextBlock()
            {
                Text = setting.Description,
                Height = 14,
                FontSize = 11,
                Foreground = Tools.CreateBrush("FF759A85")
            };



            settingsContent.Children.Add(checkBox);
            settingsContent.Children.Add(description);
            border.Child = settingsContent;
            SettingsStackPanel.Children.Add(border);
        }

        private void SettingCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox checkBox) return;
            if (checkBox.Tag is not UserSetting<bool> setting) return;

            setting.SetValue(checkBox.IsChecked.Value);
        }

        //internal void UpdateSettings()
        //{
        //    stack
        //    UserSettings.DeepSearch.SetValue(DeepSearchCheckBox.IsChecked.Value);
        //    UserSettings.IncludeEmptyFolders.SetValue(IncludeEmptyFoldersCheckBox.IsChecked.Value);
        //    UserSettings.CreateOtherFolder.SetValue(CreateOtherFolderCheckBox.IsChecked.Value);
        //}

        //private void DeepSearchCheckBox_Click(object sender, RoutedEventArgs e)
        //{
        //    CheckBox? checkBox = sender as CheckBox;
        //    if (checkBox == null || checkBox.IsChecked == null) return;

        //    DeepSearchCheckBox.IsChecked = checkBox.IsChecked;
        //    UserSettings.DeepSearch.SetValue((bool)checkBox.IsChecked);
        //}
        //private void Settings2CheckBox_Click(object sender, RoutedEventArgs e)
        //{
        //    CheckBox? checkBox = sender as CheckBox;
        //    if (checkBox == null || checkBox.IsChecked == null) return;

        //    IncludeEmptyFoldersCheckBox.IsChecked = checkBox.IsChecked;
        //    UserSettings.IncludeEmptyFolders.SetValue((bool)checkBox.IsChecked);
        //}
        //private void Settings3CheckBox_Click(object sender, RoutedEventArgs e)
        //{
        //    CheckBox? checkBox = sender as CheckBox;
        //    if (checkBox == null || checkBox.IsChecked == null) return;

        //    CreateOtherFolderCheckBox.IsChecked = checkBox.IsChecked;
        //    UserSettings.CreateOtherFolder.SetValue((bool)checkBox.IsChecked);
        //}
    }
}
