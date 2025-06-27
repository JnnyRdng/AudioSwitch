using System.Runtime.InteropServices;
using System.Text;
using AudioSwitch.App;
using AudioSwitch.Extensions;

namespace AudioSwitch.Utils;

public static class Native
{
    #region PInvoke

    private const string Dll = "user32.dll";


    [DllImport(Dll, SetLastError = true, EntryPoint = "RegisterHotKey")]
    private static extern bool ExternRegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport(Dll, SetLastError = true, EntryPoint = "UnregisterHotKey")]
    private static extern bool ExternUnregisterHotKey(IntPtr hWnd, int id);

    [DllImport(Dll, EntryPoint = "ToUnicode")]
    private static extern int ExternToUnicode(uint wVirtKey, uint wScanCode, byte[] lpKeyState,
        [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)]
        StringBuilder pwszBuff, int cchBuff, uint wFlags);

    [DllImport(Dll, EntryPoint = "GetKeyboardState")]
    private static extern bool ExternGetKeyboardState(byte[] lpKeyState);

    [DllImport(Dll, EntryPoint = "MapVirtualKey")]
    private static extern uint ExternMapVirtualKey(uint uCode, uint uMapType);

    #endregion

    #region Public Wrappers

    public static bool RegisterHotKey(IntPtr hWnd, DeviceHotKey hotkey)
    {
        if (ExternRegisterHotKey(hWnd, hotkey.Id, hotkey.GetModifiers(), hotkey.GetKey())) return true;
        var error = Marshal.GetLastWin32Error();
        Console.WriteLine($"Failed to register hotkey '{hotkey}', threw error code: '{error}'");
        return false;
    }

    public static bool UnregisterHotKey(IntPtr hWnd, DeviceHotKey hotkey)
    {
        if (ExternUnregisterHotKey(hWnd, hotkey.Id)) return true;
        var error = Marshal.GetLastWin32Error();
        Console.WriteLine($"Failed to unregister hotkey '{hotkey}', threw error code: '{error}'");
        return false;
    }

    public static int ToUnicode(Keys key, byte[] keyboardState, out StringBuilder sb)
    {
        sb = new StringBuilder(2);
        var scanCode = ExternMapVirtualKey(key.AsUint(), 0);
        return ExternToUnicode(key.AsUint(), scanCode, keyboardState, sb, sb.Capacity, 0);
    }

    public static bool GetKeyboardState(out byte[] keyboardState)
    {
        keyboardState = new byte[256];
        return ExternGetKeyboardState(keyboardState);
    }

    #endregion
}