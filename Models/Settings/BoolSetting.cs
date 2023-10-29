using SmartOrganizerWPF.Interfaces;

namespace SmartOrganizerWPF.Models.Settings
{
    internal class BoolSetting : IUserSettingType<bool>
    {
        public bool DefaultValue { get; private set; }
        public bool CurrentValue { get; private set; }

        private string tooltipIfOff;
        private string tooltipIfOn;

        public BoolSetting(bool defualt, string tooltipIfOff, string tooltipIfOn)
        {
            DefaultValue = defualt;
            CurrentValue = DefaultValue;

            this.tooltipIfOff = tooltipIfOff;
            this.tooltipIfOn = tooltipIfOn;
        }

        public string CreateTooltip()
        {
            string tooltipDefault = DefaultValue ? "on" : "off";

            return $"If on: {tooltipIfOn}\n" +
                    $"If off: {tooltipIfOff}\n" +
                    $"Default: {tooltipDefault}";
        }

        public void SetCurrentValue(bool value)
        {
            CurrentValue = value;
        }
    }
}
