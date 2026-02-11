using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SSHDirectClientWinUI.Models;
using System;
using Windows.System;

namespace SSHDirectClientWinUI.Pages;

public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        ProxyAddressTextBox.Text = AppSettings.ProxyAddress;
        ProxyPortNumberBox.Value = AppSettings.ProxyPort;
        TimeoutNumberBox.Value = AppSettings.ConnectionTimeout;
        KeepAliveNumberBox.Value = AppSettings.KeepAliveInterval;
        MaxRetriesNumberBox.Value = AppSettings.MaxRetries;

        // Set theme selection
        var theme = AppSettings.Theme;
        foreach (ComboBoxItem item in ThemeComboBox.Items)
        {
            if (item.Tag?.ToString() == theme)
            {
                ThemeComboBox.SelectedItem = item;
                break;
            }
        }

        if (ThemeComboBox.SelectedItem == null)
        {
            ThemeComboBox.SelectedIndex = 0;
        }
    }

    private void ProxyAddressTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var address = ProxyAddressTextBox.Text.Trim();
        if (!string.IsNullOrEmpty(address))
        {
            AppSettings.ProxyAddress = address;
        }
    }

    private void ProxyPortNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (!double.IsNaN(args.NewValue))
        {
            AppSettings.ProxyPort = (int)args.NewValue;
        }
    }

    private void TimeoutNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (!double.IsNaN(args.NewValue))
        {
            AppSettings.ConnectionTimeout = (int)args.NewValue;
        }
    }

    private void KeepAliveNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (!double.IsNaN(args.NewValue))
        {
            AppSettings.KeepAliveInterval = (int)args.NewValue;
        }
    }

    private void MaxRetriesNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (!double.IsNaN(args.NewValue))
        {
            AppSettings.MaxRetries = (int)args.NewValue;
        }
    }

    private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ThemeComboBox.SelectedItem is ComboBoxItem item && item.Tag is string theme)
        {
            AppSettings.Theme = theme;
            
            // Apply theme immediately
            if (MainWindow.Instance?.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = theme switch
                {
                    "Light" => ElementTheme.Light,
                    "Dark" => ElementTheme.Dark,
                    _ => ElementTheme.Default
                };
            }
        }
    }

    private async void GitHubCard_Click(object sender, RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new Uri("https://github.com/arvinesmaeily/SSHDirectClient"));
    }
}
