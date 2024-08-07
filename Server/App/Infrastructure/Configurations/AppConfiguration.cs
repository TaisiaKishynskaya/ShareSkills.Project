using App.Infrastructure.Exceptions;
using Libraries.Data;
using Serilog;

namespace App.Infrastructure.Configurations;

public static class AppConfiguration
{
    public static void ConfigureApp(WebApplication app)
    {
        if (app.Environment.IsDevelopment()) 
        {
            app.UseSwagger(); 
            app.UseSwaggerUI(); 
        }
        
        app.UseMiddleware<ExceptionMiddleware>();
        
        //app.UseAuthentication();
        //app.UseAuthorization();
        //app.MapControllers().RequireAuthorization("AppScope");
        
        AppDbInitializer.EnsureDatabaseCreated(app.Services);
        
        var serilogConfig = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("serilog.json")
            .Build();
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(serilogConfig)
            .CreateLogger();
    }
}