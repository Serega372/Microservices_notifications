namespace AuthService.Application.Configs;

public sealed class DatabaseConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    
    public string DatabaseName { get; set; } = string.Empty;

    public string CreateConnectionString(string? dbName = null)
    {
        return ConnectionString.Replace("[database_name]", dbName ?? DatabaseName);
    }
}