using AudioSwitch.Components;
using AudioSwitch.Enum;
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
    private readonly List<ToolStripItem> _deviceButtons = new();

    public TrayAppContext()
    {
        var dummyForm = new HiddenForm();
        HotKeys.RegisterHotKeys(dummyForm.Handle, SettingsService.Settings.DeviceHotKeys);
        dummyForm.HotKeyPressed += OnHotKeyPressed;

        _trayIcon = new NotifyIcon
        {
            Icon = new Icon("Resources/audioswitch.ico"),
            Visible = true,
            Text = Constants.AppName,
            ContextMenuStrip = new ContextMenuStrip
            {
                BackColor = Color.White,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9)
            }
        };

        foreach (var device in SettingsService.Settings.DeviceHotKeys)
        {
            // device.ChangeShortcut(dummyForm.Handle,
            //     Keys.Control | Keys.Shift | Keys.Alt | (device.DeviceName.StartsWith("Speak")
            //         ? Keys.OemCloseBrackets
            //         : Keys.OemOpenBrackets));
            
            _deviceButtons.Add(GetDeviceButton(device));
        }

        _ = CheckDeviceMenuItems();
        
        _trayIcon.ContextMenuStrip.Items.AddRange(_deviceButtons.ToArray());
        _trayIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
        _trayIcon.ContextMenuStrip.Items.Add(GetThemeDropdown());
        _trayIcon.ContextMenuStrip.Items.Add(GetExitButton(dummyForm.Handle));
        _trayIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
        _trayIcon.ContextMenuStrip.Items.Add(GetVersionItem());
    }

    private ToolStripMenuItem GetDeviceButton(DeviceHotKey device)
    {
        return new DeviceMenuItem(device, (s, e) =>
        {
            _ = SwitchTo(device.DeviceName);
        });
    }

    private ToolStripMenuItem GetThemeDropdown()
    {
        var menu = new ToolStripMenuItem("Theme");

        var darkOption = new ToolStripMenuItem("Dark") { Tag = Theme.Dark };
        var lightOption = new ToolStripMenuItem("Light") { Tag = Theme.Light };
        var systemOption = new ToolStripMenuItem("System") { Tag = Theme.System };
        
        darkOption.Click += SettingItemClick;
        lightOption.Click += SettingItemClick;
        systemOption.Click += SettingItemClick;
        
        menu.DropDownItems.Add(darkOption);
        menu.DropDownItems.Add(lightOption);
        menu.DropDownItems.Add(systemOption);

        if (_trayIcon.ContextMenuStrip != null)
        {
            _trayIcon.ContextMenuStrip.Opening += ContextMenuStripOpening;
        }

        return menu;

        void UpdateCheckedState()
        {
          
        }

        void ContextMenuStripOpening(object? sender, EventArgs e)
        {
            UpdateCheckedState();
        }

        void SettingItemClick(object? sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem { Tag: Theme theme }) return;
           
            UpdateCheckedState();
        }
    }

    private ToolStripMenuItem GetExitButton(IntPtr handle)
    {
        return new ToolStripMenuItem("Exit", null, (s, e) =>
        {
            HotKeys.UnregisterHotKeys(handle, SettingsService.Settings.DeviceHotKeys);
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
        var device = devices?.FirstOrDefault(d => d.FullName.StartsWith(name));
        if (device == null) return;

        var success = await device.SetAsDefaultAsync();
        if (!success) return;

        await CheckDeviceMenuItems();
        await new ToastForm($"Switched to {device.FullName}").ShowToast();
    }

    private async Task CheckDeviceMenuItems()
    {
        var devices = await _audioController.GetPlaybackDevicesAsync(DeviceState.Active);
        _deviceButtons.ForEach(device =>
        {
            var match = devices.FirstOrDefault(d => d.FullName.StartsWith(device.Text));
            if (match is null || device is not ToolStripMenuItem menuItem ) return;
            menuItem.Checked = match.IsDefaultDevice;
        });
    }
}