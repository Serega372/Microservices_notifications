using Microsoft.EntityFrameworkCore;
using NotificationsService.Application.Configs;
using NotificationsService.Application.Interfaces;
using NotificationsService.Infrastructure;
using NotificationsService.Infrastructure.Caching;
using NotificationsService.Infrastructure.ExternalServices;
using NotificationsService.Infrastructure.Kafka.Producer;

namespace NotificationsService.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaProducerConfig>(configuration.GetSection("Kafka"));
        services.Configure<DatabaseConfig>(configuration.GetSection("ConnectionStrings"));
        services.Configure<ExternalServicesConfig>(configuration.GetSection("ExternalServices"));
    }

    public static void AddKafkaProducer<TMessage>(this IServiceCollection services)
    {
        services.AddSingleton<IKafkaProducerService<TMessage>, KafkaProducerService<TMessage>>();
    }

    public static void AddServices(this IServiceCollection services)
    {
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IMemoryCacheStorage, MemoryCacheStorage>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<DatabaseContext>());
    }

    public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DatabaseContext>(options => options
            .UseNpgsql(configuration.GetConnectionString("NotificationDatabase"))
            .UseSnakeCaseNamingConvention());
    }

    public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        var externalServicesConfig = configuration.GetSection("ExternalServices").Get<ExternalServicesConfig>();
        services.AddHttpClient<IAuthServiceStorage, AuthServiceStorage>(client =>
        {
            var baseUrl = externalServicesConfig.AuthServiceOptions.BaseUrl;
            client.BaseAddress = new Uri(baseUrl.EndsWith("/") ? baseUrl : $"{baseUrl}/");
        });
    }
}