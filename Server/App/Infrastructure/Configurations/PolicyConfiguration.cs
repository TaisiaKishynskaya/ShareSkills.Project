namespace App.Infrastructure.Configurations;

public static class PolicyConfiguration
{
    public static void ConfigureCors(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder => 
            { 
                builder.WithOrigins("https://localhost:7281") // Update this with your Swagger domain
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); // This is needed to allow cookies
                /*builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();*/
            });
        });
    }
}