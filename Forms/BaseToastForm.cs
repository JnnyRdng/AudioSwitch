using System.Runtime.InteropServices;
using AudioSwitch.Utils;

namespace AudioSwitch.Forms;

public partial class BaseToastForm : Form
{
    private readonly float _maxOpacity = 1;
    protected bool DisableFade { get; init; } = false;

    protected float MaxOpacity
    {
        get => _maxOpacity;
        init => _maxOpacity = Math.Clamp(value, 0f, 1f);
    }

    protected int ToastDuration { get; init; } = 1000;

    protected BaseToastForm()
    {
        TopMost = true;
        ShowInTaskbar = false;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        Size = new Size(350, 100);
        SetRoundedRegion();
    }

    protected async Task ShowToast()
    {
        Opacity = DisableFade ? _maxOpacity : 0;
        Show();
        await FadeIn();
        await Task.Delay(ToastDuration);
        await FadeOut();
        Close();
    }

    private async Task FadeIn()
    {
        if (DisableFade) return;

        var timer = new AsyncTimer(Constants.TickInterval);
        timer.Tick += (s, e) =>
        {
            Opacity += 0.2;
            if (Opacity >= MaxOpacity)
            {
                timer.Stop();
            }
        };
        timer.Start();
        await timer.WaitUntilStoppedAsync();
    }

    private async Task FadeOut()
    {
        if (DisableFade) return;

        var timer = new AsyncTimer(Constants.TickInterval);
        timer.Tick += (s, e) =>
        {
            Opacity -= 0.1;
            if (Opacity <= 0)
            {
                timer.Stop();
            }
        };
        timer.Start();
        await timer.WaitUntilStoppedAsync();
    }

    #region Internal

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
    private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect,
        int nTopRect,
        int nRightRect,
        int nBottomRect,
        int nWidthEllipse,
        int nHeightEllipse
    );

    private const int SwShowNoActivate = 4;

    protected override bool ShowWithoutActivation => true;

    private void SetRoundedRegion()
    {
        const int radius = 40;
        var handle = CreateRoundRectRgn(0, 0, Width, Height, radius, radius);
        Region = Region.FromHrgn(handle);
    }

    private new void Show()
    {
        ShowWindow(Handle, SwShowNoActivate);
    }

    #endregion
}