using AudioSwitch.Enum;
using AudioSwitch.Services;
using Newtonsoft.Json;

namespace AudioSwitch.App;

public class AppSettings
{
    #region Fields

    private Theme _appTheme = Theme.System;
    private List<DeviceHotKey> _deviceHotKeys = new();
    private float _toastOpacity = 0.8f;
    private int _toastDuration = 1_000;
    private bool _disableFade = false;
    private bool _playSound = false;

    public Theme AppTheme
    {
        get => _appTheme;
        set => Set(ref _appTheme, value);
    }

    public List<DeviceHotKey> DeviceHotKeys
    {
        get => _deviceHotKeys;
        set => Set(ref _deviceHotKeys, value);
    }

    public float ToastOpacity
    {
        get => _toastOpacity;
        set => Set(ref _toastOpacity, Math.Clamp(value, 0.05f, 1.0f));
    }

    public int ToastDuration
    {
        get => _toastDuration;
        set => Set(ref _toastDuration, value);
    }

    public bool DisableFade
    {
        get => _disableFade;
        set => Set(ref _disableFade, value);
    }

    public bool PlaySound
    {
        get => _playSound;
        set => Set(ref _playSound, value);
    }

    #endregion

    #region Utils

    [JsonIgnore] private bool _preventSave;

    public static AppSettings Deserialize(string json)
    {
        var settings = new AppSettings
        {
            _preventSave = true
        };
        JsonConvert.PopulateObject(json, settings);
        settings._preventSave = false;
        return settings;
    }

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    private void Set<T>(ref T field, T value,
        [System.Runtime.CompilerServices.CallerMemberName]
        string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnSettingChange(propertyName, value);
    }

    protected virtual void OnSettingChange(string? propertyName, object? value)
    {
        if (_preventSave) return;
        Console.WriteLine($"Setting changed: {propertyName} -> {value}");
        SettingsService.Save();
    }

    #endregion
}