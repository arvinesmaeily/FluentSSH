using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using FluentSSH.Models;
using FluentSSH.Pages;
using FluentSSH.Services;
using System;
using System.IO;
using System.Text;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI;
using WinRT.Interop;

namespace FluentSSH;

public sealed partial class MainWindow : Window
{
    public static MainWindow? Instance { get; private set; }
    
    public DatabaseService Database { get; } = new DatabaseService();
    public SshConnectionService SshConnection { get; } = new SshConnectionService();
    public ConnectionState ConnectionState { get; } = new ConnectionState();
    
    private readonly StringBuilder _logBuilder = new();
    private const int MaxLogLength = 50000;
    private AppWindow? _appWindow;

    public MainWindow()
    {
        Instance = this;
        InitializeComponent();

        // Set up custom title bar
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        
        // Set title bar icon
        var iconPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Icon", "icon.png");
        if (File.Exists(iconPath))
        {
            AppIcon.Source = new BitmapImage(new Uri(iconPath));
        }
        
        // Get the AppWindow and customize title bar button colors
        SetupTitleBarColors();

        // Subscribe to SSH service events
        SshConnection.LogMessage += OnLogMessage;
        SshConnection.Connected += OnSshConnected;
        SshConnection.Disconnected += OnSshDisconnected;
        SshConnection.ErrorOccurred += OnSshError;

        // Load last selected configuration
        LoadLastConfiguration();
        
        Closed += OnWindowClosed;
        
        // Update title bar colors when theme changes
        if (Content is FrameworkElement rootElement)
        {
            rootElement.ActualThemeChanged += (s, e) => SetupTitleBarColors();
        }
    }

    private void SetupTitleBarColors()
    {
        var hwnd = WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        _appWindow = AppWindow.GetFromWindowId(windowId);

        _appWindow?.SetIcon("Resources/Icon/icon.ico");

        if (_appWindow?.TitleBar != null && AppWindowTitleBar.IsCustomizationSupported())
        {
            var titleBar = _appWindow.TitleBar;
            
            // Determine if we're in light or dark theme
            var isDark = Content is FrameworkElement fe && fe.ActualTheme == ElementTheme.Dark;
            
            if (isDark)
            {
                // Dark theme colors
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(30, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(50, 255, 255, 255);
            }
            else
            {
                // Light theme colors
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(30, 0, 0, 0);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(50, 0, 0, 0);
            }
            
            // Transparent background for all states
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }
    }

    private void LoadLastConfiguration()
    {
        var lastId = AppSettings.LastSelectedConfigId;
        if (lastId > 0)
        {
            var config = Database.GetConfiguration(lastId);
            if (config != null)
            {
                ConnectionState.ActiveConfiguration = config;
            }
        }
    }

    private void NavView_Loaded(object sender, RoutedEventArgs e)
    {
        // Select Home page by default
        NavView.SelectedItem = NavView.MenuItems[0];
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            ContentFrame.Navigate(typeof(SettingsPage));
            NavView.Header = "Settings";
        }
        else if (args.SelectedItem is NavigationViewItem item)
        {
            var tag = item.Tag?.ToString();
            switch (tag)
            {
                case "HomePage":
                    ContentFrame.Navigate(typeof(HomePage));
                    NavView.Header = "Home";
                    break;
                case "ConfigurationsPage":
                    ContentFrame.Navigate(typeof(ConfigurationsPage));
                    NavView.Header = "Configurations";
                    break;
            }
        }
    }

    public void Log(string message)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var line = $"[{timestamp}] {message}\n";
        
        _logBuilder.Append(line);
        
        // Trim if too long
        if (_logBuilder.Length > MaxLogLength)
        {
            _logBuilder.Remove(0, _logBuilder.Length - MaxLogLength);
        }
    }

    public string GetLogs() => _logBuilder.ToString();

    public void ClearLogs()
    {
        _logBuilder.Clear();
    }

    private void OnLogMessage(object? sender, string message)
    {
        Log(message);
    }

    private void OnSshConnected(object? sender, EventArgs e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            ConnectionState.IsConnected = true;
            ConnectionState.IsConnecting = false;
            ConnectionState.StatusMessage = $"Connected to {ConnectionState.ActiveConfiguration?.Name}";
        });
    }

    private void OnSshDisconnected(object? sender, EventArgs e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            ConnectionState.IsConnected = false;
            ConnectionState.IsConnecting = false;
            ConnectionState.StatusMessage = "Disconnected";
        });
    }

    private void OnSshError(object? sender, Exception e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            ConnectionState.StatusMessage = $"Error: {e.Message}";
        });
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        SshConnection.Dispose();
        Database.Dispose();
    }
}
