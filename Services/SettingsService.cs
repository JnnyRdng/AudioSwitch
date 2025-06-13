using System.Reflection;
using AudioSwitch.Context;

namespace AudioSwitch.Services;

public static class SettingsService
{
    public static AppSettings Settings { get; private set; } = null!;

    private static readonly string ConfigDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.AppName);

    private static readonly string ConfigPath = Path.Combine(ConfigDir, "settings.json");

    public static void Load()
    {
        if (File.Exists(ConfigPath))
        {
            try
            {
                var json = File.ReadAllText(ConfigPath);
                Settings = AppSettings.Deserialize(json);;
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not read settings file");
                Console.WriteLine(e);
            }
        }

        Settings = new AppSettings();
    }

    public static void Save()
    {
        Directory.CreateDirectory(ConfigDir);
        File.WriteAllText(ConfigPath, Settings.Serialize());
    }

    public static string GetVersion()
    {
        var v = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
        var suffix = string.Empty;
#if DEBUG
        suffix = "-dev";
#endif
        return (v ?? "Unknown").Split('+')[0] + suffix;
    }
}