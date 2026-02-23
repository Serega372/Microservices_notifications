using AuthService.Application.Configs;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace AuthService.Infrastructure.Migrations;

public sealed class DatabaseContext
{
    private readonly ILogger<DatabaseContext> _logger;
    
    private string ConnectionString { get; init; }
    
    public DatabaseContext(
        IOptions<DatabaseConfig> databaseConfig,
        ILogger<DatabaseContext> logger)
    {
        _logger = logger;
        ConnectionString = databaseConfig.Value.CreateConnectionString("postgres");
        
        _logger.LogInformation($"{GetType().Name} was initialized");
    }
    
    public void EnsureCreated(string dbName)
    {
        const string checkQuery = "SELECT COUNT(oid) FROM pg_database WHERE datname = @name";

        var parameters = new DynamicParameters();
        parameters.Add("@name", dbName);
        
        using var connection = new NpgsqlConnection(ConnectionString);
        bool databaseExists;

        try
        { 
            var count = connection.ExecuteScalar<int>(checkQuery, parameters);
            databaseExists = count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception: [{ex.Message}]. Failed to create database");
            throw;
        }

        if (databaseExists) return;
        
        _logger.LogInformation($"Creating Database {dbName}");
        connection.Execute($"CREATE DATABASE {dbName}");
    }
}