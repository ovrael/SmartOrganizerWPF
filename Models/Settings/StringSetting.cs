using SmartOrganizerWPF.Interfaces;
using System;

namespace SmartOrganizerWPF.Models.Settings
{
    internal class StringSetting : IUserSettingType<string>
    {
        public string DefaultValue { get; private set; }

        public string CurrentValue { get; private set; }

        public string CreateTooltip()
        {
            throw new NotImplementedException();
        }

        public void SetCurrentValue(string value)
        {
            if (CurrentValue == value) return;
            CurrentValue = value;
        }
    }
}
