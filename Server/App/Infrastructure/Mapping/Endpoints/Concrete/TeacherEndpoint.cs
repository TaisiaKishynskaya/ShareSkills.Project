using App.Infrastructure.Mapping.Endpoints.Abstract;
using App.Services.Abstract;
using Libraries.Contracts.Teacher;
using Libraries.Contracts.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace App.Infrastructure.Mapping.Endpoints.Concrete;

public class TeacherEndpoint : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/teachers", 
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Teacher)]
                async (ITeacherService service) =>
            {
                var teacher = await service.GetAllAsync();

                return Results.Ok(teacher);
            })
            .WithOpenApi();

        routeBuilder.MapGet("/teachers/{id:guid}",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Teacher)]
                async (Guid id, ITeacherService service) =>
            {
                var teacher = await service.GetByIdAsync(id);

                return Results.Ok(teacher);
            })
            .WithOpenApi();

        routeBuilder.MapPost("/teachers", 
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Teacher)]
                async (TeacherForCreationDto dto, ITeacherService service) =>
            {
                var teacher = await service.CreateAsync(dto);

                return Results.Created($"/teachers/{teacher.Id}", teacher);
            })
            .WithOpenApi();

        routeBuilder.MapDelete("/teachers/{id:guid}", 
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.Teacher)]
                async (Guid id, ITeacherService service) =>
            {
                await service.DeleteAsync(id);

                return Results.NoContent();
            })
            .WithOpenApi();
    }
}