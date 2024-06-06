using App.Infrastructure.Mapping.Endpoints.Abstract;
using App.Services.Abstract;
using Libraries.Contracts.Grade;
using Libraries.Contracts.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Infrastructure.Mapping.Endpoints.Concrete;

public class GradeEndpoint : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/grades", 
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Student)]
                async ([FromServices]IGradeService service) =>
                {
                    var grade = await service.GetAllAsync();

                    return Results.Ok(grade);
                })
            .WithOpenApi();

        routeBuilder.MapGet("/grades/{id:guid}",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Student)]
                async (Guid id, [FromServices]IGradeService service) =>
                {
                    var grade = await service.GetByIdAsync(id);

                    return Results.Ok(grade);
                })
            .WithOpenApi();

        routeBuilder.MapPost("/grades",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Student)]
                async (GradeForCreatingDto dto, [FromServices]IGradeService service) =>
                {
                    var grade = await service.CreateAsync(dto);

                    return Results.Created($"/grades/{grade.Id}", grade);
                })
            .WithOpenApi();

        routeBuilder.MapPut("/grades/{id:guid}", 
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Student)]
                async (Guid id, GradeForUpdateDto dto, [FromServices]IGradeService service) =>
                {
                    await service.UpdateAsync(id, dto);

                    return Results.NoContent();
                })
            .WithOpenApi();

        routeBuilder.MapDelete("/grades/{id:guid}", 
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Student)]
                async (Guid id, [FromServices]IGradeService service) =>
                {
                    await service.DeleteAsync(id);

                    return Results.NoContent();
                })
            .WithOpenApi();
    }
}