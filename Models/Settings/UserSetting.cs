using SmartOrganizerWPF.Interfaces;

namespace SmartOrganizerWPF.Models.Settings
{
    internal class UserSetting<T>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Tooltip { get; private set; }

        private IUserSettingType<T> type;
        public T Value => type.CurrentValue;

        public UserSetting(string name, string description, IUserSettingType<T> type)
        {
            Name = name;
            Description = description;
            this.type = type;
            Tooltip = this.type.CreateTooltip();
        }
    }
}
