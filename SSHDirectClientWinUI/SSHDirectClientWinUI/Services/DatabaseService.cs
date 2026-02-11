using SQLite;
using SSHDirectClientWinUI.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace SSHDirectClientWinUI.Services;

public class DatabaseService : IDisposable
{
    private readonly SQLiteConnection _db;
    private bool _disposed;

    public DatabaseService()
    {
        var folderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BlackPlatinum",
            "SSH-Direct Client");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var dbPath = Path.Combine(folderPath, "configs.db");
        _db = new SQLiteConnection(dbPath);
        _db.CreateTable<SSHConfiguration>();
    }

    public List<SSHConfiguration> GetAllConfigurations()
    {
        return _db.Table<SSHConfiguration>().ToList();
    }

    public SSHConfiguration? GetConfiguration(int id)
    {
        return _db.Find<SSHConfiguration>(id);
    }

    public void InsertConfiguration(SSHConfiguration config)
    {
        _db.Insert(config);
    }

    public void UpdateConfiguration(SSHConfiguration config)
    {
        _db.Update(config);
    }

    public void DeleteConfiguration(SSHConfiguration config)
    {
        _db.Delete(config);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _db?.Dispose();
            _disposed = true;
        }
    }
}
