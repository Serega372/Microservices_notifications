using Microsoft.Extensions.Options;
using NotificationsService.Application.Configs;
using NotificationsService.Application.Interfaces;
using NotificationsService.Application.Services;
using NotificationsService.Core.Interfaces;
using NotificationsService.Infrastructure.Kafka.Producer;
using NotificationsService.Infrastructure.Repositories;
using Npgsql;

namespace NotificationsService.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaProducerConfig>(configuration.GetSection("Kafka"));
        services.Configure<DatabaseConfig>(configuration.GetSection("DatabaseConnections"));
    }

    public static void AddKafkaProducer<TMessage>(this IServiceCollection services)
    {
        services.AddSingleton<IKafkaProducerService<TMessage>, KafkaProducerService<TMessage>>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationsService, Application.Services.NotificationsService>();
        services.AddHostedService<RetryMessagesService>();
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var cfg = sp.GetRequiredService<IOptions<DatabaseConfig>>().Value;
            var builder = new NpgsqlDataSourceBuilder(cfg.CreateConnectionString());
            return builder.Build();
        });

        services.AddScoped<INotificationsRepository, NotificationsRepository>();
        services.AddScoped<IRetryMessagesRepository, RetryMessagesRepository>();
    }
}