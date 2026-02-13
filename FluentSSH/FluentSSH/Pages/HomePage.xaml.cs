using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using FluentSSH.Models;
using System;
using System.Threading;
using Windows.UI;

namespace FluentSSH.Pages;

public sealed partial class HomePage : Page
{
    private CancellationTokenSource? _connectCts;
    private MainWindow Main => MainWindow.Instance!;

    // Status colors
    private static readonly SolidColorBrush GrayBrush = new(Colors.Gray);
    private static readonly SolidColorBrush GreenBrush = new(Color.FromArgb(255, 16, 124, 16)); // #107C10
    private static readonly SolidColorBrush RedBrush = new(Color.FromArgb(255, 209, 52, 56));   // #D13438
    private static readonly SolidColorBrush YellowBrush = new(Color.FromArgb(255, 252, 185, 0)); // #FCB900

    public HomePage()
    {
        InitializeComponent();
        
        Main.ConnectionState.PropertyChanged += ConnectionState_PropertyChanged;
        Main.SshConnection.LogMessage += OnLogMessage;
        
        // Load logs when expander is opened
        LogsExpander.RegisterPropertyChangedCallback(
            CommunityToolkit.WinUI.Controls.SettingsExpander.IsExpandedProperty,
            (s, e) =>
            {
                if (LogsExpander.IsExpanded)
                {
                    var logs = Main.GetLogs();
                    LogsTextBlock.Text = string.IsNullOrEmpty(logs) ? "No logs yet." : logs;
                }
            });
        
        UpdateUI();
    }

    private void ConnectionState_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        DispatcherQueue.TryEnqueue(UpdateUI);
    }

    private void UpdateUI()
    {
        var state = Main.ConnectionState;
        
        // Update selected config text
        if (state.ActiveConfiguration != null)
        {
            SelectedConfigText.Text = $"{state.ActiveConfiguration.Name} ({state.ActiveConfiguration.ServerAddress}:{state.ActiveConfiguration.ServerPort})";
        }
        else
        {
            SelectedConfigText.Text = "No configuration selected. Go to Configurations to add one.";
        }

        // Update buttons
        ConnectButton.IsEnabled = state.CanConnect;
        ConnectButton.Visibility = state.IsConnected ? Visibility.Collapsed : Visibility.Visible;
        DisconnectButton.Visibility = state.IsConnected ? Visibility.Visible : Visibility.Collapsed;
        DisconnectButton.IsEnabled = state.CanDisconnect;

        // Update connecting panel
        ConnectingPanel.Visibility = state.IsConnecting ? Visibility.Visible : Visibility.Collapsed;

        // Update status indicator and text
        if (state.IsConnecting)
        {
            StatusIndicator.Fill = YellowBrush;
            StatusText.Text = "Attempting to connect...";
        }
        else if (state.IsConnected)
        {
            StatusIndicator.Fill = GreenBrush;
            StatusText.Text = $"Connected to {state.ActiveConfiguration?.Name}";
            
            ProxyInfoCard.Visibility = Visibility.Visible;
            ProxyAddressText.Text = AppSettings.ProxyAddress;
            ProxyPortText.Text = AppSettings.ProxyPort.ToString();
        }
        else
        {
            // Check if there's an error
            var hasError = !string.IsNullOrEmpty(state.StatusMessage) && 
                          state.StatusMessage != "Not connected" && 
                          state.StatusMessage != "Disconnected" &&
                          state.StatusMessage != "Connection cancelled" &&
                          (state.StatusMessage.Contains("Failed") || 
                           state.StatusMessage.Contains("Error") ||
                           state.StatusMessage.Contains("refused") ||
                           state.StatusMessage.Contains("timeout"));
            
            StatusIndicator.Fill = hasError ? RedBrush : GrayBrush;
            StatusText.Text = hasError ? "Connection failed. Check logs for details." : "Disconnected";
            
            ProxyInfoCard.Visibility = Visibility.Collapsed;
        }
    }

    private async void ConnectButton_Click(object sender, RoutedEventArgs e)
    {
        var config = Main.ConnectionState.ActiveConfiguration;
        if (config == null) return;


        Main.ConnectionState.IsConnecting = true;
        _connectCts?.Cancel();
        _connectCts = new CancellationTokenSource();

        try
        {
            await Main.SshConnection.ConnectAsync(
                config,
                AppSettings.ProxyAddress,
                AppSettings.ProxyPort,
                AppSettings.ConnectionTimeout,
                AppSettings.KeepAliveInterval,
                _connectCts.Token);
        }
        catch (OperationCanceledException)
        {
            Main.ConnectionState.StatusMessage = "Connection cancelled";
        }
        catch (Exception ex)
        {
            Main.ConnectionState.StatusMessage = $"Failed: {ex.Message}";
        }
        finally
        {
            Main.ConnectionState.IsConnecting = false;
        }
    }

    private async void DisconnectButton_Click(object sender, RoutedEventArgs e)
    {
        _connectCts?.Cancel();
        await Main.SshConnection.DisconnectAsync();
    }

    private void ClearLogsButton_Click(object sender, RoutedEventArgs e)
    {
        Main.ClearLogs();
        LogsTextBlock.Text = "Logs cleared.";
    }

    private void OnLogMessage(object? sender, string message)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            if (LogsExpander.IsExpanded)
            {
                LogsTextBlock.Text = Main.GetLogs();
            }
        });
    }
}
