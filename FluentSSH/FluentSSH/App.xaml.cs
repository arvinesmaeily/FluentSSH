using Microsoft.UI.Xaml;
using FluentSSH.Models;

namespace FluentSSH;

public partial class App : Application
{
    private Window? _window;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        
        // Apply saved theme
        if (_window.Content is FrameworkElement rootElement)
        {
            var theme = AppSettings.Theme;
            rootElement.RequestedTheme = theme switch
            {
                "Light" => ElementTheme.Light,
                "Dark" => ElementTheme.Dark,
                _ => ElementTheme.Default
            };
        }
        
        _window.Activate();
    }
}
