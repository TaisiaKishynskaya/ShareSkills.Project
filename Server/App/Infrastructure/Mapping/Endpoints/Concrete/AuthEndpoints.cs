using App.Infrastructure.Mapping.Endpoints.Abstract;
using Microsoft.AspNetCore.Identity;

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
    }
}