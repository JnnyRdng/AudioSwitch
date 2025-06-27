namespace AudioSwitch.Forms;

public partial class HiddenForm : Form
{
    public event Action<int>? HotKeyPressed;

    protected override void WndProc(ref Message m)
    {
        const int wmHotkey = 0x0312;
        if (m.Msg == wmHotkey)
        {
            var id = m.WParam.ToInt32();
            HotKeyPressed?.Invoke(id);
            return;
        }
        base.WndProc(ref m);
    }
    public HiddenForm()
    {
       ShowInTaskbar = false;
       WindowState = FormWindowState.Minimized;
       Visible = false;
    }
}