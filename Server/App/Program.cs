using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using App;
using App.Infrastructure.Configurations;
using App.Infrastructure.Mapping.Endpoints.Concrete;
using App.Services.Abstract;
using App.Services.Concrete;
using Libraries.Configurations;
using Libraries.Contracts.User;
using Libraries.Data;
using Libraries.Data.UnitOfWork.Abstract;
using Libraries.Data.UnitOfWork.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

// СМОТРЕТЬ https://github.com/TaisiaKishynskaya/CSharp_A-Level/tree/main/eShop.Project/Application/Catalog/Catalog.API

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // --------------------------------------------------------------------------------------------------------------
        // ДОБАВЬТЕ ЭТОТ КОД когда пропишете классы с конфигурациями

        AuthenticationConfiguration.ConfigureAuthentication(builder);
        AuthorizationConfiguration.ConfigureAuthorization(builder); 

        builder.Services.AddSwagger(builder.Configuration); 

        /* не добавлять - черновик
        builder.Services.AddScoped<DbContext, AppDbContext>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IStudentService, StudentService>();
        builder.Services.AddScoped<ITeacherService, TeacherService>();*/
        
        DatabaseConfiguration.ConfigureDatabase(builder);
        ServicesConfiguration.ConfigureServices(builder);
        builder.Services.AddMinimalEndpoints();

        //----------------------------------------------------------------------------------------------------------------
        var app = builder.Build();
        
        app.UseAuthentication();
        
        app.UseAuthorization();
        
        app.UseHttpsRedirection();

        app.RegisterStudentEndpoint();
        
        app.RegisterMinimalEndpoints();

        AppConfiguration.ConfigureApp(app);
        

        /*app.MapPost("/register", async (IUserService UserService, UserForCreationDto user) =>
        {
            await UserService.CreateAsync(user);
        });
        
        app.MapPost("/login", async (IUserService UserService, IConfiguration configuration, string email, string password) =>
        {
            var user = await UserService.GetByEmailAsync(email);

            if (user is null)
            {
                return Results.NotFound("The user not found");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email), //TODO: claims should be extended
            };

        var token = new JwtSecurityToken
        (
            issuer: builder.Configuration["Jwt:Issuer"],
            audience: builder.Configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(60),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                SecurityAlgorithms.HmacSha256)
        );
            
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            
            return Results.Ok(tokenString);
        });
        
        app.MapGet("/test", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "Administrator"#1#)]() => "It works");
        */
    
        app.Run();
    }
}
