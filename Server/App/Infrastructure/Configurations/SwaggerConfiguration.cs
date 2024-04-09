using Microsoft.OpenApi.Models;

namespace App.Infrastructure.Configurations;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            
        });

        return services; 
    }
}