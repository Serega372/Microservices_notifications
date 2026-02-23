using AuthService.WebApi.Extensions;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;
var services = builder.Services;

services.AddConfigurations(configuration);
services.AddDatabaseContext();
services.AddJwtAuthentication(configuration);
services.AddRepositories();
services.AddServices();
services.AddFluentMigrator(configuration);

services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MigrateDatabase();
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
});
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/health", () =>  Results.Ok());
app.MapControllers();

await app.RunAsync();