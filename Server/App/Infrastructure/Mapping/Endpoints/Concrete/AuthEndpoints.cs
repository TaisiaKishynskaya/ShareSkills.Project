using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using App.Infrastructure.Mapping.Endpoints.Abstract;
using App.Services.Abstract;
using FluentValidation;
using Libraries.Contracts.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace App.Infrastructure.Mapping.Endpoints.Concrete;

public class AuthEndpoints : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/register", async (IUserService userService, UserForCreationDto user, IValidator<UserForCreationDto> validator) =>
        {
            var validationResult = await validator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            
            if (await userService.GetByEmailAsync(user.Email) is not null)
            {
                return Results.BadRequest("User already exists");
            }
            var userModel = new UserModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Role = user.Role
            };

            var passwordHasher = new PasswordHasher<UserModel>();
            var passwordHash = passwordHasher.HashPassword(userModel, user.Password);
            userModel.PasswordHash = passwordHash;
            
            var userId = await userService.CreateAsync(userModel);
            
            return Results.Ok(userId);
        });
        
        routeBuilder.MapPost("/login", async (IUserService userService, IConfiguration configuration, HttpContext httpContext, string email, string password) =>
        {
            var user = await userService.GetByEmailAsync(email);

            if (user is null)
            {
                return Results.NotFound("User not found");
            }

            var userModel = new UserModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Role = user.Role
            };
            
            var passwordHasher = new PasswordHasher<UserModel>();
            if (passwordHasher.VerifyHashedPassword(userModel, userModel.PasswordHash, password) == PasswordVerificationResult.Failed)
            {
                return Results.BadRequest("Wrong password");
            }
            
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name), //TODO: claims should be extended
                new Claim(ClaimTypes.Role, user.Role),
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
            
            // Create Cookie Authentication after successful login
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Sign in with cookie
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            
            return Results.Ok(new { Token = tokenString, UserId = user.Id });
        });
    }
}