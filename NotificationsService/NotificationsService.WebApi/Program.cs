using NotificationsService.Application.Models;
using NotificationsService.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;
// Add services to the container.
services.AddConfigurations(configuration);
services.AddDatabaseContext();
services.AddKafkaProducer<SendEmailMessageDto>();
services.AddKafkaProducer<SendSmsMessageDto>();
services.AddKafkaProducer<SendPushMessageDto>();
services.AddRepositories();
services.AddServices();
services.AddFluentMigrator(configuration);
services.AddControllers();

var app = builder.Build();
// Configure the HTTP request pipeline.
app.MigrateDatabase();
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok());
app.MapControllers();
await app.RunAsync();