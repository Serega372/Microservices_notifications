using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotificationsGateway.Application.Configs;
using NotificationsGateway.Application.Interfaces;
using NotificationsGateway.Application.Services;
using NotificationsGateway.Core.Interfaces;
using NotificationsGateway.Infrastructure.Kafka.Producer;
using NotificationsGateway.Infrastructure.Repositories;
using Npgsql;

namespace NotificationsGateway.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var jwtConfig = configuration.GetSection("JwtConfig").Get<JwtConfig>();

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtConfig.SecretKey))
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["someInformation"];
                        return Task.CompletedTask;
                    }
                };
            });
    }
    
    public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));
        services.Configure<KafkaProducerConfig>(configuration.GetSection("Kafka"));
        services.Configure<DatabaseConfig>(configuration.GetSection("DatabaseConnections"));
    }

    public static void AddKafkaProducer<TMessage>(this IServiceCollection services)
    {
        services.AddSingleton<IKafkaProducerService<TMessage>, KafkaProducerService<TMessage>>();
    }
    
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationsService, NotificationsService>();
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