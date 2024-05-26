using Libraries.Data;
using Microsoft.AspNetCore.Identity;

namespace App.Infrastructure.Configurations;

public static class AuthorizationConfiguration
{
    public static void ConfigureAuthorization(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthorization();
    }
}