using AudioSwitch.App;
using AudioSwitch.Services;

namespace AudioSwitch;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        _ = new Mutex(true, Constants.AppName, out var isNewInstance);
        if (!isNewInstance)
        {
            Console.WriteLine("Application is already running");
            return;
        }

        SettingsService.Load();


        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new TrayAppContext());
    }
}