using SimpleFileConverter.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddSettingsConfigurations();
builder.Services.AddApiInfrastructure();

var app = builder.Build();
app.UseCors("CorsPolicy");
app.MapControllers();
await app.RunAsync();
