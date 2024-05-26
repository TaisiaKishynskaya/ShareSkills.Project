using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using App.Infrastructure.Mapping.Endpoints.Abstract;
using App.Services.Abstract;
using Libraries.Contracts.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace App.Infrastructure.Mapping.Endpoints.Concrete;

public class AuthEndpoints : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/register", async (IUserService UserService, UserModel user) =>
        {
            if (await UserService.GetByEmailAsync(user.Email) is not null)
            {
                return Results.BadRequest("User already exists");
            }
            
            var passwordHasher = new PasswordHasher<UserModel>();
            var passwordHash = passwordHasher.HashPassword(user, user.PasswordHash);

            user.PasswordHash = passwordHash;
            
            var userId = await UserService.CreateAsync(user);
            
            return Results.Ok(userId);
        });
        
        routeBuilder.MapPost("/login", async (IUserService UserService, IConfiguration configuration, string email, string password) =>
        {
            var user = await UserService.GetByEmailAsync(email);

            if (user is null)
            {
                return Results.NotFound("User not found");
            }

            var userModel = new UserModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                PasswordHash = user.PasswordHash
            };
            
            var passwordHasher = new PasswordHasher<UserModel>();
            if (passwordHasher.VerifyHashedPassword(userModel, userModel.PasswordHash, password) == PasswordVerificationResult.Failed)
            {
                return Results.BadRequest("Wrong password");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email), //TODO: claims should be extended
            };

            var token = new JwtSecurityToken
            (
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15), //TODO: should be changed
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                    SecurityAlgorithms.HmacSha256)
            );
            
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            
            return Results.Ok(tokenString);
        });
        
        routeBuilder.MapGet("/test", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme/*, Roles = "Teacher"*/)]() => "It works");
    }
}