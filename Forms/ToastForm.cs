using System.Runtime.InteropServices;
using AudioSwitch.App;
using AudioSwitch.Services;
using AudioSwitch.Utils;
using Timer = System.Windows.Forms.Timer;

namespace AudioSwitch.Forms;

public partial class ToastForm : Form
{
    private AppSettings Settings { get; }

    public ToastForm(string message)
    {
        Settings = SettingsService.Settings;
        var isDarkMode = ThemeUtils.IsDark(Settings.DarkMode);
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        Size = new Size(350, 100);
        BackColor = isDarkMode ? Color.Black : Color.White;
        Opacity = 0;
        TopMost = true;
        ShowInTaskbar = false;

        var label = new Label
        {
            Text = message,
            ForeColor = isDarkMode ? Color.White : Color.Black,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12),
        };

        Controls.Add(label);
        SetRoundedRegion();
    }
    
    protected override CreateParams CreateParams
    {
        get
        {
            var handleParam = base.CreateParams;
            handleParam.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED       
            return handleParam;
        }
    }
    
    public sealed override Color BackColor
    {
        get => base.BackColor;
        set => base.BackColor = value;
    }
    
    [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
    private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect,
        int nTopRect,
        int nRightRect,
        int nBottomRect,
        int nWidthEllipse,
        int nHeightEllipse
    );
    
    private void SetRoundedRegion()
    {
        const int radius = 20; // Adjust for more/less roundness
        var handle = CreateRoundRectRgn(0, 0, Width, Height, radius, radius);
        Region = Region.FromHrgn(handle);
    }

    public async Task ShowToast()
    {
        var screen = Screen.FromPoint(Cursor.Position).WorkingArea;
        var x = screen.Left + (screen.Width - Width) / 2;
        var y = screen.Bottom - Height - 40;

        Location = new Point(x, y);
        Show();

        FadeIn();
        await Task.Delay(Settings.ToastDuration);
        FadeOut();
    }

    private void FadeIn()
    {
        if (Settings.DisableFade)
        {
            Opacity = Settings.ToastOpacity;
            return;
        }
        var timer = new Timer { Interval = 17 };
        timer.Tick += (s, e) =>
        {
            if (Opacity < Settings.ToastOpacity)
            {
                Opacity += 0.2;
            }
            else
            {
                timer.Stop();
            }
        };
        timer.Start();
    }

    private void FadeOut()
    {
        if (Settings.DisableFade)
        {
            Opacity = 0;
            return;
        }
        var outTimer = new Timer { Interval = 17 };
        outTimer.Tick += (s, e) =>
        {
            if (Opacity > 0)
            {
                Opacity -= 0.1;
            }
            else
            {
                outTimer.Stop();
                Close();
            }
        };
        outTimer.Start();
    }
}