using System.Runtime.InteropServices;
using System.Text;
using AudioSwitch.App;
using AudioSwitch.Enum;

namespace AudioSwitch.Utils;

public static class HotKeys
{
    #region External

    private const string Dll = "user32.dll";

    [DllImport(Dll)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport(Dll)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport(Dll)]
    private static extern int ToUnicode(
        uint wVirtKey,
        uint wScanCode,
        byte[] lpKeyState,
        [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
        StringBuilder pwszBuff,
        int cchBuff,
        uint wFlags);

    [DllImport(Dll)]
    private static extern bool GetKeyboardState(byte[] lpKeyState);

    [DllImport(Dll)]
    private static extern uint MapVirtualKey(uint uCode, uint uMapType);

    #endregion

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

    public static string KeysToKeySymbol(Keys key)
    {
        var keyCode = (uint)key;
        var keyboardState = new byte[256];
        if (!GetKeyboardState(keyboardState)) return string.Empty;

        var sb = new StringBuilder(2);
        var scanCode = MapVirtualKey(keyCode, 0);
        var uni = ToUnicode(keyCode, scanCode, keyboardState, sb, sb.Capacity, 0);
        return uni > 0 ? sb.ToString() : string.Empty;
    }
}