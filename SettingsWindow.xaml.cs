using SmartOrganizerWPF.Common;
using SmartOrganizerWPF.Models.Settings;

using System.Windows;
using System.Windows.Controls;

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

            StackPanel loadingFilesCategory = CreateSettingsCategory("Loading files", "loadingFiles");
            StackPanel organizingFilesCategory = CreateSettingsCategory("Organizing files", "organizingFiles");
            loadingFilesCategory.Children.Add(CreateCheckBoxSetting(UserSettings.DeepSearch));
            loadingFilesCategory.Children.Add(CreateCheckBoxSetting(UserSettings.IncludeEmptyFolders));
            organizingFilesCategory.Children.Add(CreateCheckBoxSetting(UserSettings.CreateOtherFolder));
            organizingFilesCategory.Children.Add(CreateCheckBoxSetting(UserSettings.DeleteMovedFiles));
            organizingFilesCategory.Children.Add(CreateCheckBoxSetting(UserSettings.CreateOrganizedFolder));
            organizingFilesCategory.Children.Add(CreateCheckBoxSetting(UserSettings.CreateEmptyFolders));
            organizingFilesCategory.Children.Add(CreateCheckBoxSetting(UserSettings.OpenOrganizedFolderAfterWork));
            organizingFilesCategory.Children.Add(CreateCheckBoxSetting(UserSettings.CopyOnRun));
        }

        private StackPanel? GetCategory(string tag)
        {
            if (CategoriesScrollViewer.Content is not StackPanel scrollContent) return null;

            foreach (var category in scrollContent.Children)
            {
                if (category is not StackPanel categoryContent) return null;
                if (categoryContent.Tag is not string categoryTag) return null;
                if (categoryTag == tag) return categoryContent;
            }

            return null;
        }

        private StackPanel CreateSettingsCategory(string displayName, string tag)
        {
            StackPanel category = new StackPanel()
            {
                Tag = tag
            };
            DockPanel categoryName = new DockPanel() { LastChildFill = true };

            Label label = new Label()
            {
                Content = displayName,
                Height = 40,
                FontSize = 20,
                Foreground = Tools.CreateBrush("#5acc8b")
            };
            DockPanel.SetDock(label, Dock.Left);

            Border decoration = new Border()
            {
                Height = 2,
                Width = 300,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                BorderBrush = Tools.CreateBrush("#5acc8b"),
                Background = Tools.CreateBrush("#5acc8b"),
            };
            DockPanel.SetDock(decoration, Dock.Left);

            categoryName.Children.Add(label);
            categoryName.Children.Add(decoration);

            category.Children.Add(categoryName);

            if (CategoriesScrollViewer.Content is not StackPanel scrollContent) return category;
            scrollContent.Children.Add(category);

            return category;
        }

        private Border CreateCheckBoxSetting(UserSetting<bool> setting)
        {
            Border border = new Border()
            {
                Padding = new Thickness(5, 5, 5, 5),
                Margin = new Thickness(0, 0, 0, 5),
                BorderBrush = null,
                BorderThickness = new Thickness(0, 0, 0, 0),
                Tag = "checkbox"
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
                Height = 16,
                FontSize = 11,
                Foreground = Tools.CreateBrush("FF759A85")
            };

            settingsContent.Children.Add(checkBox);
            settingsContent.Children.Add(description);
            border.Child = settingsContent;

            return border;
        }

        private void RefreshUI()
        {
            if (CategoriesScrollViewer.Content is not StackPanel scrollContent) return;
            foreach (var category in scrollContent.Children)
            {
                if (category is not StackPanel categoryPanel) continue;
                foreach (var setting in categoryPanel.Children)
                {
                    if (setting is not Border settingBorder) continue;
                    if (settingBorder.Tag is not string settingType) continue;

                    switch (settingType)
                    {
                        case "checkbox":
                            RefreshCheckBox(settingBorder);
                            break;

                        default:
                            break;
                    }

                }
            }
        }

        private void RefreshCheckBox(Border settingBorder)
        {
            if (settingBorder.Child is not StackPanel stackPanel) return;
            if (stackPanel.Children[0] is not CheckBox checkBox) return;
            if (checkBox.Tag is not UserSetting<bool> setting) return;

            checkBox.IsChecked = setting.Value;
        }

        private void SettingCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox checkBox) return;
            if (checkBox.Tag is not UserSetting<bool> setting) return;

            setting.SetValue(checkBox.IsChecked.Value);
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            UserSettings.SaveToFile();
        }

        private void ResetSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            UserSettings.ResetToDefault();
            UserSettings.SaveToFile();
            RefreshUI();
        }
    }
}
