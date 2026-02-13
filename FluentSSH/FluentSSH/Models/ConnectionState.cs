using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FluentSSH.Models;

public class ConnectionState : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _isConnected;
    private bool _isConnecting;
    private string _statusMessage = "Not connected";
    private SSHConfiguration? _activeConfiguration;

    public bool IsConnected
    {
        get => _isConnected;
        set => SetProperty(ref _isConnected, value);
    }

    public bool IsConnecting
    {
        get => _isConnecting;
        set => SetProperty(ref _isConnecting, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public SSHConfiguration? ActiveConfiguration
    {
        get => _activeConfiguration;
        set => SetProperty(ref _activeConfiguration, value);
    }

    public bool CanConnect => !IsConnected && !IsConnecting && ActiveConfiguration != null;
    public bool CanDisconnect => IsConnected && !IsConnecting;

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        
        if (propertyName is nameof(IsConnected) or nameof(IsConnecting) or nameof(ActiveConfiguration))
        {
            OnPropertyChanged(nameof(CanConnect));
            OnPropertyChanged(nameof(CanDisconnect));
        }
        
        return true;
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
