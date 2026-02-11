using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SSHDirectClientWinUI.Models;
using System.Collections.Generic;

namespace SSHDirectClientWinUI.Dialogs;

public sealed partial class ConfigurationDialog : ContentDialog
{
    private readonly SSHConfiguration? _existingConfig;
    private readonly Brush _errorBrush;
    private readonly Brush? _normalBrush;
    
    public SSHConfiguration? Configuration { get; private set; }

    public ConfigurationDialog(SSHConfiguration? existingConfig = null)
    {
        InitializeComponent();
        
        _existingConfig = existingConfig;
        _errorBrush = new SolidColorBrush(Microsoft.UI.Colors.Red);
        
        // Store the original brush from a TextBox (get it after InitializeComponent)
        _normalBrush = NameTextBox.BorderBrush;

        if (_existingConfig != null)
        {
            Title = "Edit configuration";
            NameTextBox.Text = _existingConfig.Name;
            ServerAddressTextBox.Text = _existingConfig.ServerAddress;
            ServerPortNumberBox.Value = _existingConfig.ServerPort;
            UsernameTextBox.Text = _existingConfig.Username;
            PasswordInput.Password = _existingConfig.Password;
        }
    }


    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var errors = ValidateFields();
        
        if (errors.Count > 0)
        {
            args.Cancel = true;
            ErrorInfoBar.Message = string.Join("\n", errors);
            ErrorInfoBar.IsOpen = true;
            return;
        }

        ErrorInfoBar.IsOpen = false;

        Configuration = new SSHConfiguration
        {
            Id = _existingConfig?.Id ?? 0,
            Name = NameTextBox.Text.Trim(),
            ServerAddress = ServerAddressTextBox.Text.Trim(),
            ServerPort = (int)ServerPortNumberBox.Value,
            Username = UsernameTextBox.Text.Trim(),
            Password = PasswordInput.Password
        };
    }

    private List<string> ValidateFields()
    {
        var errors = new List<string>();
        
        // Reset borders
        ResetFieldBorders();

        // Validate Name
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            errors.Add("Configuration name is required.");
            SetFieldError(NameTextBox);
        }

        // Validate Server Address
        if (string.IsNullOrWhiteSpace(ServerAddressTextBox.Text))
        {
            errors.Add("Server address is required.");
            SetFieldError(ServerAddressTextBox);
        }

        // Validate Server Port
        if (double.IsNaN(ServerPortNumberBox.Value) || ServerPortNumberBox.Value < 1 || ServerPortNumberBox.Value > 65535)
        {
            errors.Add("Server port must be between 1 and 65535.");
            ServerPortNumberBox.BorderBrush = _errorBrush;
        }

        // Validate Username
        if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
        {
            errors.Add("Username is required.");
            SetFieldError(UsernameTextBox);
        }

        // Validate Password
        if (string.IsNullOrWhiteSpace(PasswordInput.Password))
        {
            errors.Add("Password is required.");
            PasswordInput.BorderBrush = _errorBrush;
        }

        return errors;
    }

    private void SetFieldError(TextBox textBox)
    {
        textBox.BorderBrush = _errorBrush;
    }

    private void ResetFieldBorders()
    {
        NameTextBox.BorderBrush = _normalBrush;
        ServerAddressTextBox.BorderBrush = _normalBrush;
        ServerPortNumberBox.BorderBrush = _normalBrush;
        UsernameTextBox.BorderBrush = _normalBrush;
        PasswordInput.BorderBrush = _normalBrush;
    }

    private void Field_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Clear error state when user starts typing
        if (sender is TextBox textBox && textBox.BorderBrush == _errorBrush)
        {
            textBox.BorderBrush = _normalBrush;
        }
        
        if (ErrorInfoBar.IsOpen)
        {
            ErrorInfoBar.IsOpen = false;
        }
    }

    private void Field_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (sender.BorderBrush == _errorBrush)
        {
            sender.BorderBrush = _normalBrush;
        }
        
        if (ErrorInfoBar.IsOpen)
        {
            ErrorInfoBar.IsOpen = false;
        }
    }

    private void Field_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox && passwordBox.BorderBrush == _errorBrush)
        {
            passwordBox.BorderBrush = _normalBrush;
        }
        
        if (ErrorInfoBar.IsOpen)
        {
            ErrorInfoBar.IsOpen = false;
        }
    }
}
