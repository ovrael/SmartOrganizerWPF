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
        }

        internal void UpdateSettings()
        {
            UserSettings.DeepSearch.SetValue(DeepSearchCheckBox.IsChecked.Value);
        }

        private void DeepSearchCheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox? checkBox = sender as CheckBox;
            if (checkBox == null || checkBox.IsChecked == null) return;

            DeepSearchCheckBox.IsChecked = checkBox.IsChecked;
            UserSettings.DeepSearch.SetValue((bool)checkBox.IsChecked);
        }
    }
}
