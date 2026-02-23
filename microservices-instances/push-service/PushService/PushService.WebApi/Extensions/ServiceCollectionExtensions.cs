using Microsoft.Extensions.Options;
using Npgsql;
using PushService.Application.Configs;
using PushService.Application.Handlers;
using PushService.Application.Interfaces;
using PushService.Application.Models;
using PushService.Core.Interfaces;
using PushService.Infrastructure.DataSources;
using PushService.Infrastructure.Kafka.Consumer;
using PushService.Infrastructure.Repositories;

namespace PushService.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaConsumerConfig>(configuration.GetSection("Kafka"));
        services.Configure<MessagesDatabaseConfig>(configuration.GetSection("DatabaseConnections:Messages"));
        services.Configure<NotificationsDatabaseConfig>(configuration.GetSection("DatabaseConnections:Notifications"));
    }

    public static void AddKafkaConsumer<TMessage>(this IServiceCollection services)
    {
        services.AddHostedService<KafkaConsumerService<TMessage>>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IMessageHandler<PushMessageResponse>, PushMessageHandler>();
    }
    
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<MessagesDataSource>(sp =>
        {
            var cfg = sp.GetRequiredService<IOptions<MessagesDatabaseConfig>>().Value;
            var builder = new NpgsqlDataSourceBuilder(cfg.CreateConnectionString());
            return new MessagesDataSource(builder.Build());
        });
        
        services.AddSingleton<NotificationsDataSource>(sp =>
        {
            var cfg = sp.GetRequiredService<IOptions<NotificationsDatabaseConfig>>().Value;
            var builder = new NpgsqlDataSourceBuilder(cfg.CreateConnectionString());
            return new NotificationsDataSource(builder.Build());
        });
        
        services.AddScoped<INotificationsRepository, NotificationsRepository>();
        services.AddScoped<IPushMessagesRepository, PushMessagesRepository>();
    }
}