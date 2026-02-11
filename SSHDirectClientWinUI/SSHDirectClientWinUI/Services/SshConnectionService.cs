using Renci.SshNet;
using SSHDirectClientWinUI.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SSHDirectClientWinUI.Services;

public class SshConnectionService : IDisposable
{
    private SshClient? _client;
    private ForwardedPortDynamic? _port;
    private bool _disposed;

    public event EventHandler<string>? LogMessage;
    public event EventHandler<Exception>? ErrorOccurred;
    public event EventHandler? Connected;
    public event EventHandler? Disconnected;

    public bool IsConnected => _client?.IsConnected == true;

    public async Task ConnectAsync(SSHConfiguration config, string proxyAddress, int proxyPort, 
        int timeout, int keepAlive, CancellationToken cancellationToken = default)
    {
        try
        {
            Cleanup();

            _client = new SshClient(config.ServerAddress, config.ServerPort, config.Username, config.Password);
            
            _client.ConnectionInfo.Timeout = timeout == 0 
                ? TimeSpan.FromDays(1) 
                : TimeSpan.FromSeconds(timeout);

            _client.KeepAliveInterval = keepAlive == 0 
                ? TimeSpan.FromMinutes(1) 
                : TimeSpan.FromSeconds(keepAlive);

            _client.ErrorOccurred += OnClientError;

            Log($"Connecting to {config.ServerAddress}:{config.ServerPort}...");
            await _client.ConnectAsync(cancellationToken);

            _port = new ForwardedPortDynamic(proxyAddress, (uint)proxyPort);
            _client.AddForwardedPort(_port);
            _port.Start();

            Log($"Connected! SOCKS5 proxy available on {proxyAddress}:{proxyPort}");
            Connected?.Invoke(this, EventArgs.Empty);
        }
        catch (OperationCanceledException)
        {
            Log("Connection cancelled.");
            Cleanup();
            throw;
        }
        catch (Exception ex)
        {
            Log($"Connection failed: {ex.Message}");
            Cleanup();
            throw;
        }
    }

    public Task DisconnectAsync()
    {
        Log("Disconnecting...");
        Cleanup(disconnect: true);
        Log("Disconnected.");
        Disconnected?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    private void OnClientError(object? sender, Renci.SshNet.Common.ExceptionEventArgs e)
    {
        Log($"SSH Error: {e.Exception.Message}");
        ErrorOccurred?.Invoke(this, e.Exception);
    }

    private void Cleanup(bool disconnect = false)
    {
        try { _port?.Stop(); } catch { }
        try { if (_port != null) _client?.RemoveForwardedPort(_port); } catch { }
        
        if (disconnect)
        {
            try { _client?.Disconnect(); } catch { }
        }

        try { if (_client != null) _client.ErrorOccurred -= OnClientError; } catch { }
        try { _client?.Dispose(); } catch { }
        
        
        _client = null;
        _port = null;
    }

    private void Log(string message)
    {
        LogMessage?.Invoke(this, message);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Cleanup(disconnect: true);
            _disposed = true;
        }
    }
}
