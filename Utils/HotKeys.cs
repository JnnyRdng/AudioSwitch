using System.Runtime.InteropServices;
using AudioSwitch.Context;
using AudioSwitch.Enum;

namespace AudioSwitch.Utils;

public static class HotKeys
{
    private const string Dll = "user32.dll";

    [DllImport(Dll)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport(Dll)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public static uint Modifiers(params ModifierKeys[] keys)
    {
        return keys.Aggregate(0u, (acc, key) => acc | (uint)key);
    }

    public static void RegisterMultipleHotKeys(IntPtr hWnd, List<DeviceHotKey> hotkeys)
    {
        foreach (var hotkey in hotkeys)
        {
            var success = RegisterHotKey(hWnd, hotkey.Id, hotkey.Modifiers, (uint)hotkey.Key);
            if (success) continue;
            throw new ArgumentException($"Failed to register hotkey {hotkey.Key} {hotkey.Modifiers} {hotkey.Id}");
        }
    }

    public static void UnregisterMultipleHotKeys(IntPtr hWnd, List<DeviceHotKey> hotkeys)
    {
        foreach (var hotkey in hotkeys)
        {
            UnregisterHotKey(hWnd, hotkey.Id);
        }
    }
}