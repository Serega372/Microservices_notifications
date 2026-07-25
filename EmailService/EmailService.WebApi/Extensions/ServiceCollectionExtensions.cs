using Microsoft.Extensions.Options;
using Npgsql;
using EmailService.Application.Configs;
using EmailService.Application.Handlers;
using EmailService.Application.Interfaces;
using EmailService.Application.Models;
using EmailService.Core.Interfaces;
using EmailService.Infrastructure.DataSources;
using EmailService.Infrastructure.Kafka.Consumer;
using EmailService.Infrastructure.Repositories;

namespace EmailService.WebApi.Extensions;

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
        services.AddScoped<IEmailsRepository, EmailsRepository>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IMessageHandler<EmailMessageResponse>, EmailMessageHandler>();
    }
}