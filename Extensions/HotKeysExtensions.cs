using AudioSwitch.Enum;
using AudioSwitch.Utils;

namespace AudioSwitch.Extensions;

public static class HotKeysExtensions
{
    public static string ToSymbol(this Keys key)
    {
        return HotKeys.KeysToKeySymbol(key);
    }

    public static string ToModifierString(this ModifierKeys keys)
    {
        var parts = new List<string>();

        if (keys.HasFlag(ModifierKeys.Control)) parts.Add("Ctrl");
        if (keys.HasFlag(ModifierKeys.Alt)) parts.Add("Alt");
        if (keys.HasFlag(ModifierKeys.Shift)) parts.Add("Shift");
        if (keys.HasFlag(ModifierKeys.Win)) parts.Add("Win");

        return string.Join('+', parts);
    }
}