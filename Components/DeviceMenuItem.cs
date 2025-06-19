using AudioSwitch.App;
using AudioSwitch.Extensions;

namespace AudioSwitch.Components;

public class DeviceMenuItem : ToolStripMenuItem
{

    public DeviceMenuItem(DeviceHotKey device, EventHandler onClick) : base(device.DeviceName, null, onClick)
    {
        ShowShortcutKeys = true;
        ShortcutKeys = device.Shortcut;
        ShortcutKeyDisplayString = device.Shortcut.ToDisplayString();
    }
}