using AuthService.Application.Configs;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthService.Infrastructure.Migrations;

public static class MigrationService
{
    public static void Migrate(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        var databaseConfig = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseConfig>>().Value;
        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        logger.LogInformation($"Database \"{databaseConfig.DatabaseName}\" migration started");
        
        try
        {
            dbContext.EnsureCreated(databaseConfig.DatabaseName);
            migrationService.ListMigrations();
            migrationService.MigrateUp();
        }
        catch (Exception ex)
        {
            logger.LogError($"Exception: {ex.Message}. Exception while creating database");
            throw;
        }
    }
}