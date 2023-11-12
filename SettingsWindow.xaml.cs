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

            DeepSearchCheckBox.IsChecked = UserSettings.DeepSearch.Value;
            IncludeEmptyFoldersCheckBox.IsChecked = UserSettings.IncludeEmptyFolders.Value;
            CreateOtherFolderCheckBox.IsChecked = UserSettings.CreateOtherFolder.Value;
        }

        internal void UpdateSettings()
        {
            UserSettings.DeepSearch.SetValue(DeepSearchCheckBox.IsChecked.Value);
            UserSettings.IncludeEmptyFolders.SetValue(IncludeEmptyFoldersCheckBox.IsChecked.Value);
            UserSettings.CreateOtherFolder.SetValue(CreateOtherFolderCheckBox.IsChecked.Value);
        }

        private void DeepSearchCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox? checkBox = sender as CheckBox;
            if (checkBox == null || checkBox.IsChecked == null) return;

            DeepSearchCheckBox.IsChecked = checkBox.IsChecked;
            UserSettings.DeepSearch.SetValue((bool)checkBox.IsChecked);
        }
        private void Settings2CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox? checkBox = sender as CheckBox;
            if (checkBox == null || checkBox.IsChecked == null) return;

            IncludeEmptyFoldersCheckBox.IsChecked = checkBox.IsChecked;
            UserSettings.IncludeEmptyFolders.SetValue((bool)checkBox.IsChecked);
        }
        private void Settings3CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox? checkBox = sender as CheckBox;
            if (checkBox == null || checkBox.IsChecked == null) return;

            CreateOtherFolderCheckBox.IsChecked = checkBox.IsChecked;
            UserSettings.CreateOtherFolder.SetValue((bool)checkBox.IsChecked);
        }
    }
}
