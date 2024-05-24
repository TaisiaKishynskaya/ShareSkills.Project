using Libraries.Data;
using Microsoft.AspNetCore.Identity;

namespace App.Infrastructure.Configurations;

public class AuthorizationConfiguration
{
    public static void ConfigureAuthorization(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization(options => 
        {
            options.AddPolicy("AppScope", policy =>
            {
                policy.RequireAuthenticatedUser(); 
                policy.RequireClaim("scope", "App"); 
            });
        });
        
        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
    }
}