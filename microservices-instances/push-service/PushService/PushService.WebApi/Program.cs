using PushService.WebApi.Extensions;
using PushService.Application.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.
services.AddConfigurations(configuration);
services.AddDatabaseContext();
services.AddKafkaConsumer<PushMessageResponse>();
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