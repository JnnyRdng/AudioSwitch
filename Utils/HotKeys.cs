using System.Runtime.InteropServices;
using AudioSwitch.App;

namespace AudioSwitch.Utils;

public static class HotKeys
{
    #region External

    private const string Dll = "user32.dll";

    [DllImport(Dll)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport(Dll)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    #endregion

    public static void RegisterMultipleHotKeys(IntPtr hWnd, List<DeviceHotKey> hotkeys)
    {
        foreach (var hotkey in hotkeys)
        {
            var success = RegisterHotKey(hWnd, hotkey);
            if (success) continue;
            throw new ArgumentException($"Failed to register hotkey '{hotkey.Shortcut}' (id: {hotkey.Id})");
        }
    }

    public static bool RegisterHotKey(IntPtr hWnd, DeviceHotKey hotkey)
    {
        return RegisterHotKey(hWnd, hotkey.Id, hotkey.GetModifiers(), hotkey.GetKey());
    }

    public static void UnregisterMultipleHotKeys(IntPtr hWnd, List<DeviceHotKey> hotkeys)
    {
        foreach (var hotkey in hotkeys)
        {
            UnregisterHotKey(hWnd, hotkey);
        }
    }

    public static void UnregisterHotKey(IntPtr hWnd, DeviceHotKey hotkey)
    {
        UnregisterHotKey(hWnd, hotkey.Id);
    }
}