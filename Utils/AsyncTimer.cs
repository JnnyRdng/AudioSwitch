using Timer = System.Windows.Forms.Timer;

namespace AudioSwitch.Utils;

public class AsyncTimer
{
    private readonly Timer _timer;
    private TaskCompletionSource<bool>? _tcs;

    public event EventHandler? Tick
    {
        add => _timer.Tick += value;
        remove => _timer.Tick -= value;
    }

    public AsyncTimer(int interval)
    {
        _timer = new Timer { Interval = interval };
    }

    public void Start()
    {
        _tcs = new TaskCompletionSource<bool>();
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
        _tcs?.TrySetResult(true);
    }

    public Task WaitUntilStoppedAsync()
    {
        return _tcs?.Task ?? Task.CompletedTask;
    }
}