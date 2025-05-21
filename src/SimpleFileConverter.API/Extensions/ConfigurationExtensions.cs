using SimpleFileConverter.API.Interfaces.Services;
using SimpleFileConverter.API.Services;

namespace SimpleFileConverter.API.Extensions;

public static class ConfigurationExtensions
{
    public static WebApplicationBuilder AddSettingsConfigurations(this WebApplicationBuilder builder)
    {
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

        return builder;
    }
    public static IServiceCollection AddApiInfrastructure(this IServiceCollection services)
    {


        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                 builder =>
                 {
                     builder
                            .WithOrigins("*")
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                 });
        });

        services.AddScoped<IConverterService, ConverterService>();
        services.AddScoped<ISampleService,SampleService>(); 
        services.AddControllers();

        return services;
    }
}
