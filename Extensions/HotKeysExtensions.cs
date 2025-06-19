using System.Runtime.InteropServices;
using System.Text;
using AudioSwitch.Enum;

namespace AudioSwitch.Extensions;

public static class HotKeysExtensions
{
    #region External

    private const string Dll = "user32.dll";

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

    private static string KeysToKeySymbol(Keys key)
    {
        var keyCode = (uint)key;
        var keyboardState = new byte[256];
        if (!GetKeyboardState(keyboardState)) return string.Empty;

        var sb = new StringBuilder(2);
        var scanCode = MapVirtualKey(keyCode, 0);
        var uni = ToUnicode(keyCode, scanCode, keyboardState, sb, sb.Capacity, 0);
        return uni > 0 ? sb.ToString() : string.Empty;
    }

    public static ModifierKeys GetModifiers(this Keys keys)
    {
        ModifierKeys modifiers = 0;
        var keyModifiers = keys & Keys.Modifiers;

        if (keyModifiers.HasFlag(Keys.Control))
            modifiers |= ModifierKeys.Control;
        if (keyModifiers.HasFlag(Keys.Shift))
            modifiers |= ModifierKeys.Shift;
        if (keyModifiers.HasFlag(Keys.Alt))
            modifiers |= ModifierKeys.Alt;

        return modifiers;
    }

    public static Keys GetKey(this Keys keys)
    {
        return keys & Keys.KeyCode;
    }

    public static uint AsUint(this ModifierKeys modifiers)
    {
        return (uint)modifiers;
    }

    public static uint AsUint(this Keys key)
    {
        return (uint)key;
    }

    public static string ToDisplayString(this Keys keys)
    {
        var parts = new List<string>();
        var mods = keys & Keys.Modifiers;
        if (mods.HasFlag(Keys.Control))
            parts.Add("Ctrl");
        if (mods.HasFlag(Keys.Shift))
            parts.Add("Shift");
        if (mods.HasFlag(Keys.Alt))
            parts.Add("Alt");
        var modString = string.Join("+", parts);
        var key = KeysToKeySymbol(keys.GetKey());
        return modString.Length == 0 ? key : $"{modString}+{key}";
    }
}