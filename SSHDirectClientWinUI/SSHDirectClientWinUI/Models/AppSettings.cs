using System;
using System.IO;
using System.Text.Json;

namespace SSHDirectClientWinUI.Models;

public class AppSettings
{
    private static readonly string SettingsFilePath;
    private static SettingsData _settings;
    private static readonly object _lock = new();

    static AppSettings()
    {
        var folderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BlackPlatinum",
            "SSH-Direct Client");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        SettingsFilePath = Path.Combine(folderPath, "settings.json");
        _settings = LoadSettings();
    }

    private static SettingsData LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                return JsonSerializer.Deserialize<SettingsData>(json) ?? new SettingsData();
            }
        }
        catch
        {
            // Ignore errors, use defaults
        }
        return new SettingsData();
    }

    private static void SaveSettings()
    {
        lock (_lock)
        {
            try
            {
                var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFilePath, json);
            }
            catch
            {
                // Ignore save errors
            }
        }
    }

    public static string ProxyAddress
    {
        get => _settings.ProxyAddress;
        set { _settings.ProxyAddress = value; SaveSettings(); }
    }

    public static int ProxyPort
    {
        get => _settings.ProxyPort;
        set { _settings.ProxyPort = value; SaveSettings(); }
    }

    public static int ConnectionTimeout
    {
        get => _settings.ConnectionTimeout;
        set { _settings.ConnectionTimeout = value; SaveSettings(); }
    }

    public static int KeepAliveInterval
    {
        get => _settings.KeepAliveInterval;
        set { _settings.KeepAliveInterval = value; SaveSettings(); }
    }

    public static int MaxRetries
    {
        get => _settings.MaxRetries;
        set { _settings.MaxRetries = value; SaveSettings(); }
    }

    public static int LastSelectedConfigId
    {
        get => _settings.LastSelectedConfigId;
        set { _settings.LastSelectedConfigId = value; SaveSettings(); }
    }

    public static string Theme
    {
        get => _settings.Theme;
        set { _settings.Theme = value; SaveSettings(); }
    }

    private class SettingsData
    {
        public string ProxyAddress { get; set; } = "127.0.0.1";
        public int ProxyPort { get; set; } = 1080;
        public int ConnectionTimeout { get; set; } = 0;
        public int KeepAliveInterval { get; set; } = 0;
        public int MaxRetries { get; set; } = 10;
        public int LastSelectedConfigId { get; set; } = -1;
        public string Theme { get; set; } = "Default";
    }
}
