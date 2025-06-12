using AudioSwitch.Services;

namespace AudioSwitch.Context;

public class AppSettings
{
    #region Fields

    public bool DarkMode { get; private set; } = true;

    public List<DeviceHotKey> DeviceHotKeys { get; private set; } = new();

    #endregion

    #region Setters

    public void ToggleDarkMode()
    {
        DarkMode = !DarkMode;
        SettingsService.Save();
    }

    #endregion
}