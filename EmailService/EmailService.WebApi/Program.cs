
using EmailService.Application.Models;
using EmailService.WebApi.Extensions;

namespace EmailService.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            var services = builder.Services;

            // Add services to the container.
            services.AddConfigurations(configuration);
            services.AddDatabaseContext();
            services.AddKafkaConsumer<EmailMessageResponse>();
            services.AddRepositories();
            services.AddServices();
            services.AddFluentMigrator(configuration);
            services.AddControllers();

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            app.MigrateDatabase();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
