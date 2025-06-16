using AudioSwitch.Enum;
using AudioSwitch.Extensions;
using AudioSwitch.Forms;
using AudioSwitch.Services;
using AudioSwitch.Utils;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace AudioSwitch.App;

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
            Icon = new Icon("Resources/audioswitch.ico"),
            Visible = true,
            Text = $"AudioSwitch",
            ContextMenuStrip = new ContextMenuStrip
            {
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9)
            }
        };

        foreach (var device in SettingsService.Settings.DeviceHotKeys)
        {
            _trayIcon.ContextMenuStrip.Items.Add(CreateDescriptivePanelFromDevice(device));
            _trayIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
        }

        _trayIcon.ContextMenuStrip.Items.Add(GetDarkModeToggle());
        _trayIcon.ContextMenuStrip.Items.Add(GetExitButton(dummyForm.Handle));
        _trayIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
        _trayIcon.ContextMenuStrip.Items.Add(GetVersionItem());
    }

    private static ToolStripItem CreateDescriptivePanelFromDevice(DeviceHotKey device)
    {
        var panel = new Panel
        {
            BackColor = Color.Transparent,
            Padding = new Padding(4),
            AutoSize = true,
        };

        var titleLabel = new Label
        {
            Text = device.DeviceName,
            AutoSize = true,
        };
        var modString = ((ModifierKeys)device.Modifiers).ToModifierString();
        var moddedString = modString.Length == 0 ? string.Empty : $"{modString}+";
        var key = device.Key.ToSymbol();
        var descLabel = new Label
        {
            Text = $"{moddedString}{key}",
            Font = new Font("Segoe UI", 8),
            ForeColor = SystemColors.GrayText,
            AutoSize = true
        };

        panel.Controls.Add(titleLabel);
        panel.Controls.Add(descLabel);
        descLabel.Top = titleLabel.Bottom + 2;

        return new ToolStripControlHost(panel)
        {
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            AutoSize = true,
            Size = panel.PreferredSize,
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
            Checked = SettingsService.Settings.DarkMode
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

    private static ToolStripItem GetVersionItem()
    {
        SettingsService.GetVersion();
        var panel = new Panel
        {
            BackColor = Color.Transparent,
            Padding = new Padding(4),
            AutoSize = true,
        };
        panel.Controls.Add(new Label
        {
            Text = $"v{SettingsService.GetVersion()}",
            ForeColor = SystemColors.GrayText,
            AutoSize = true
        });
        return new ToolStripControlHost(panel)
        {
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            AutoSize = false,
            Size = panel.PreferredSize
        };
    }

    private async void OnHotKeyPressed(int id)
    {
        var device = SettingsService.Settings.DeviceHotKeys.FirstOrDefault(x => x.Id == id);
        if (device is null) return;
        await SwitchTo(device.DeviceName);
    }

    private async Task SwitchTo(string name)
    {
        var devices = await _audioController.GetPlaybackDevicesAsync(DeviceState.Active);
        var device = devices.FirstOrDefault(d => d.FullName.StartsWith(name));
        if (device == null) return;

        await device.SetAsDefaultAsync();
        await new ToastForm($"Switched to {device.FullName}").ShowToast();
    }
}