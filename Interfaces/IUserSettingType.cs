namespace SmartOrganizerWPF.Interfaces
{
    internal interface IUserSettingType<T>
    {
        public T DefaultValue { get; }
        public T CurrentValue { get; }

        public void SetCurrentValue(T value);
        public string CreateTooltip();
    }
}
