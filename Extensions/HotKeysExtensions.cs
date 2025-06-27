using AudioSwitch.Enum;
using AudioSwitch.Utils;

namespace AudioSwitch.Extensions;

public static class HotKeysExtensions
{
    private static string KeysToKeySymbol(Keys key)
    {
        if (!Native.GetKeyboardState(out var keyboardState)) return string.Empty;
        var unicode = Native.ToUnicode(key, keyboardState, out var sb);
        return unicode > 0 ? sb.ToString() : string.Empty;
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