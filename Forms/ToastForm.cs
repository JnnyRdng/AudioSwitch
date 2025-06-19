using AudioSwitch.App;
using AudioSwitch.Services;
using AudioSwitch.Utils;

namespace AudioSwitch.Forms;

public sealed partial class ToastForm : BaseToastForm
{
    private AppSettings Settings { get; }

    public ToastForm(string message)
    {
        Settings = SettingsService.Settings;
        DisableFade = Settings.DisableFade;
        MaxOpacity = Settings.ToastOpacity;
        ToastDuration = Settings.ToastDuration;

        var isDarkMode = ThemeUtils.IsDark(Settings.AppTheme);
        BackColor = isDarkMode ? Color.Black : Color.White;
        Controls.Add(new Label
        {
            Text = message,
            ForeColor = isDarkMode ? Color.White : Color.Black,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12),
        });
    }

    public new async Task ShowToast()
    {
        var screen = Screen.FromPoint(Cursor.Position).WorkingArea;
        var x = screen.Left + (screen.Width - Width) / 2;
        var y = screen.Bottom - Height - 40;
        Location = new Point(x, y);

        await base.ShowToast();
    }
}