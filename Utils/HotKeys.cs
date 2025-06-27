using System.Runtime.InteropServices;
using AudioSwitch.App;

namespace AudioSwitch.Utils;

public static class HotKeys
{
    public static void RegisterHotKeys(IntPtr hWnd, List<DeviceHotKey> hotkeys)
    {
        foreach (var hotkey in hotkeys)
        {
            var success = Native.RegisterHotKey(hWnd, hotkey);
            if (success) continue;
            int error = Marshal.GetLastWin32Error();
            throw new ArgumentException(
                $"Error '{error}'\nFailed to register hotkey '{hotkey.Shortcut}' (id: {hotkey.Id})");
        }
    }

    public static void UnregisterHotKeys(IntPtr hWnd, List<DeviceHotKey> hotkeys)
    {
        foreach (var hotkey in hotkeys)
        {
            Native.UnregisterHotKey(hWnd, hotkey);
        }
    }
}