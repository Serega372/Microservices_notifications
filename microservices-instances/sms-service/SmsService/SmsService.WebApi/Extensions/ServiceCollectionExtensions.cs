using Microsoft.Extensions.Options;
using Npgsql;
using SmsService.Application.Configs;
using SmsService.Application.Handlers;
using SmsService.Application.Interfaces;
using SmsService.Application.Models;
using SmsService.Core.Interfaces;
using SmsService.Infrastructure.DataSources;
using SmsService.Infrastructure.Kafka.Consumer;
using SmsService.Infrastructure.Repositories;

namespace SmsService.WebApi.Extensions;

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
        services.AddScoped<IMessageHandler<SmsMessageResponse>, SmsMessageHandler>();
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
        services.AddScoped<ISmsMessagesRepository, SmsMessagesRepository>();
    }
}