using AudioSwitch.Enum;
using AudioSwitch.Extensions;
using AudioSwitch.Services;
using AudioSwitch.Utils;

namespace AudioSwitch.App;

public class DeviceHotKey
{
    public string DeviceName { get; }
    public Keys Shortcut { get; private set; }
    public int Id { get; }

    public DeviceHotKey(string deviceName, Keys shortcut, int id)
    {
        DeviceName = deviceName;
        Shortcut = shortcut;
        Id = id;
    }

    public uint GetModifiers()
    {
        return Shortcut.GetModifiers().AsUint();
    }

    public uint GetKey()
    {
        return Shortcut.GetKey().AsUint();
    }

    /// <summary>
    /// Change the device shortcut.
    /// If registering new shortcut fails, attempt to revert to the existing.
    /// If reverting fails, clear the shortcut.
    /// </summary>
    /// <param name="handle">The form handle to register the shortcut against</param>
    /// <param name="newShortcut">The new shortcut</param>
    /// <returns>Return code</returns>
    public Return ChangeShortcut(IntPtr handle, Keys newShortcut)
    {
        try
        {
            var previousShortcut = Shortcut;
            HotKeys.UnregisterHotKey(handle, this);
            Shortcut = newShortcut;
            var success = HotKeys.RegisterHotKey(handle, this);
            if (success) return Return.Success;
            Shortcut = previousShortcut;
            var revertSuccess = HotKeys.RegisterHotKey(handle, this);
            if (!revertSuccess)
            {
                Shortcut = 0u;
            }

            return revertSuccess ? Return.Noop : Return.Failure;
        }
        finally
        {
            SettingsService.Save();
        }
    }
}