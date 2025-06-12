using System.Reflection;
using System.Text.Json;
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
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                if (settings != null)
                {
                    Settings = settings;
                    return;
                }
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
        Console.WriteLine(ConfigPath);
        var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
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