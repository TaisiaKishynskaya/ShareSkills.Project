using App.Infrastructure.Mapping.Endpoints.Abstract;
using App.Services.Abstract;
using Libraries.Contracts.User;

namespace App.Infrastructure.Mapping.Endpoints.Concrete;

public class UserEndpoint : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/users", async (IUserService service) =>
            {
                var user = await service.GetAllAsync();

                return Results.Ok(user);
            })
            .WithOpenApi();

        routeBuilder.MapGet("/users/{id:guid}", async (Guid id, IUserService service) =>
            {
                var user = await service.GetByIdAsync(id);

                return Results.Ok(user);
            })
            .WithOpenApi();

        routeBuilder.MapPut("/users/{id:guid}", async (Guid id, UserForUpdateDto dto, IUserService service) =>
            {
                await service.UpdateAsync(id, dto);

                return Results.NoContent();
            })
            .WithOpenApi();

        routeBuilder.MapDelete("/users/{id:guid}", async (Guid id, IUserService service) =>
            {
                await service.DeleteAsync(id);

                return Results.NoContent();
            })
            .WithOpenApi();

        routeBuilder.MapPost("/getId", async (IUserService UserService, IConfiguration configuration, string email) => 
        {
            var user = await UserService.GetByEmailAsync(email);
            return Results.Ok(new {userId = user.Id});
        });
    }
}