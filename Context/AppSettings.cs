﻿using AudioSwitch.Services;
using Newtonsoft.Json;

namespace AudioSwitch.Context;

public class AppSettings
{
    #region Fields

    private bool _darkMode = true;
    private List<DeviceHotKey> _deviceHotKeys = new();
    private float _toastOpacity = 0.8f;
    private int _toastDuration = 1_000;
    private bool _disableFade = false;

    public bool DarkMode
    {
        get => _darkMode;
        set => Set(ref _darkMode, value);
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