using Microsoft.EntityFrameworkCore;

namespace App.Infrastructure.Configurations;

public class DatabaseConfiguration
{
    public static void ConfigureDatabase(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("ShareSkillsDb"); 
        var serverVersion = new MySqlServerVersion(new Version(8,3,0)); // Вказати потрібну версію сервера MySQL
        
        builder.Services.AddDbContext<AppDbContext>(options => options
            .UseMySql(connectionString, serverVersion,b => b.MigrationsAssembly("App")));
    }
}