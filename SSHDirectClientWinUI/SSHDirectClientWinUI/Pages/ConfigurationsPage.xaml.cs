using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using SSHDirectClientWinUI.Dialogs;
using SSHDirectClientWinUI.Models;
using System;
using System.Collections.ObjectModel;

namespace SSHDirectClientWinUI.Pages;

public sealed partial class ConfigurationsPage : Page
{
    private readonly ObservableCollection<SSHConfiguration> _configurations = new();
    private MainWindow Main => MainWindow.Instance!;

    public ConfigurationsPage()
    {
        InitializeComponent();
        ConfigurationsList.ItemsSource = _configurations;
        
        Main.ConnectionState.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ConnectionState.ActiveConfiguration))
            {
                DispatcherQueue.TryEnqueue(UpdateSelectionIndicators);
            }
        };
        
        LoadConfigurations();
    }

    private void LoadConfigurations()
    {
        _configurations.Clear();
        
        var configs = Main.Database.GetAllConfigurations();
        foreach (var config in configs)
        {
            _configurations.Add(config);
        }

        UpdateEmptyState();
        
        // Update selection after items are loaded
        DispatcherQueue.TryEnqueue(UpdateSelectionIndicators);
    }

    private void UpdateEmptyState()
    {
        EmptyState.Visibility = _configurations.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void UpdateSelectionIndicators()
    {
        // This will be called after layout - we need to find and update selection indicators
        var activeId = Main.ConnectionState.ActiveConfiguration?.Id ?? -1;
        
        // Iterate through all items in the ItemsControl
        for (int i = 0; i < _configurations.Count; i++)
        {
            var container = ConfigurationsList.ContainerFromIndex(i) as ContentPresenter;
            if (container != null)
            {
                var border = FindChildByName<Border>(container, "SelectionIndicator");
                if (border != null)
                {
                    border.Visibility = _configurations[i].Id == activeId ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
    }

    private static T? FindChildByName<T>(DependencyObject parent, string name) where T : FrameworkElement
    {
        int childCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T element && element.Name == name)
            {
                return element;
            }
            
            var result = FindChildByName<T>(child, name);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    private async void AddConfigurationButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ConfigurationDialog();
        dialog.XamlRoot = XamlRoot;
        dialog.RequestedTheme = ActualTheme;

        var result = await dialog.ShowAsync();
        
        if (result == ContentDialogResult.Primary && dialog.Configuration != null)
        {
            Main.Database.InsertConfiguration(dialog.Configuration);
            LoadConfigurations();
            Main.Log($"Added configuration: {dialog.Configuration.Name}");
        }
    }

    private async void EditConfigButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is SSHConfiguration config)
        {
            var dialog = new ConfigurationDialog(config);
            dialog.XamlRoot = XamlRoot;
            dialog.RequestedTheme = ActualTheme;

            var result = await dialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary && dialog.Configuration != null)
            {
                Main.Database.UpdateConfiguration(dialog.Configuration);
                LoadConfigurations();
                
                // Update active configuration if it was edited
                if (Main.ConnectionState.ActiveConfiguration?.Id == config.Id)
                {
                    Main.ConnectionState.ActiveConfiguration = dialog.Configuration;
                }
                
                Main.Log($"Updated configuration: {dialog.Configuration.Name}");
            }
        }
    }

    private async void RemoveConfigButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is SSHConfiguration config)
        {
            var confirmDialog = new ContentDialog
            {
                Title = "Remove configuration",
                Content = $"Are you sure you want to remove \"{config.Name}\"?",
                PrimaryButtonText = "Remove",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = XamlRoot,
                RequestedTheme = ActualTheme
            };

            var result = await confirmDialog.ShowAsync();
            
            if (result == ContentDialogResult.Primary)
            {
                Main.Database.DeleteConfiguration(config);
                
                // Clear active configuration if it was removed
                if (Main.ConnectionState.ActiveConfiguration?.Id == config.Id)
                {
                    Main.ConnectionState.ActiveConfiguration = null;
                    AppSettings.LastSelectedConfigId = -1;
                }
                
                LoadConfigurations();
                Main.Log($"Removed configuration: {config.Name}");
            }
        }
    }

    private void ConfigItem_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border border && border.DataContext is SSHConfiguration config)
        {
            Main.ConnectionState.ActiveConfiguration = config;
            AppSettings.LastSelectedConfigId = config.Id;
            Main.Log($"Selected configuration: {config.Name}");
            UpdateSelectionIndicators();
        }
    }

    private void ConfigItem_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border border)
        {
            border.Background = (Brush)Application.Current.Resources["CardBackgroundFillColorSecondaryBrush"];
        }
    }

    private void ConfigItem_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border border)
        {
            border.Background = (Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"];
        }
    }
}
