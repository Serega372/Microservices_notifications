using FluentMigrator.Runner;
using NotificationsService.Application.Configs;
using NotificationsService.Infrastructure.Migrations;

namespace NotificationsService.WebApi.Extensions;

public static class DatabaseExtensions
{
    public static void AddDatabaseContext(this IServiceCollection services)
    {
        services.AddScoped<DatabaseContext>();
    }

    public static void AddFluentMigrator(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConfig = configuration.GetSection("DatabaseConnections").Get<DatabaseConfig>();

        services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb.AddPostgres()
                .WithGlobalConnectionString(dbConfig?.CreateConnectionString())
                .ScanIn(typeof(InitialMigration).Assembly).For.Migrations());
    }

    public static void MigrateDatabase(this IHost host)
    {
        MigrationService.Migrate(host.Services);
    }
}