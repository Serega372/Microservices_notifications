using AuthService.Application.Configs;
using AuthService.Application.Interfaces;
using AuthService.Application.Services;
using AuthService.Core.Interfaces;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Reflection;

namespace AuthService.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Auth API",
                Description = "An ASP.NET Core Web API for authentication and authorization"
            });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccessTokenRepository, AccessTokenRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAccessTokenService, AccessTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, Application.Services.AuthService>();
        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<DatabaseContext>());
    }

    public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DatabaseContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    }

    public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PasswordHasherConfig>(configuration.GetSection("PasswordHasherConfig"));
    }
}