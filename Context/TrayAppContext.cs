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
    private const int HotkeyId1 = 1;
    private const int HotkeyId2 = 2;


    public TrayAppContext()
    {
        var dummyForm = new HiddenForm();
        var modifiers = HotKeys.Modifiers(ModifierKeys.Control, ModifierKeys.Alt, ModifierKeys.Shift);
        HotKeys.RegisterMultipleHotKeys(dummyForm.Handle,
            (HotkeyId1, modifiers, Keys.OemOpenBrackets),
            (HotkeyId2, modifiers, Keys.OemCloseBrackets)
        );

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
                    new ToolStripMenuItem("Exit", null, (s, e) =>
                    {
                        HotKeys.UnregisterMultipleHotKeys(dummyForm.Handle, HotkeyId1, HotkeyId2);
                        if (_trayIcon != null)
                        {
                            _trayIcon.Visible = false;
                        }

                        _trayIcon?.Dispose();
                        Application.Exit();
                    })
                }
            }
        };
    }

    private async void OnHotKeyPressed(int id)
    {
        var deviceName = id switch
        {
            HotkeyId1 => "Headset (USB Audio Device)",
            HotkeyId2 => "Speakers (Realtek(R) Audio)",
            _ => throw new ArgumentOutOfRangeException($"Unknown hotkey ID {id}")
        };
        await SwitchTo(deviceName);
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