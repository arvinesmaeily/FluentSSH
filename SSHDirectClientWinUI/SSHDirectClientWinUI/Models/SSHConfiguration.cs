using SQLite;

namespace SSHDirectClientWinUI.Models;

[Table("SSHConfigs")]
public class SSHConfiguration
{
    [PrimaryKey, AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("ServerAddress")]
    public string ServerAddress { get; set; } = string.Empty;

    [Column("ServerPort")]
    public int ServerPort { get; set; } = 22;

    [Column("Username")]
    public string Username { get; set; } = string.Empty;

    [Column("Password")]
    public string Password { get; set; } = string.Empty;
}
