using AuthService.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

services.AddConfigurations(configuration);
services.AddDbContext(configuration);
services.AddRepositories();
services.AddServices();
services.AddControllers();
services.AddSwagger();
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(swaggerUiOptions =>
    {
        swaggerUiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "AiJudge API V1");
        swaggerUiOptions.InjectJavascript("/swagger/custom-auth.js");
    });

    app.UseDeveloperExceptionPage();
}

app.MigrateDatabase();
app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseAuthorization();
app.MapGet("/health", () => Results.Ok());
app.MapControllers();

await app.RunAsync();