using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using App.Infrastructure.Mapping.Endpoints.Abstract;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace App.Infrastructure.Mapping.Endpoints.Concrete;

public class AuthEndpoints : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/register", async (UserManager<IdentityUser> userManager, string email, string password) =>
        {
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return Results.Ok(new { Message = "User registered successfully" });
            }

            return Results.BadRequest(result.Errors);
        });
        

        routeBuilder.MapPost("/login", async (UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, string email, string password) =>
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
                if (result.Succeeded)
                {
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JwtSettings:SecretKey"]));

                    var token = new JwtSecurityToken(
                        issuer: configuration["JwtSettings:Issuer"],
                        audience: configuration["JwtSettings:Audience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                    return Results.Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }
            }
            return Results.Unauthorized();
        });
    }
}