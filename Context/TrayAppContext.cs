using AudioSwitch.Enum;
using AudioSwitch.Forms;
using AudioSwitch.Services;
using AudioSwitch.Utils;
using AudioSwitcher.AudioApi.CoreAudio;

namespace AudioSwitch.Context;

public class TrayAppContext : ApplicationContext
{
    private readonly NotifyIcon _trayIcon;
    private readonly CoreAudioController _audioController = new();

    public TrayAppContext()
    {
        var dummyForm = new HiddenForm();
        HotKeys.RegisterMultipleHotKeys(dummyForm.Handle, SettingsService.Settings.DeviceHotKeys);

        dummyForm.HotKeyPressed += OnHotKeyPressed;

        _trayIcon = new NotifyIcon
        {
            Icon = new Icon("Resources/audioswitch.ico"), //SystemIcons.Application,
            Visible = true,
            Text = $"AudioSwitch ({SettingsService.GetVersion()})",
            ContextMenuStrip = new ContextMenuStrip
            {
                Items =
                {
                    GetDarkModeToggle(),
                    GetExitButton(dummyForm.Handle),
                }
            }
        };
    }

    private static ToolStripMenuItem GetDarkModeToggle()
    {
        return new ToolStripMenuItem("Dark Mode", null, (s, e) =>
        {
            if (s is not ToolStripMenuItem menu) return;
            SettingsService.Settings.DarkMode = !SettingsService.Settings.DarkMode;
            menu.Checked = SettingsService.Settings.DarkMode;
        })
        {
            CheckOnClick = false,
            Checked = SettingsService.Settings.DarkMode,
        };
    }

    private ToolStripMenuItem GetExitButton(IntPtr handle)
    {
        return new ToolStripMenuItem("Exit", null, (s, e) =>
        {
            HotKeys.UnregisterMultipleHotKeys(handle, SettingsService.Settings.DeviceHotKeys);
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
            Application.Exit();
        });
    }

    private async void OnHotKeyPressed(int id)
    {
        var device = SettingsService.Settings.DeviceHotKeys.FirstOrDefault(x => x.Id == id);
        if (device is null) return;
        await SwitchTo(device.DeviceName);
    }

    private async Task SwitchTo(string name)
    {
        var devices = await _audioController.GetPlaybackDevicesAsync();
        var device = devices.FirstOrDefault(d => d.FullName.StartsWith(name));
        if (device == null) return;

        await device.SetAsDefaultAsync();
        await new ToastForm($"Switched to {device.FullName}").ShowToast();
    }
}