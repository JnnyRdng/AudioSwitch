using AudioSwitch.Enum;
using Microsoft.Win32;

namespace AudioSwitch.Utils;

public static class ThemeUtils
{

    public static bool IsDark(Theme appSetting)
    {
        if (appSetting != Theme.System) return appSetting == Theme.Dark;
        return GetSystemTheme() == Theme.Dark;
    }
    
    private static Theme GetSystemTheme()
    {
        const string registryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        const string valueName = "SystemUsesLightTheme";
        
        using var key = Registry.CurrentUser.OpenSubKey(registryKeyPath);
        if (key?.GetValue(valueName) is int value)
        {
            return value == 0 ?  Theme.Dark : Theme.Light;
        }

        return Theme.Light;
    }
}