using System.Runtime.InteropServices;
using Timer = System.Windows.Forms.Timer;

namespace AudioSwitch.Forms;

public partial class ToastForm : Form
{
    private const double MaxOpacity = 0.75;
    private const int ShowDuration = 1000;

    public ToastForm(string message)
    {
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        Size = new Size(350, 100);
        BackColor = Color.Black;
        Opacity = 0;
        TopMost = true;
        ShowInTaskbar = false;

        var label = new Label
        {
            Text = message,
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 12),
        };

        Controls.Add(label);
        SetRoundedRegion();
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
        var screen = Screen.PrimaryScreen?.WorkingArea;
        if (screen == null) return;
        var w = screen.Value.Width;
        var h = screen.Value.Height;
        Location = new Point((w / 2) - (Width / 2), h - Height - 40);
        Show();

        FadeIn();
        await Task.Delay(ShowDuration);
        FadeOut();
    }

    private void FadeIn()
    {
        var timer = new Timer { Interval = 17 };
        timer.Tick += (s, e) =>
        {
            if (Opacity < MaxOpacity)
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