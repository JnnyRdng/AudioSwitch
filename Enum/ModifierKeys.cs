namespace AudioSwitch.Enum;

[Flags]
public enum ModifierKeys : uint
{
    None = 0,
    Alt = 0x001,
    Control = 0x002,
    Shift = 0x004,
    Win = 0x008,
}