using AuthService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AuthService.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static void MigrateDatabase(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            db.Database.Migrate();
        }
    }
}