using App.Infrastructure.Mapping.Endpoints.Abstract;
using App.Services.Abstract;
using Libraries.Contracts.Teacher;

namespace App.Infrastructure.Mapping.Endpoints.Concrete;

public class TeacherEndpoint : IMinimalEndpoint
{
    public void MapRoutes(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("/teachers", async (ITeacherService service) =>
            {
                var teacher = await service.GetAllAsync();

                return Results.Ok(teacher);
            })
            .WithOpenApi();

        routeBuilder.MapGet("/teachers/{id:guid}", async (Guid id, ITeacherService service) =>
            {
                var teacher = await service.GetByIdAsync(id);

                return Results.Ok(teacher);
            })
            .WithOpenApi();

        routeBuilder.MapPost("/teachers", async (TeacherForCreationDto dto, ITeacherService service) =>
            {
                var teacher = await service.CreateAsync(dto);

                return Results.Created($"/teachers/{teacher.Id}", teacher);
            })
            .WithOpenApi();

        routeBuilder.MapPut("/teachers/{id:guid}", async (Guid id, TeacherForUpdateDto dto, ITeacherService service) =>
            {
                await service.UpdateAsync(id, dto);

                return Results.NoContent();
            })
            .WithOpenApi();

        routeBuilder.MapDelete("/teachers/{id:guid}", async (Guid id, ITeacherService service) =>
            {
                await service.DeleteAsync(id);

                return Results.NoContent();
            })
            .WithOpenApi();
    }
}