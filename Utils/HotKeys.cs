using System.Runtime.InteropServices;
using AudioSwitch.Enum;

namespace AudioSwitch.Utils;

public static class HotKeys
{
    private const string dll = "user32.dll";

    [DllImport(dll)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport(dll)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public static uint Modifiers(params ModifierKeys[] keys)
    {
        return keys.Aggregate(0u, (acc, key) => acc | (uint)key);
    }

    public static void RegisterMultipleHotKeys(IntPtr hWnd, params (int id, uint modifiers, Keys key)[] keys)
    {
        foreach (var (id, modifiers, key) in keys)
        {
            var success = RegisterHotKey(hWnd, id, modifiers, (uint)key);
            if (success) continue;
            throw new ArgumentException($"Failed to register hotkey {key} {modifiers}");
        }
    }

    public static void UnregisterMultipleHotKeys(IntPtr hWnd, params int[] ids)
    {
        foreach (var id in ids)
        {
            UnregisterHotKey(hWnd, id);
        }
    }
}