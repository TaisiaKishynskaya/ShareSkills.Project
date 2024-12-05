using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace App.Infrastructure.Configurations;

public static class AuthenticationConfiguration
{
    public static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
            {
                // Default schemes
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Use cookies for sign-in
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateActor = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/login"; // Путь для перенаправления при необходимости входа
                options.Cookie.Name = "ShareSkills_App_Cookie"; // Название куки
                options.Cookie.HttpOnly = true; // Защита от XSS
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Использовать только через HTTPS
                options.ExpireTimeSpan = TimeSpan.FromDays(30); // Кука действительна 30 дней
                
                // Явно указываем срок жизни куки на стороне клиента
                options.Cookie.MaxAge = options.ExpireTimeSpan; // Эквивалентно 30 дням
            });
    }
}