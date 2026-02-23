using FluentMigrator.Runner;
using SmsService.Application.Configs;
using SmsService.Infrastructure.Migrations;

namespace SmsService.WebApi.Extensions;

public static class DatabaseExtensions
{
    public static void AddDatabaseContext(this IServiceCollection services)
    {
        services.AddScoped<DatabaseContext>();
    }

    public static void AddFluentMigrator(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConfig = configuration.GetSection("DatabaseConnections:Messages").Get<MessagesDatabaseConfig>();

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