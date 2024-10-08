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
        
        routeBuilder.MapPost("/login", async (IUserService userService, IConfiguration configuration, HttpContext httpContext, string? email, string? password, bool authMethodCookie) =>
        {
            if (authMethodCookie)
            {
                return await LoginWithCookies(httpContext, userService);
            }

            return await LoginWithJwt(userService, configuration, httpContext, email, password);
        });

        async Task<IResult> LoginWithJwt(IUserService userService, IConfiguration configuration, HttpContext httpContext, string email, string password)
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
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                    SecurityAlgorithms.HmacSha256)
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Сохранение куки для последующего входа
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return Results.Ok(new { Token = tokenString, UserId = user.Id });
        }

        // Метод для логина через куки
        async Task<IResult> LoginWithCookies(HttpContext httpContext, IUserService userService)
        {
            // Если куки уже установлены и валидны
            if (httpContext.User.Identity != null)
            {
                var isAuthenticated = httpContext.User.Identity.IsAuthenticated;
                Console.WriteLine($"User.Identity: {httpContext.User.Identity.Name}, IsAuthenticated: {isAuthenticated}");

                if (isAuthenticated)
                {
                    var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                    
                    var user = await userService.GetByEmailAsync(email);

                    return user is not null 
                        ? Results.Ok(new { Message = "Logged in with cookies", UserId = user.Id }) 
                        : Results.NotFound("User not found");
                }

                return Results.BadRequest("User is not authenticated with cookies.");
            }

            return Results.BadRequest("User identity is null.");
        }



        routeBuilder.MapGet("/dashboard", 
            [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},{CookieAuthenticationDefaults.AuthenticationScheme}")]
            async (HttpContext httpContext) =>
        {
            // Проверяем, аутентифицирован ли пользователь через куки
            return httpContext.User.Identity.IsAuthenticated 
                ? Results.Ok(new { Message = "Welcome back, you are logged in with cookies!" }) 
                : Results.Unauthorized();
        });
    }
}